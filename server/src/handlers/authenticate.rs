use std::str::FromStr;
use std::sync::Arc;

use anyhow::Context;
use chrono::{Duration, Utc};
use lodestone_scraper::LodestoneScraper;
use log::trace;
use tokio::sync::RwLock;

use crate::{AuthenticateRequest, AuthenticateResponse, ClientState, State, User, util, World, WsStream};

pub async fn authenticate(state: Arc<RwLock<State>>, client_state: Arc<RwLock<ClientState>>, conn: &mut WsStream, number: u32, req: AuthenticateRequest) -> anyhow::Result<()> {
    if client_state.read().await.user.is_some() {
        util::send(conn, number, AuthenticateResponse::error("already logged in")).await?;
        return Ok(());
    }

    let key = prefixed_api_key::parse(&*req.key)
        .context("could not parse key")?;
    let hash = util::hash_key(&key);
    let user = sqlx::query!(
        // language=sqlite
        "select * from users where key_short = ? and key_hash = ?",
        key.short_token,
        hash,
    )
        .fetch_optional(&state.read().await.db)
        .await
        .context("could not query database for user")?;
    let mut user = match user {
        Some(u) => u,
        None => {
            util::send(conn, number, AuthenticateResponse::error("invalid key")).await?;
            return Ok(());
        }
    };

    if Utc::now().naive_utc().signed_duration_since(user.last_updated) >= Duration::hours(2) {
        let info = LodestoneScraper::default()
            .character(user.lodestone_id as u64)
            .await
            .context("could not get character info")?;
        let world_name = info.world.as_str();

        user.name = info.name.clone();
        user.world = world_name.to_string();

        sqlx::query!(
            // language=sqlite
            "update users set name = ?, world = ?, last_updated = current_timestamp where lodestone_id = ?",
            info.name,
            world_name,
            user.lodestone_id,
        )
            .execute(&state.read().await.db)
            .await
            .context("could not update user")?;
    }

    let world = World::from_str(&user.world).map_err(|_| anyhow::anyhow!("invalid world in db"))?;

    trace!("  [authenticate] before user write");
    let mut c_state = client_state.write().await;
    c_state.user = Some(User {
        lodestone_id: user.lodestone_id as u64,
        name: user.name.clone(),
        world,
        hash,
    });

    c_state.pk = req.pk.into_inner();
    c_state.allow_invites = req.allow_invites;

    // release lock asap
    drop(c_state);
    trace!("  [authenticate] after user write");

    trace!("  [authenticate] before state write 1");
    state.write().await.clients.insert(user.lodestone_id as u64, Arc::clone(&client_state));
    trace!("  [authenticate] before state write 2");
    state.write().await.ids.insert((user.name, util::id_from_world(world)), user.lodestone_id as u64);
    trace!("  [authenticate] after state writes");

    util::send(conn, number, AuthenticateResponse::success()).await?;

    Ok(())
}
