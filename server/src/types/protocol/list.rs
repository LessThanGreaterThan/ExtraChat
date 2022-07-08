use serde::{Deserialize, Serialize};
use uuid::Uuid;

use crate::types::protocol::channel::{Channel, ChannelMember, SimpleChannel};

#[derive(Debug, Clone, Serialize, Deserialize)]
#[serde(rename_all = "snake_case")]
pub enum ListRequest {
    All,
    Channels,
    Members(Uuid),
    Invites,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
#[serde(rename_all = "snake_case")]
pub enum ListResponse {
    All {
        channels: Vec<Channel>,
        invites: Vec<Channel>,
    },
    Channels(Vec<SimpleChannel>),
    Members {
        id: Uuid,
        members: Vec<ChannelMember>,
    },
    Invites(Vec<SimpleChannel>),
}
