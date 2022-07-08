use serde::{Deserialize, Serialize};
use uuid::Uuid;

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct KickRequest {
    pub channel: Uuid,
    pub name: String,
    pub world: u16,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct KickResponse {
    pub channel: Uuid,
    pub name: String,
    pub world: u16,
}
