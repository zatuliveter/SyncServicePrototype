CREATE TABLE dbo.products (
    product_id   INT PRIMARY KEY,
    name         VARCHAR(100) NOT NULL,
    price        NUMERIC(18, 2),
    stock_count  SMALLINT, -- checking int -> smallint mapping
    last_updated datetime2(7)
);
go

create table dbo.SyncState
(
	name varchar(128) not null,
	value nvarchar(max) not null,
	updated_utc datetime2(3) not null,
	constraint pk_SyncState primary key clustered (name)
);