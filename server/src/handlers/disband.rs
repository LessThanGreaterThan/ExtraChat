use std::sync::Arc;

use anyhow::{Context, Result};
use tokio::sync::RwLock;

use crate::{ClientState, ErrorResponse, Rank, State, WsStream};
use crate::types::protocol::{DisbandRequest, DisbandResponse};
use crate::util::send;

pub async fn disband(state: Arc<RwLock<State>>, client_state: Arc<RwLock<ClientState>>, conn: &mut WsStream, number: u32, req: DisbandRequest) -> Result<()> {
    match client_state.read().await.get_rank(req.channel, &state).await? {
        Some(rank) if rank == Rank::Admin => {}
        _ => return send(conn, number, ErrorResponse::new(req.channel, "not in channel/not enough permissions")).await,
    }

    crate::util::send_to_all(&state, req.channel, 0, DisbandResponse {
        channel: req.channel,
    }).await?;

    let channel_id_str = req.channel.as_simple().to_string();
    sqlx::query!(
        // language=sqlite
        "delete from channels where id = ?",
        channel_id_str,
    )
        .execute(&state.read().await.db)
        .await
        .context("could not disband channel")?;

    send(conn, number, DisbandResponse {
        channel: req.channel,
    }).await
}
