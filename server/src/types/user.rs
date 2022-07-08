use lodestone_scraper::lodestone_parser::ffxiv_types::World;

#[derive(Debug, Clone)]
pub struct User {
    pub lodestone_id: u64,
    pub name: String,
    pub world: World,
    pub hash: String,
}
