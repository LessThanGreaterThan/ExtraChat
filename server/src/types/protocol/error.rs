use serde::{Deserialize, Serialize};
use uuid::Uuid;

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct ErrorResponse {
    pub channel: Option<Uuid>,
    pub error: String,
}

impl ErrorResponse {
    pub fn new(channel: impl Into<Option<Uuid>>, error: impl Into<String>) -> Self {
        ErrorResponse {
            channel: channel.into(),
            error: error.into(),
        }
    }
}
