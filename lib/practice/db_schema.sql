CREATE TABLE user (
  id integer primary key,
  name varchar(40) not null,
  password varchar(220) not null
);

CREATE TABLE site (
  url char(1500) primary key,
  rules integer[]
);

CREATE TABLE rules (
  id integer primary key,
  name varchar(120) not null,
  message varchar(1500) not null
);

CREATE TABLE user_site (
  id integer primary key,
  user_id integer references user(id) on update cascade,
  site_url char(1500) references site(url) on update cascade
);
