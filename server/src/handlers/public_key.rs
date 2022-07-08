use std::sync::Arc;

use anyhow::Result;
use tokio::sync::RwLock;

use crate::{State, WsStream};
use crate::types::protocol::{PublicKeyRequest, PublicKeyResponse};
use crate::util::redacted::Redacted;

pub async fn public_key(state: Arc<RwLock<State>>, conn: &mut WsStream, number: u32, req: PublicKeyRequest) -> Result<()> {
    let id = match state.read().await.ids.get(&(req.name.clone(), req.world)) {
        Some(id) => *id,
        None => {
            crate::util::send(conn, number, PublicKeyResponse {
                name: req.name,
                world: req.world,
                pk: None,
            }).await?;
            return Ok(());
        }
    };

    let pk = match state.read().await.clients.get(&id) {
        Some(client) => Some(client.read().await.pk.clone()),
        None => None,
    };
    crate::util::send(conn, number, PublicKeyResponse {
        name: req.name,
        world: req.world,
        pk: pk.map(Redacted),
    }).await?;

    Ok(())
}
