
-- DDL

CREATE TABLE products (
	id BIGINT NOT NULL PRIMARY KEY IDENTITY,
	name VARCHAR(50) NOT NULL
);

CREATE TABLE categories (
	id BIGINT NOT NULL PRIMARY KEY IDENTITY,
	name VARCHAR(50) NOT NULL
);

CREATE TABLE products_to_categories (
	id BIGINT NOT NULL PRIMARY KEY IDENTITY,
	products_id BIGINT NOT NULL,
	categories_id BIGINT NOT NULL
);

-- DML

WITH seq (lev) AS (
	SELECT 1
	UNION ALL
	SELECT lev + 1 FROM seq WHERE lev < 4)
INSERT INTO products (name)
	SELECT CONCAT('product', lev) FROM seq;

SELECT * FROM products;


WITH seq (lev) AS (
	SELECT 1
	UNION ALL
	SELECT lev + 1 FROM seq WHERE lev < 6)
INSERT INTO categories (name)
	SELECT CONCAT('category', lev) FROM seq;

SELECT * FROM categories;

INSERT INTO products_to_categories VALUES
(1, 1),
(1, 2),
(1, 3),
(2, 3);

TRUNCATE TABLE products_to_categories;
SELECT * FROM products_to_categories;

-- Решение

SELECT p.name AS product, c.name AS category
FROM products p
	LEFT JOIN products_to_categories ptc ON ptc.products_id = p.id
	LEFT JOIN categories c ON c.id = ptc.categories_id;