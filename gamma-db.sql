
drop database gamma_store
create database gamma_store
use gamma_store
go

create table users (   
    username nvarchar(80) primary key not null,
    password nvarchar(80) not null
);

create table session (    
    name nvarchar(24) primary key not null,    
    ip_address nvarchar(80) not null,
	comment nvarchar(512) default '',
    livetime float default 0,
    detector_data text default ''
);

create table spectrum (    	
    session_name nvarchar(24) not null,
    session_index int not null,
    start_time datetime default null,
    latitude float default 0,
    longitude float default 0,
    altitude float default 0,
    track float default 0,
    speed float default 0,
    climb float default 0,
    livetime float default 0,
    realtime float default 0,
    num_channels int default 0,
    channels text default '',
    doserate float default 0
);
