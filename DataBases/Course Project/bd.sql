DROP TABLE IF EXISTS auth.users CASCADE;
DROP TYPE IF EXISTS auth.user_type CASCADE;
DROP SCHEMA IF EXISTS auth CASCADE;
DROP TABLE IF EXISTS specification CASCADE;
DROP TABLE IF EXISTS sales_markets CASCADE;
DROP TABLE IF EXISTS companies CASCADE;
DROP TABLE IF EXISTS models_range CASCADE;
DROP TABLE IF EXISTS models_sales_markets_relation CASCADE;
DROP TABLE IF EXISTS model_body_type_relation CASCADE;
DROP TABLE IF EXISTS body_type CASCADE;
DROP TYPE IF EXISTS engine_type CASCADE;
DROP FUNCTION IF EXISTS set_model_body_type_relation_function CASCADE;

CREATE SCHEMA auth;

CREATE TYPE auth.user_type AS ENUM (
	'admin',
	'user'
);

CREATE TABLE auth.users (
	id bigserial primary key,
	login varchar(55),
	password varchar(100),
	category auth.user_type,
    UNIQUE (login)
);

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
    creation_date date NOT NULL,
    count_of_workers int,
	logo varchar(255)
);

CREATE TABLE models_range (
	id bigserial primary key,
	company_id bigint NOT NULL,
	model_name varchar(55) NOT NULL,
	FOREIGN KEY(company_id) REFERENCES companies
	ON DELETE CASCADE
	ON UPDATE CASCADE
);

CREATE TABLE specification (
    id bigserial primary key,
    model_id bigint NOT NULL,
	generation varchar(55),
    start_of_production date,
    end_of_production date,
    engine engine_type,
    engine_displacement int,
    HP int,
    body_type bigint NOT NULL,
	FOREIGN KEY (body_type) REFERENCES body_type
	ON DELETE SET NULL
	ON UPDATE CASCADE,
	FOREIGN KEY (model_id) REFERENCES models_range
	ON DELETE CASCADE
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
	FOREIGN KEY (model_id) REFERENCES models_range
	ON DELETE CASCADE
	ON UPDATE CASCADE,
	FOREIGN KEY (sales_market_id) REFERENCES sales_markets
	ON DELETE CASCADE
	ON UPDATE CASCADE
);

CREATE TABLE model_body_type_relation (
	model_id bigint NOT NULL, 
	body_type_id bigint NOT NULL,
	FOREIGN KEY (model_id) REFERENCES models_range
	ON DELETE CASCADE
	ON UPDATE CASCADE,
	FOREIGN KEY (body_type_id) REFERENCES body_type
	ON DELETE CASCADE
	ON UPDATE CASCADE
);

CREATE FUNCTION set_model_body_type_relation_function()
RETURNS TRIGGER AS 
$$
BEGIN
    INSERT INTO model_body_type_relation (model_id, body_type_id) VALUES (NEW.model_id, NEW.body_type);
	RETURN NEW;
END
$$
LANGUAGE plpgsql;

CREATE TRIGGER model_body_type_relation_insert
AFTER INSERT ON specification
FOR EACH ROW
EXECUTE FUNCTION set_model_body_type_relation_function();

CREATE UNIQUE INDEX IF NOT EXISTS uix_companies_name ON companies (name);
CREATE UNIQUE INDEX IF NOT EXISTS uix_body_type_name ON body_type (name);
CREATE UNIQUE INDEX IF NOT EXISTS uix_model_name ON models_range (model_name);
CREATE UNIQUE INDEX IF NOT EXISTS uix_sales_market_market_zone ON sales_markets (market_zone);
CREATE UNIQUE INDEX IF NOT EXISTS uix_models_sales_markets_relation ON models_sales_markets_relation (model_id, sales_market_id);

INSERT INTO auth.users (login, password, category) VALUES
('123', '123', 'admin')

INSERT
INTO sales_markets (market_zone) VALUES 
('Europe'),
('Russia'),
('Asia'),
('China'),
('USA'),
('South America'),
('Africa'),
('India');

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
('Campervan'),
('Quad bike');


INSERT
INTO companies (name, office, creation_date, count_of_workers, logo) VALUES
('Acura', 'Torrance', '27.03.1986', 23805, 'Logos/AcuraPrimary_PCP_BLK_NEW_5-11.jpg'),
('Alfa Romeo', 'Turin', '24.06.1910', 3000, 'Logos/png-transparent-alfa-romeo-logo-alfa-romeo-156-car-logo-fiat-alfa-romeo-logo-text-trademark-automobile-repair-shop.png'),
('Aston Martin', 'Warwick', '15.01.1913', 2473, 'Logos/Aston-Martin-Logo-2048x1152.jpg'),
('Audi', 'Ingolstadt', '16.07.1909', 87000, 'Logos/1636096812_62-papik-pro-p-audi-logotip-foto-62-scaled.jpg'),
('Bentley', 'Crewe', '18.01.1919', 3600, 'Logos/1636468838_5-hdpic-club-p-logotip-bentli-5.png'),
('BMW', 'Munich', '07.03.1916', 118909, 'Logos/gratis-png-vehiculo-de-lujo-con-logotipo-de-bmw-logotipo-de-bmw-logotipo-de-bmw.png'),
('Bugatti', 'Molsheim', '01.01.1909', 1100, 'Logos/bugatti-logo-2560x1440.png'),
('BYD', 'Shaanxi Province', '10.02.1995', 288200, 'Logos/img_60426659aba7a-2048x1152.png'),
('Cadillac', 'Detroit', '22.08.1902', 11000, 'Logos/1580283058_2-p-logotipi-kadillaka-2.png'),
('Chevrolet', 'Detroit', '03.11.1911', 20000, 'Logos/chevrolet_logo_e-motors_ru.jpg'),
('Crysler', 'Auburn Hills', '06.06.1925', 56900, 'Logos/Chrysler-Logo.png'),
('Citroen', 'Poissy', '04.06.1919', 197000, 'Logos/citroen-logo-black.jpg'),
('Dacia', 'Mioveni', '01.09.1966', 12209, 'Logos/Dacia-sign-2015-2021.png'),
('Dodge', 'Auburn Hills', '14.12.1900', 235000, 'Logos/Dodge-Silver-Logo.png'),
('Ferrari', 'Maranello', '13.09.1939', 4571, 'Logos/ahmx-jimi-9.jpg'),
('Fiat', 'Turin', '11.07.1899', 214836, 'Logos/fe809947d6871b86a35a01477a776535.jpg'),
('Ford', 'Las Vegas', '16.06.1903', 183000, 'Logos/anz47dko9hu2lkv7my3nv5omslsjaatj.jpg'),
('Haval', 'Baoding', '29.03.2013', 25000, 'Logos/Haval-Logo.png'),
('Honda', 'Tokyo', '24.09.1948', 197039, 'Logos/honda.png'),
('Hummer', 'Detroit', '22.01.1992', 1000, 'Logos/Hummer-Logo-1536x864.png'),
('Infiniti', 'Yokohama', '08.11.1989', 8000, 'Logos/infiniti_PNG18.png'),
('Jeep', 'Auburn Hills', '01.01.1941', 7500, 'Logos/Jeep-Logo-PNG-File.png'),
('KIA', 'Seoul', '21.12.1944', 51975, 'Logos/Kia-Simbolo.png'),
('Lada', 'Tolyatti', '20.07.1966', 32500, 'Logos/lada-logo_01.png'),
('Lamborghini', 'Sant''Agata Bolognese', '30.10.1963', 1779, 'Logos/36e91f7d76c22d6cd013e099a3faef1f.png'),
('Lexus', 'Nagoya', '01.09.1989', 70000, 'Logos/lexus.png'),
('Lotus', 'Hethel', '01.01.1952', 1385, 'Logos/lotus-logo-3000x3000.png'),
('Maserati', 'Modena', '01.12.1914', 1100, 'Logos/debc2e38ed.png'),
('Mazda', 'Hiroshima', '30.01.1920', 49786, 'Logos/1688989344_myskillsconnect-com-p-logotip-mazda-foto-12.png'),
('Mercedes-Benz', 'Stuttgart', '28.06.1926', 145436, 'Logos/mb.png'),
('Mini', 'Farnborough', '01.04.1952', 14000, 'Logos/844-8446733_datei-mini-logo-svg-mini-logo-black-and.png'),
('Pagani', 'San Cesario sul Panaro', '01.01.1992', 162, 'Logos/419-4194037_pagani-logo-zeichen-vektor-bedeutendes-und-geschichtezeichen-pagani.png'),
('Porsche', 'Stuttgart', '06.03.1931', 36359, 'Logos/porsche-1931-present-e1650385475287.png'),
('Rolls-Royce', 'Goodwood', '01.03.1998', 1300, 'Logos/Rolls-Royce-1906-Presente.jpg'),
('Skoda', 'Mladá Boleslav', '17.12.1895', 36032, 'Logos/288-2887790_skoda-logo-png-transparent-png.png'),
('Subaru', 'Shibuya', '15.07.1953', 16961, 'Logos/216cf0e9-4ade-437a-a405-8cbb9c57a9b5-4865487.jpeg'),
('Tesla', 'Austin', '01.07.2003', 127855, 'Logos/tesla-big.png'),
('Toyota', 'Koromo', '28.08.1937', 366283, 'Logos/Toyota-Motor-Logo-Transparent-Images.png'),
('Volkswagen', 'Wolfsburg', '28.05.1937', 670000, 'Logos/logonew.jpg');

INSERT INTO models_range(company_id, model_name)
SELECT id, UNNEST (ARRAY['CSX', 'Integra', 'MDX', 'NSX']) from companies where name = 'Acura';

INSERT INTO models_range(company_id, model_name)
SELECT id, UNNEST (ARRAY['Bulldog', 'DB11', 'DB12', 'DBS', 'One-77']) from companies where name = 'Aston Martin';

INSERT INTO models_range(company_id, model_name)
SELECT id, UNNEST (ARRAY['80', '100', '200', 'A4', 'A6', 'A7', 'A8', 'Q8', 'R8', 'RS6']) from companies where name = 'Audi';

INSERT INTO models_range(company_id, model_name)
SELECT id, UNNEST (ARRAY['Series 1', 'Series 3', 'Series 5', 'Series 7', 'i8', 'Isetta', 'M6', 'M8', 'X5', 'Z4']) 
from companies where name = 'BMW';

INSERT INTO models_range(company_id, model_name)
SELECT id, UNNEST (ARRAY['300 SLR', 'AMG GT', 'C Class', 'CLA', 'E Class', 'EQB', 'G Class', 'GLS', 'S Class', 'W124']) 
from companies where name = 'Mercedes-Benz';

INSERT INTO models_range(company_id, model_name)
SELECT id, UNNEST (ARRAY['356', '718', '911', '918', '959', 'Boxster', 'Cayenne', 'Cayman', 'Macan', 'Taycan']) 
from companies where name = 'Porsche';

INSERT INTO models_range(company_id, model_name)
SELECT id, UNNEST (ARRAY['Amarok', 'Arteon', 'Beetle', 'Bora', 'Caddy', 'Golf', 'ID.7', 'Jetta', 'Passat', 'Touareg']) 
from companies where name = 'Volkswagen';

INSERT INTO 
specification(model_id, generation, start_of_production, end_of_production, engine, engine_displacement, HP, body_type)
WITH model AS (SELECT id from models_range WHERE model_name = 'NSX')
SELECT model.id, 'NSX II', '01.01.2022', NULL, 'hybrid', 3493, 520, 2 from model;

INSERT INTO 
specification(model_id, generation, start_of_production, end_of_production, engine, engine_displacement, HP, body_type)
WITH model AS (SELECT id from models_range WHERE model_name = 'DBS')
SELECT model.id, 'DBS Superleggera', '01.01.2018', NULL, 'internal combustion engine', 5204, 725, 2 from model;

INSERT INTO 
specification(model_id, generation, start_of_production, end_of_production, engine, engine_displacement, HP, body_type)
WITH model AS (SELECT id from models_range WHERE model_name = 'DBS')
SELECT model.id, 'DBS Superleggera', '01.01.2023', NULL, 'internal combustion engine', 5204, 770, 2 from model;

INSERT INTO 
specification(model_id, generation, start_of_production, end_of_production, engine, engine_displacement, HP, body_type)
WITH model AS (SELECT id from models_range WHERE model_name = '200')
SELECT model.id, '200 (C3, Typ 44,44Q)', '01.01.1989', '01.01.1991', 'internal combustion engine', 2393, 136, 3 from model;

INSERT INTO 
specification(model_id, generation, start_of_production, end_of_production, engine, engine_displacement, HP, body_type)
WITH model AS (SELECT id from models_range WHERE model_name = 'A8')
SELECT model.id, 'A8 (D4,4H facelift 2013)', '01.01.2013', '01.01.2017', 'internal combustion engine', 4134, 385, 3 from model;

INSERT INTO 
specification(model_id, generation, start_of_production, end_of_production, engine, engine_displacement, HP, body_type)
WITH model AS (SELECT id from models_range WHERE model_name = 'R8')
SELECT model.id, 'R8 II Spyder (4S)', '01.01.2017', '01.01.2018', 'internal combustion engine', 5204, 610, 6 from model;

INSERT INTO 
specification(model_id, generation, start_of_production, end_of_production, engine, engine_displacement, HP, body_type)
WITH model AS (SELECT id from models_range WHERE model_name = 'Isetta')
SELECT model.id, '300', '01.01.1955', '01.01.1962', 'internal combustion engine', 295, 13, 15 from model;

INSERT INTO 
specification(model_id, generation, start_of_production, end_of_production, engine, engine_displacement, HP, body_type)
WITH model AS (SELECT id from models_range WHERE model_name = 'S Class')
SELECT model.id, '300', '01.01.1955', '01.01.1962', 'internal combustion engine', 295, 13, 15 from model;