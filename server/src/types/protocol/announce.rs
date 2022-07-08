use serde::{Deserialize, Serialize};

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct AnnounceResponse {
    pub announcement: String,
}

impl AnnounceResponse {
    pub fn new(announcement: impl Into<String>) -> Self {
        Self {
            announcement: announcement.into(),
        }
    }
}
