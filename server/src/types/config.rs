use serde::{Deserialize, Serialize};

#[derive(Debug, Deserialize, Serialize)]
pub struct Config {
    pub server: Server,
    pub database: Database,
}

#[derive(Debug, Deserialize, Serialize)]
pub struct Server {
    pub address: String,
}

#[derive(Debug, Deserialize, Serialize)]
pub struct Database {
    pub path: String,
}
