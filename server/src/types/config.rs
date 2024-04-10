use std::path::PathBuf;

use serde::{Deserialize, Serialize};
use url::Url;

#[derive(Debug, Deserialize, Serialize)]
pub struct Config {
    pub server: Server,
    pub database: Database,
    #[serde(default)]
    pub influx: Option<Influx>,
}

#[derive(Debug, Deserialize, Serialize)]
pub struct Server {
    pub path: PathBuf,
}

#[derive(Debug, Deserialize, Serialize)]
pub struct Database {
    pub path: String,
}

#[derive(Debug, Deserialize, Serialize)]
pub struct Influx {
    pub url: Url,
    pub org: String,
    pub bucket: String,
    pub token: String,
}
