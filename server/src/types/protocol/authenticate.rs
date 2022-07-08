use serde::{Deserialize, Serialize};
use crate::util::redacted::Redacted;

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct AuthenticateRequest {
    pub key: Redacted<String>,
    #[serde(with = "serde_bytes")]
    pub pk: Redacted<Vec<u8>>,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct AuthenticateResponse {
    pub error: Option<String>,
}

impl AuthenticateResponse {
    pub fn success() -> Self {
        Self {
            error: None,
        }
    }

    pub fn error(error: impl Into<String>) -> Self {
        Self {
            error: Some(error.into()),
        }
    }
}
