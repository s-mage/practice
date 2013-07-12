CREATE TABLE users (
  id serial primary key,
  name varchar(40) not null,
  password varchar(20) not null
);

CREATE TYPE state as enum ('nothing', 'data', 'report');

CREATE TABLE sites (
  id serial primary key,
  url varchar(250) unique,
  ready state default 'nothing'
);


CREATE TABLE reports (
  id serial primary key,
  site_id integer references sites(id) on update cascade,
  creation_time timestamp,
  path varchar(250)
);

CREATE TABLE subpages (
  id serial primary key,
  url varchar(250),
  rules json,
  report_id integer references reports(id)
);

CREATE TABLE rules (
  id serial primary key,
  name varchar(120) not null,
  common boolean default 'true',
  message varchar(1500) not null
);

CREATE TABLE user_site (
  user_id integer references users(id) on update cascade,
  site_id integer references sites(id) on update cascade,
  unique (user_id, site_id)
);