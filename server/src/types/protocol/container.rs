use serde::{Deserialize, Serialize};

use crate::types::protocol::*;

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct RequestContainer {
    pub number: u32,
    pub kind: RequestKind,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
#[serde(rename_all = "snake_case")]
pub enum RequestKind {
    Ping(PingRequest),
    Version(VersionRequest),
    Register(RegisterRequest),
    Authenticate(AuthenticateRequest),
    Message(MessageRequest),
    Create(CreateRequest),
    Disband(DisbandRequest),
    Invite(InviteRequest),
    Join(JoinRequest),
    Leave(LeaveRequest),
    Kick(KickRequest),
    List(ListRequest),
    Promote(PromoteRequest),
    Update(UpdateRequest),
    PublicKey(PublicKeyRequest),
    Secrets(SecretsRequest),
    SendSecrets(SendSecretsRequest),
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct ResponseContainer {
    pub number: u32,
    pub kind: ResponseKind,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
#[serde(rename_all = "snake_case")]
pub enum ResponseKind {
    Ping(PingResponse),
    Version(VersionResponse),
    Register(RegisterResponse),
    Authenticate(AuthenticateResponse),
    Message(MessageResponse),
    Error(ErrorResponse),
    Create(CreateResponse),
    Disband(DisbandResponse),
    Invite(InviteResponse),
    Invited(InvitedResponse),
    Join(JoinResponse),
    Leave(LeaveResponse),
    Kick(KickResponse),
    List(ListResponse),
    Promote(PromoteResponse),
    Update(UpdateResponse),
    Updated(UpdatedResponse),
    PublicKey(PublicKeyResponse),
    MemberChange(MemberChangeResponse),
    Secrets(SecretsResponse),
    SendSecrets(SendSecretsResponse),
    Announce(AnnounceResponse),
}

macro_rules! request_container {
    ($name:ident, $request:ty) => {
        impl From<$request> for RequestKind {
            fn from(request: $request) -> Self {
                RequestKind::$name(request)
            }
        }
    };
}

request_container!(Ping, PingRequest);
request_container!(Version, VersionRequest);
request_container!(Register, RegisterRequest);
request_container!(Authenticate, AuthenticateRequest);
request_container!(Message, MessageRequest);
request_container!(Create, CreateRequest);
request_container!(Disband, DisbandRequest);
request_container!(Invite, InviteRequest);
request_container!(Join, JoinRequest);
request_container!(Leave, LeaveRequest);
request_container!(Kick, KickRequest);
request_container!(List, ListRequest);
request_container!(Promote, PromoteRequest);
request_container!(Update, UpdateRequest);
request_container!(PublicKey, PublicKeyRequest);
request_container!(Secrets, SecretsRequest);
request_container!(SendSecrets, SendSecretsRequest);

macro_rules! response_container {
    ($name:ident, $response:ty) => {
        impl From<$response> for ResponseKind {
            fn from(response: $response) -> Self {
                ResponseKind::$name(response)
            }
        }
    };
}

response_container!(Ping, PingResponse);
response_container!(Version, VersionResponse);
response_container!(Register, RegisterResponse);
response_container!(Authenticate, AuthenticateResponse);
response_container!(Message, MessageResponse);
response_container!(Error, ErrorResponse);
response_container!(Create, CreateResponse);
response_container!(Disband, DisbandResponse);
response_container!(Invite, InviteResponse);
response_container!(Invited, InvitedResponse);
response_container!(Join, JoinResponse);
response_container!(Leave, LeaveResponse);
response_container!(Kick, KickResponse);
response_container!(List, ListResponse);
response_container!(Promote, PromoteResponse);
response_container!(Update, UpdateResponse);
response_container!(Updated, UpdatedResponse);
response_container!(PublicKey, PublicKeyResponse);
response_container!(MemberChange, MemberChangeResponse);
response_container!(Secrets, SecretsResponse);
response_container!(SendSecrets, SendSecretsResponse);
response_container!(Announce, AnnounceResponse);
