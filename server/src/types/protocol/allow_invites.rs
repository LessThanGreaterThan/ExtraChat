use serde::{Deserialize, Serialize};

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct AllowInvitesRequest {
    pub allowed: bool,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct AllowInvitesResponse {
    pub allowed: bool,
}
