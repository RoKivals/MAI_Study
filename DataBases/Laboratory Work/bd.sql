DROP TABLE IF EXISTS models;
DROP TABLE IF EXISTS sales_markets;
DROP TABLE IF EXISTS companies;
DROP TABLE IF EXISTS models_range;
DROP TABLE IF EXISTS models_sales_markets_relation;
DROP TABLE IF EXISTS body_type;
DROP TYPE IF EXISTS engine_type;

CREATE TYPE engine_type AS ENUM (
	'hybrid',
	'electric',
	'internal combustion engine'
);

CREATE TABLE body_type (
	id bigserial primary key,
	name varchar(255) NOT NULL
);

CREATE TABLE companies(
    id bigserial primary key,
    name varchar(255) NOT NULL,
    office varchar(255) NOT NULL,
    factory varchar(255),
    creation_date date NOT NULL,
    count_of_workers int
);

CREATE TABLE models (
    id bigserial primary key,
	company_id bigint NOT NULL,
    name varchar(255) NOT NULL,
    release_date date,
    engine engine_type,
    engine_capacity int,
    HP int,
	cost int,
    body_type bigint NOT NULL,
	FOREIGN KEY (body_type) REFERENCES body_type
	ON DELETE SET NULL
	ON UPDATE CASCADE,
	FOREIGN KEY (company_id) REFERENCES companies
	ON DELETE SET NULL
	ON UPDATE CASCADE
);

CREATE TABLE sales_markets (
    id bigserial primary key,
    market_zone varchar(255) NOT NULL
);

CREATE TABLE models_sales_markets_relation (
	id bigserial primary key,
	model_id bigint NOT NULL, 
	sales_market_id bigint NOT NULL,
	FOREIGN KEY (model_id) REFERENCES models
	ON DELETE CASCADE
	ON UPDATE CASCADE,
	FOREIGN KEY (sales_market_id) REFERENCES sales_markets
	ON DELETE CASCADE
	ON UPDATE CASCADE
);


CREATE UNIQUE INDEX IF NOT EXISTS uix_companies_name ON companies (name);
CREATE UNIQUE INDEX IF NOT EXISTS uix_body_type_name ON body_type (name);
CREATE UNIQUE INDEX IF NOT EXISTS uix_sales_market_market_zone ON sales_markets (market_zone);
CREATE UNIQUE INDEX IF NOT EXISTS uix_models_sales_markets_relation ON models_sales_markets_relation (model_id, sales_market_id);

INSERT
INTO sales_markets (market_zone) VALUES 
('Europe'),
('Russia'),
('Asia'),
('China'),
('USA'),
('South America'),
('Africa');

INSERT
INTO body_type (name) VALUES 
('Hatchback'),
('Coupe'),
('Sedan'),
('Station wagon'),
('Kammback'),
('Cabriolet'),
('Roadster'),
('Targa'),
('CrossOver'),
('Jeep'),
('Pickup'),
('Limousine'),
('Minivan'),
('Campervan');

INSERT
INTO companies (name, office, factory, creation_date, count_of_workers) VALUES 
('Volkswagen', 'Wolfsburg', 'Wolfsburg', '28.05.1937', 670000),
('Audi', 'Ingolstadt', 'Ingolstadt', '16.07.1909', 87000),
('Porsche', 'Stuttgart', 'Zuffenhausen', '06.03.1931', 36359),
('Ford', 'Las Vegas', 'Tehas', '16.06.1903', 183000),
('BMW', 'Munich', 'Munich', '07.03.1916', 118909),
('Mercedes-Benz', 'Stuttgart', 'Sindelfingen', '28.06.1926', 145436),
('Mazda', 'Hiroshima', 'Hiroshima', '30.01.1920', 49786),
('Maserati', 'Modena', 'Modena', '01.12.1914', 1100);


INSERT
INTO models (company_id, name, release_date, body_type) VALUES 
(1, 'Golf', '01.01.1974', 1),
(1, 'ID.4', '01.01.2020', 9),
(2, 'A1', '01.01.2010', 1),
(2, 'A2', '01.01.1999', 1),
(2, 'A4', '01.01.1994', 3),
(2, 'A5', '01.06.2007', 2),
(2, 'A6', '01.01.1994', 3),
(2, 'A7', '01.01.2010', 5),
(2, 'A8', '01.01.1994', 3),
(3, 'Cayenne', '01.12.2002', 9),
(3, '911', '01.01.1965', 8),
(4, 'Focus', '01.01.1998', 1),
(4, 'F-150', '01.01.1979', 11),
(5, '3 Series', '01.01.1975', 4),
(5, '7 Series', '01.01.1977', 3),
(6, 'E-Class', '01.01.1993', 3),
(6, 'A-Class', '01.01.1997', 1),
(7, 'MX-5', '01.01.1989', 7),
(7, 'RX-7', '01.01.1978', 2),
(8, 'Ghibli', '01.01.1967', 3);

INSERT 
INTO models_sales_markets_relation (model_id, sales_market_id) VALUES
(1, 1),
(1, 2),
(1, 3),
(1, 4),
(1, 5),
(1, 6),
(2, 1),
(2, 2),
(2, 4),
(2, 5),
(3, 1),
(3, 2),
(3, 4),
(4, 1),
(4, 3),
(5, 1),
(5, 4),
(6, 1),
(6, 2),
(6, 4),
(6, 5),
(7, 1),
(7, 2),
(7, 3),
(7, 4),
(7, 5),
(7, 7),
(8, 1),
(8, 2),
(8, 3),
(8, 4),
(8, 5),
(8, 7),
(9, 1),
(9, 2),
(9, 5),
(10, 1),
(10, 2),
(10, 4),
(10, 5),
(11, 1),
(11, 2),
(11, 4),
(11, 5),
(12, 1),
(12, 2),
(12, 5),
(12, 6),
(12, 7),
(13, 5),
(13, 6),
(14, 1),
(14, 2),
(14, 4),
(14, 5),
(15, 1),
(15, 4),
(15, 5),
(16, 1),
(16, 2),
(16, 5),
(17, 1),
(17, 2),
(18, 1),
(18, 3),
(18, 4),
(18, 5),
(18, 6),
(18, 7),
(19, 3),
(19, 4),
(20, 1),
(20, 2),
(20, 5);
