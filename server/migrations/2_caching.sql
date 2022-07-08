-- add a column so we can cache login lodestone requests
alter table users
    add column last_updated timestamp not null default 0;
