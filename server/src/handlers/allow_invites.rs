use std::sync::Arc;

use tokio::sync::RwLock;

use crate::{ClientState, State, util, WsStream};
use crate::types::protocol::{AllowInvitesRequest, AllowInvitesResponse};

pub async fn allow_invites(_state: Arc<RwLock<State>>, client_state: Arc<RwLock<ClientState>>, conn: &mut WsStream, number: u32, req: AllowInvitesRequest) -> anyhow::Result<()> {
    client_state.write().await.allow_invites = req.allowed;
    util::send(conn, number, AllowInvitesResponse {
        allowed: req.allowed,
    }).await
}
