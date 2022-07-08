use serde::{Deserialize, Serialize};
use uuid::Uuid;

use crate::types::protocol::channel::Channel;
use crate::util::redacted::Redacted;

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct InviteRequest {
    pub channel: Uuid,
    pub name: String,
    pub world: u16,
    #[serde(with = "serde_bytes")]
    pub encrypted_secret: Redacted<Vec<u8>>,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct InviteResponse {
    pub channel: Uuid,
    pub name: String,
    pub world: u16,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct InvitedResponse {
    pub channel: Channel,
    pub name: String,
    pub world: u16,
    #[serde(with = "serde_bytes")]
    pub pk: Redacted<Vec<u8>>,
    #[serde(with = "serde_bytes")]
    pub encrypted_secret: Redacted<Vec<u8>>,
}
