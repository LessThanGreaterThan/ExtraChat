use serde::{Deserialize, Serialize};
use crate::util::redacted::Redacted;

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct PublicKeyRequest {
    pub name: String,
    pub world: u16,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct PublicKeyResponse {
    pub name: String,
    pub world: u16,
    #[serde(with = "serde_bytes")]
    pub pk: Option<Redacted<Vec<u8>>>,
}
