use anyhow::Result;

use crate::{
    ErrorResponse,
    types::protocol::{
        VersionRequest,
        VersionResponse,
    },
    util::send,
    WsStream,
};

pub async fn version(conn: &mut WsStream, number: u32, req: VersionRequest) -> Result<bool> {
    if req.version != 1 {
        send(conn, number, ErrorResponse::new(None, "unsupported version")).await?;
        return Ok(false);
    }

    send(conn, number, VersionResponse {
        version: 1,
    }).await?;

    Ok(true)
}
