use std::collections::HashMap;
use std::sync::Arc;

use anyhow::{Context, Result};
use lodestone_scraper::LodestoneScraper;
use tokio::sync::RwLock;
use tokio::task::JoinHandle;
use log::{error, info, trace};

use crate::State;

pub fn spawn(state: Arc<RwLock<State>>) -> JoinHandle<()> {
    tokio::task::spawn(async move {
        let lodestone = LodestoneScraper::default();

        loop {
            match inner(&state, &lodestone).await {
                Ok(results) => {
                    let successful = results.values().filter(|result| result.is_ok()).count();
                    info!("Updated {}/{} characters", successful, results.len());
                    for (id, result) in results {
                        if let Err(e) = result {
                            error!("error updating user {}: {:?}", id, e);
                        }
                    }
                }
                Err(e) => {
                    error!("error updating users: {:?}", e);
                }
            }

            tokio::time::sleep(std::time::Duration::from_secs(60)).await;
        }
    })
}

async fn inner(state: &RwLock<State>, lodestone: &LodestoneScraper) -> Result<HashMap<u32, Result<()>>> {
    let users = sqlx::query!(
        // language=sqlite
        "select * from users where (julianday(current_timestamp) - julianday(last_updated)) * 24 >= 2",
    )
        .fetch_all(&state.read().await.db)
        .await
        .context("could not query database for users")?;

    let mut results = HashMap::with_capacity(users.len());
    for user in users {
        results.insert(user.lodestone_id as u32, update(state, lodestone, user.lodestone_id).await);
        tokio::time::sleep(std::time::Duration::from_secs(5)).await;
    }

    Ok(results)
}

async fn update(state: &RwLock<State>, lodestone: &LodestoneScraper, lodestone_id: i64) -> Result<()> {
    let info = lodestone
        .character(lodestone_id as u64)
        .await
        .context("could not get character info")?;
    let world_name = info.world.as_str();

    sqlx::query!(
            // language=sqlite
            "update users set name = ?, world = ?, last_updated = current_timestamp where lodestone_id = ?",
            info.name,
            world_name,
            lodestone_id,
        )
        .execute(&state.read().await.db)
        .await
        .context("could not update user")?;

    trace!("  [updater] before state read");
    let client_state = state.read().await.clients.get(&(lodestone_id as u64)).cloned();
    trace!("  [updater] after state read");
    if let Some(user) = client_state {
        trace!("  [updater] before user write");
        if let Some(user) = user.write().await.user.as_mut() {
            user.name = info.name.clone();
            user.world = info.world;
        }
        trace!("  [updater] after user write");
    }

    Ok(())
}
