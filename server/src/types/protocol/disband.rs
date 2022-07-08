use serde::{Deserialize, Serialize};
use uuid::Uuid;

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct DisbandRequest {
    pub channel: Uuid,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct DisbandResponse {
    pub channel: Uuid,
}
