CREATE TABLE products (
    product_id   INT PRIMARY KEY,
    name         VARCHAR(100) NOT NULL,
    price        NUMERIC(18, 2),
    stock_count  SMALLINT, -- checking int -> smallint mapping
    last_updated TIMESTAMP
);
