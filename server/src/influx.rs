use std::sync::Arc;
use std::sync::atomic::Ordering;
use std::time::Duration;

use chrono::Utc;
use log::{debug, error};
use reqwest::Client;
use tokio::sync::RwLock;

use crate::{Config, State};

pub fn spawn(config: &Config, state: Arc<RwLock<State>>) {
    let influx = match &config.influx {
        Some(i) => i,
        None => return,
    };

    let mut url = match influx.url.join("/api/v2/write") {
        Ok(url) => url,
        Err(e) => {
            error!("Failed to parse influxdb url: {}", e);
            return;
        }
    };

    url.query_pairs_mut()
        .append_pair("org", &influx.org)
        .append_pair("bucket", &influx.bucket);

    let influx_token = influx.token.clone();

    tokio::task::spawn(async move {
        let mut last_messages = 0;

        let client = Client::new();

        loop {
            let messages = state.read().await.messages_sent.load(Ordering::SeqCst);
            let diff = messages - last_messages;
            last_messages = messages;

            let clients = state.read().await.clients.len();

            let timestamp = Utc::now().timestamp_nanos();

            let line_format = format!(
                "logged_in value={logged_in}u {timestamp}\nmessages_this_instance value={messages_this_instance}u {timestamp}\nmessages_new value={messages_new}u {timestamp}\n",
                logged_in = clients,
                messages_this_instance = messages,
                messages_new = diff,
                timestamp = timestamp,
            );

            debug!("line_format: {}", line_format);

            let res = client.post(url.clone())
                .header("Authorization", format!("Token {}", influx_token))
                .body(line_format)
                .send()
                .await
                .and_then(|resp| resp.error_for_status());

            if let Err(e) = res {
                error!("failed to send to influxdb: {}", e);
            } else {
                debug!("sent to influxdb");
            }

            tokio::time::sleep(Duration::from_secs(60)).await;
        }
    });
}
