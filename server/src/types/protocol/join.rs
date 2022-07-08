use serde::{Deserialize, Serialize};
use uuid::Uuid;
use crate::types::protocol::channel::Channel;

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct JoinRequest {
    pub channel: Uuid,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct JoinResponse {
    pub channel: Channel,
}
