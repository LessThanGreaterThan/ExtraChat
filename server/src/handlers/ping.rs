use anyhow::Result;

use crate::WsStream;
use crate::types::protocol::PingResponse;

pub async fn ping(conn: &mut WsStream, number: u32) -> Result<()> {
    crate::util::send(conn, number, PingResponse {}).await
}
