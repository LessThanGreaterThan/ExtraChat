create table users
(
    lodestone_id unsigned bigint not null primary key,
    name         text            not null,
    world        text            not null,
    key_short    text            not null,
    key_hash     text            not null
);

create table verifications
(
    lodestone_id unsigned bigint not null primary key,
    challenge    text            not null,
    created_at   timestamp       not null default current_timestamp
);

create table channels
(
    id   text not null primary key,
    name blob not null
);

create table user_channels
(
    lodestone_id unsigned bigint not null references users (lodestone_id) on delete cascade,
    channel_id   text            not null references channels (id) on delete cascade,
    rank         tinyint         not null,

    primary key (lodestone_id, channel_id)
);

create index user_channels_lodestone_id_idx on user_channels (lodestone_id);
create index user_channels_channel_id_idx on user_channels (channel_id);

create table channel_invites
(
    channel_id text            not null references channels (id) on delete cascade,
    invited    unsigned bigint not null references users (lodestone_id) on delete cascade,
    inviter    unsigned bigint not null references users (lodestone_id) on delete cascade,

    primary key (channel_id, invited)
);

create index channel_invites_channel_id_idx on channel_invites (channel_id);
create index channel_invites_channel_id_invited_idx on channel_invites (channel_id, invited);
