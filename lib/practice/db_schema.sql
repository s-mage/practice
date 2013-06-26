CREATE TABLE users (
  id integer primary key,
  name varchar(40) not null,
  password varchar(220) not null
);

CREATE TABLE sites (
  url char(1500) primary key,
  rules integer[],
  report bytea,
  ready boolean default 'false'
);

CREATE TABLE rules (
  id integer primary key,
  name varchar(120) not null,
  message varchar(1500) not null
);

CREATE TABLE user_site (
  user_id integer references users(id) on update cascade,
  site_url char(1500) references sites(url) on update cascade
);
