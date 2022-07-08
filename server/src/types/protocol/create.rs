use serde::{Deserialize, Serialize};

use crate::types::protocol::channel::Channel;
use crate::util::redacted::Redacted;

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct CreateRequest {
    #[serde(with = "serde_bytes")]
    pub name: Redacted<Vec<u8>>,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct CreateResponse {
    pub channel: Channel,
}
