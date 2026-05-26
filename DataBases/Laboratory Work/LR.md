# Описание базы данных "Автомобильный журнал"

## Схема базы данных
[![image.png](https://i.postimg.cc/9FyFyVwf/image.png)](https://postimg.cc/qzMTdPLf)


## DDL SQL создание БД + заполнение данными:
```sql
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

```

# Запросы к базе данных "Автомобильный журнал"

## Реляционные и булевы операторы

Запрос 1:
- Назначение: Узнать компании, в которых количество работников меньше 100К человек
- Запрос:
```sql
SELECT * from companies where count_of_workers < 100000
```
- Результат:
<div style="text-align: center;">
  
[![image13.png](https://i.postimg.cc/NGVsrFNT/image13.png)](https://postimg.cc/JtNVfrC4)
</div>

Запрос 2:
- Назначение: Узнать компании, в которых количество работников больше 100К человек и они были образованы после конца первой мировой войны
- Запрос:
```sql
SELECT * from companies where count_of_workers > 100000 AND creation_date > '11.11.1918'
```
- Результат:
<div style="text-align: center;">
  
[![image26.png](https://i.postimg.cc/nrx6v8Qy/image26.png)](https://postimg.cc/0MtWsBnn)
</div>

Запрос 3:
- Назначение: Узнать компании, в которых количество работников не меньше 50К человек, а также головной офис и завод компании находятся в одном городе
- Запрос:
```sql
SELECT * from companies where NOT count_of_workers < 50000 AND office = factory
```
- Результат:
<div style="text-align: center;">
  
  [![image22.png](https://i.postimg.cc/c4Ssc2S4/image22.png)](https://postimg.cc/w3w8qWMS)
</div>

## Операторы `IN` `BETWEEN` `LIKE`

Запрос 1:
- Назначение: Узнать компании, головной офис, которых находится в одном из перечисленных городов
- Запрос:
```sql
SELECT * from companies where office IN ('Wolfsburg', 'Las Vegas', 'Moscow')
```
- Результат:
<div style="text-align: center;">
  
  [![image7.png](https://i.postimg.cc/LsdgkvNt/image7.png)](https://postimg.cc/dhB0wRM3)
</div>

Запрос 2:
- Назначение: Узнать компании, в которых количество работников находится в диапазоне между двух значений
- Запрос:
```sql
SELECT * from companies where count_of_workers BETWEEN 80000 AND 500000
```
- Результат:
<div style="text-align: center;">
  
  [![image1.png](https://i.postimg.cc/wjrdyGxV/image1.png)](https://postimg.cc/Wd0yxSkF) 
</div>

Запрос 3:
- Назначение: Узнать модели автомобилей, название которых начинается с буквы A
- Запрос:
```sql
SELECT * from models where name LIKE 'A%'
```
- Результат:
<div style="text-align: center;">
  
  [![image17.png](https://i.postimg.cc/fL1bZRpx/image17.png)](https://postimg.cc/KkrbDxZz)
 </div>

## Агрегирующие функции

Запрос 1:
- Назначение: Посчитать количество моделей Audi начинающихся с буквы A
- Запрос:
```sql
SELECT count(id) from models
where name LIKE 'A%'
AND models.company_id = (select id from companies where name = 'Audi')
```
- Результат:
<div style="text-align: center;">
  
  [![image28.png](https://i.postimg.cc/kgRfX12K/image28.png)](https://postimg.cc/Yvk1dxfC)
</div>

Запрос 2:
- Назначение: Узнать среднее количество работников среди компаний с головным офисов в определённых городах
- Запрос:
```sql
SELECT CEIL(AVG(count_of_workers)) from companies 
where office IN ('Munich', 'Hiroshima', 'Zuffenhausen')
```
- Результат:
<div style="text-align: center;">
  
  [![image4.png](https://i.postimg.cc/pd1D96dP/image.png)](https://postimg.cc/KKBKwQLW)
</div>

Запрос 3:
- Назначение: Узнать название компании с минимальным количество работников
- Запрос:
```sql
SELECT name from companies 
where count_of_workers = (select MIN(count_of_workers) from companies)
```
- Результат:
<div style="text-align: center;">
  
  [![image10.png](https://i.postimg.cc/3JdkPzqj/image10.png)](https://postimg.cc/Xp6N514X)
</div>

## Форматирование результата

Запрос 1:
- Назначение: Вывести названия моделей вместе с названием их бренда
- Запрос:
```sql
SELECT FORMAT('%s %s', companies.name, models.name) from models
JOIN companies ON models.company_id = companies.id
```
- Результат:
<div style="text-align: center;">
  
  [![image27.png](https://i.postimg.cc/kGyLsjQB/image27.png)](https://postimg.cc/CnzvLs6g)
</div>

Запрос 2:
- Назначение: Вывести название компании и год её основания (только год)
- Запрос:
```sql
SELECT name as company, extract(year from creation_date) AS foundation_year from companies
```
- Результат:
<div style="text-align: center;">
  
  [![image25.png](https://i.postimg.cc/RV4Btt7W/image25.png)](https://postimg.cc/w3fZ87bz)
</div>

Запрос 3:
- Назначение: Вывести название компании и дату её основания в формате `ГГГГ-Мес`
- Запрос:
```sql
SELECT name as company, 
TO_CHAR(creation_date, 'YYYY-Mon') AS foundation_date from companies
```
- Результат:
<div style="text-align: center;">
  
  [![image11.png](https://i.postimg.cc/bJmv5RHF/image11.png)](https://postimg.cc/njQZjBWv)
</div>

## Несколько таблиц в запросе
     
Запрос 1:
- Назначение: Вывести названия моделей и названия рынков, на которых они продаются
- Запрос:
```sql
SELECT models.name, sales_markets.market_zone 
from models, sales_markets, models_sales_markets_relation
WHERE (models.id = models_sales_markets_relation.model_id
      AND sales_markets.id = models_sales_markets_relation.sales_market_id)
```
- Результат:
<div style="text-align: center;">
  
  [![image35.png](https://i.postimg.cc/Zn06sn3p/image35.png)](https://postimg.cc/kR3VBnTg)
  [![image38.png](https://i.postimg.cc/N04TgmkR/image38.png)](https://postimg.cc/nsXM2s5L)
</div>

Запрос 2:
- Назначение: Вывести названия моделей вместе с названием их бренда
- Запрос:
```sql
SELECT companies.name, models.name from models, companies
WHERE companies.id = models.company_id
```
- Результат:
<div style="text-align: center;">
  
  [![image9.png](https://i.postimg.cc/QCMFNzd3/image9.png)](https://postimg.cc/cgVxXX39)
</div>

Запрос 3:
- Назначение: Вывести название компании и год релиза её самой ранней модели (из БД)
- Запрос:
```sql
SELECT companies.name, MIN(extract(year from models.release_date))
from companies, models
where companies.id = models.company_id
GROUP BY companies.name
```
- Результат:
<div style="text-align: center;">
  
  [![image12.png](https://i.postimg.cc/W4npD7Kb/image12.png)](https://postimg.cc/jWDYmytG)
</div>

## Вложенные запросы

Запрос 1:
- Назначение: Узнать название компании с минимальным количество работников
- Запрос:
```sql
SELECT name from companies 
where count_of_workers = (select MIN(count_of_workers) from companies)
```
- Результат:
<div style="text-align: center;">
  
  [![image29.png](https://i.postimg.cc/xdK5LXyd/image29.png)](https://postimg.cc/f3RYNTP4)
</div>

Запрос 2:
- Назначение: Вывести названия компаний и кол-во работников, где их количество больше среднего значения работников среди всех компаний
- Запрос:
```sql
SELECT name, count_of_workers from companies
where count_of_workers > (select AVG(count_of_workers) from companies)
```
- Результат:
<div style="text-align: center;">
  
  [![image34.png](https://i.postimg.cc/FKgb72Nd/image34.png)](https://postimg.cc/SXRYP1Xk)
</div>

Запрос 3:
- Назначение: Вывести название моделей, которые были представлены спустя 70 лет после основания компании производителя
- Запрос:
```sql
SELECT models.name from models
where models.release_date - (SELECT creation_date from companies
                             where companies.id = models.company_id) > 364 * 70
```
- Результат:
<div style="text-align: center;">
  
  [![image16.png](https://i.postimg.cc/VNG6GRFZ/image16.png)](https://postimg.cc/bGnpd1W1)
</div>

## Связанные подзапросы

Запрос 1:
- Назначение: Вывести компании и рынки сбыта, в которых у этой компании представлено наибольшее количество моделей
- Запрос:
```sql
WITH companies_markets AS 
(SELECT companies.name, market_zone, COUNT(market_zone) as market_models 
from companies JOIN models
ON models.company_id = companies.id
JOIN models_sales_markets_relation
ON models.id = models_sales_markets_relation.model_id
JOIN sales_markets
ON sales_markets.id = models_sales_markets_relation.sales_market_id
GROUP BY companies.name, market_zone
ORDER BY companies.name)


SELECT name, market_zone from companies_markets CM1 where
market_models = (SELECT MAX(market_models) from companies_markets CM2 
WHERE CM1.name = CM2.name)
```
- Результат:
<div style="text-align: center;">

  [![image6.png](https://i.postimg.cc/m2D9TPPH/image6.png)](https://postimg.cc/vDJTzHLY)
</div>

## Оператор JOIN

Запрос 1:
- Назначение: Полная информация о модели и компании, которой она принадлежит
- Запрос:
```sql
SELECT * from companies JOIN models ON models.company_id = companies.id
```
- Результат:
<div style="text-align: center;">
  
  [![image15.png](https://i.postimg.cc/ht0hxgQB/image15.png)](https://postimg.cc/5X6xdhm7)
</div>

Запрос 2:
- Назначение: Вывод названия моделей и их рынков сбыта
- Запрос:
```sql
SELECT models.name, market_zone from models JOIN models_sales_markets_relation
ON models.id = models_sales_markets_relation.model_id
JOIN sales_markets ON sales_markets.id = models_sales_markets_relation.sales_market_id
```
- Результат:
<div style="text-align: center;">
  
  [![image31.png](https://i.postimg.cc/yxry7cBL/image31.png)](https://postimg.cc/5QLC3HXz)
  [![image19.png](https://i.postimg.cc/02RNBrpv/image19.png)](https://postimg.cc/9zbV4m8g)
</div>

Запрос 3:
- Назначение: Вывести компании и кол-во представленных ими моделей на каждом из рынков сбыта
- Запрос:
```sql
WITH companies_markets AS 
(SELECT companies.name, market_zone, COUNT(market_zone) as market_models 
from companies JOIN models
ON models.company_id = companies.id
JOIN models_sales_markets_relation
ON models.id = models_sales_markets_relation.model_id
JOIN sales_markets
ON sales_markets.id = models_sales_markets_relation.sales_market_id
GROUP BY companies.name, market_zone
ORDER BY companies.name)


SELECT * from companies_markets
```
- Результат:
<div style="text-align: center;">
  
  [![image8.png](https://i.postimg.cc/KzxKwjX4/image8.png)](https://postimg.cc/bGVN2zwj)
</div>

## Операторы `EXIST` `ANY` `ALL` `SOME`

Запрос 1:
- Назначение: Полная информация о моделях и рынках сбыта, если среди моделей есть названия начинающиеся с A
- Запрос:
```sql
WITH models_markets AS (SELECT * from models
JOIN models_sales_markets_relation
ON models.id = models_sales_markets_relation.model_id)


SELECT * from models_markets
where EXISTS(select *
              from models_markets
            where name LIKE 'A%')
```
- Результат:
<div style="text-align: center;">
  
  [![image2.png](https://i.postimg.cc/c4rWxW0H/image2.png)](https://postimg.cc/SJF3dwXF)
</div>

Запрос 2:
- Назначение: Вывод информации о моделях, продаваемых на азиатском рынке (включая Китай)
- Запрос:
```sql
WITH models_markets AS (SELECT * from models
JOIN models_sales_markets_relation
ON models.id = models_sales_markets_relation.model_id)


SELECT * from models_markets
where sales_market_id = ANY(select id from sales_markets
                           where market_zone IN ('Asia', 'China'))
```
- Результат:
<div style="text-align: center;">
  
  [![image20.png](https://i.postimg.cc/rwgFsJ7y/image20.png)](https://postimg.cc/CRBV6jF9)
</div>

Запрос 3:
- Назначение: Вывести компании, которые были созданы позже ВСЕХ компаний, кол-во сотрудников которых не более 100К
- Запрос:
```sql
SELECT name from companies
where creation_date >
ALL(SELECT creation_date from companies where count_of_workers <= 100000)
```
- Результат:
<div style="text-align: center;">
  
  [![image14.png](https://i.postimg.cc/Vk9mTRzB/image14.png)](https://postimg.cc/mPrv17zt)
</div>

Запрос 4:
- Назначение: Вывести модели, которые произведены компаниями с количеством работников не более 100К
- Запрос:
```sql
SELECT name from models
where company_id =  
SOME(SELECT id from companies where count_of_workers <= 100000)
```
- Результат:
<div style="text-align: center;">
  
  [![image3.png](https://i.postimg.cc/zfSZtXSd/image3.png)](https://postimg.cc/kV4pDCH8)
</div>

## Операторы `UNION` `INTERSECT` `EXCEPT`

Запрос 1:
- Назначение: Совмещение результатов двух запросов поиска моделей по id компании
- Запрос:
```sql
SELECT * FROM models WHERE company_id IN (1, 2)
UNION
SELECT * FROM models WHERE company_id IN (2, 3)
```
- Результат:
<div style="text-align: center;">
  
  [![image32.png](https://i.postimg.cc/DwGPKSNc/image32.png)](https://postimg.cc/TKRDV3fy)
</div>

Запрос 2:
- Назначение: Исключение результатов второго запроса из первого
- Запрос:
```sql
SELECT * FROM models WHERE company_id IN (1, 2)
EXCEPT
SELECT * FROM models WHERE company_id IN (2, 3)
```
- Результат:
<div style="text-align: center;">
  
  [![image30.png](https://i.postimg.cc/x8dPrtrk/image30.png)](https://postimg.cc/WFQZgm1j)
</div>

Запрос 3:
- Назначение: Пересечение результатов двух запросов поиска моделей по id компании
- Запрос:
```sql
SELECT * FROM models WHERE company_id IN (1, 2)
INTERSECT
SELECT * FROM models WHERE company_id IN (2, 3)
```
- Результат:
<div style="text-align: center;">
  
  [![image23.png](https://i.postimg.cc/L842cP5c/image23.png)](https://postimg.cc/gn7CRnvs)
</div>

## Оператор `GROUP BY`

Запрос 1:
- Назначение: Количество моделей для каждого типа кузова
- Запрос:
```sql
SELECT body_type.name, COUNT(*) from models
JOIN body_type ON models.body_type = body_type.id
GROUP BY body_type.name
```
- Результат:
<div style="text-align: center;">
  
  [![image36.png](https://i.postimg.cc/vTjfJyLj/image36.png)](https://postimg.cc/gLVxhf7q)
</div>

Запрос 2:
- Назначение: Количество моделей у каждой компании
- Запрос:
```sql
SELECT companies.name, count(models.id) from models JOIN companies
ON models.company_id = companies.id
GROUP by companies.name
```
- Результат:
<div style="text-align: center;">
  
  [![image18.png](https://i.postimg.cc/NGrMDR2b/image18.png)](https://postimg.cc/z388XbjR)
</div>

Запрос 3:
- Назначение: Город в котором находится завод и максимальное кол-во работников компании, завод которой находится в этом городе
- Запрос:
```sql
SELECT factory, MAX(count_of_workers) from models JOIN companies
ON models.company_id = companies.id
GROUP BY factory
```
- Результат:
<div style="text-align: center;">
  
  [![image5.png](https://i.postimg.cc/pTh7NWDb/image5.png)](https://postimg.cc/BL3NHGYM)
</div>

## Оператор `ORDER BY`

Запрос 1:
- Назначение: Отсортировать компании от новых к старым
- Запрос:
```sql
SELECT * from companies
ORDER BY creation_date DESC
```
- Результат:
<div style="text-align: center;">
  
  [![image24.png](https://i.postimg.cc/43qGHFPG/image24.png)](https://postimg.cc/HJwRfzFv)
</div>

Запрос 2:
- Назначение: Отсортировать компании по количеству работников по убыванию
- Запрос:
```sql
SELECT * from companies
ORDER BY count_of_workers DESC
```
- Результат:
<div style="text-align: center;">
  
  [![image37.png](https://i.postimg.cc/y80cbwJQ/image37.png)](https://postimg.cc/QFxFBYJ1)
</div>

Запрос 3:
- Назначение: Отсортировать модели в алфавитном порядке их названия
- Запрос:
```sql
SELECT * from models
ORDER BY name
```
- Результат:
<div style="text-align: center;">
  
  [![image33.png](https://i.postimg.cc/25fFjzRN/image33.png)](https://postimg.cc/PC282n5S)
</div>

## Выражение `CASE`

Запрос 1:
- Назначение: Определение и характеристика моделей, которые появились в 2000 или раньше
- Запрос:
```sql
SELECT name, release_date,
	CASE
		WHEN release_date > '1999-12-30' THEN '20XX'
		ELSE '19XX'
	END AS age
FROM models
```
- Результат:
<div style="text-align: center;">
  
[![image21.png](https://i.postimg.cc/J0wRvdHx/image21.png)](https://postimg.cc/BLgW1pgP)
</div>

## Команды `UPDATE` `INSERT` `DELETE`

Запрос 1:
- Назначение: Добавление рынков сбыта
- Запрос:
```sql
INSERT
INTO sales_markets (market_zone) VALUES 
('Europe'),
('Russia'),
('Asia'),
('China'),
('USA'),
('South America'),
('Africa');
```
- Результат:
INSERT 7

Запрос 2:
- Назначение: удаление начинающихся с A рынков
- Запрос:
```sql
DELETE FROM sales_markets WHERE market_zone LIKE 'A%'
```
- Результат:
DELETE 2

Запрос 3:
- Назначение: Изменение названия рынка сбыта
- Запрос:
```sql
UPDATE sales_markets
SET market_zone = 'RUS'
WHERE market_zone = 'Russia'
```
- Результат:
UPDATE 1

