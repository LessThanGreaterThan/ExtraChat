use serde::{Deserialize, Serialize};
use uuid::Uuid;

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct LeaveRequest {
    pub channel: Uuid,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct LeaveResponse {
    pub channel: Uuid,
    pub error: Option<String>,
}

impl LeaveResponse {
    pub fn success(channel: Uuid) -> Self {
        LeaveResponse {
            channel,
            error: None,
        }
    }

    pub fn error(channel: Uuid, error: impl Into<String>) -> Self {
        LeaveResponse {
            channel,
            error: Some(error.into()),
        }
    }
}
