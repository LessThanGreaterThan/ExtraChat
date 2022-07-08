use serde::{Deserialize, Serialize};
use uuid::Uuid;

use crate::Rank;

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct MemberChangeResponse {
    pub channel: Uuid,
    pub name: String,
    pub world: u16,
    pub kind: MemberChangeKind,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
#[serde(rename_all = "snake_case")]
pub enum MemberChangeKind {
    Invite {
        inviter: String,
        inviter_world: u16,
    },
    InviteDecline,
    InviteCancel {
        canceler: String,
        canceler_world: u16,
    },
    Join,
    Leave,
    Promote {
        rank: Rank,
    },
    Kick {
        kicker: String,
        kicker_world: u16,
    },
}
