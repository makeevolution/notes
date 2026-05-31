- Get all foreign keys in a table:
  ```
  USE '<yourschema>';  # verwijder de aanhalingstekens

  SELECT i.TABLE_NAME, i.CONSTRAINT_TYPE, i.CONSTRAINT_NAME, k.REFERENCED_TABLE_NAME, k.REFERENCED_COLUMN_NAME 
  FROM information_schema.TABLE_CONSTRAINTS i 
  LEFT JOIN information_schema.KEY_COLUMN_USAGE k ON i.CONSTRAINT_NAME = k.CONSTRAINT_NAME 
  WHERE i.CONSTRAINT_TYPE = 'FOREIGN KEY' 
  AND i.TABLE_SCHEMA = DATABASE()
  AND i.TABLE_NAME = '<yourtable>';  # houd de aanhalingstekens op
  ```

- Get the DDL that will create the table from scratch to get all details of the table:
```
SELECT DBMS_METADATA.GET_DDL('TABLE', 'SOMETABLE') FROM dual;
```

- Get all sequences in the all tables like PARTIALTABLENAME
```
-- sequences
SELECT * FROM all_sequences WHERE sequence_name LIKE '%PARTIALTABLENAME%';
```

- Get all triggers in all tables like PARTIALTABLENAME

```
-- triggers
SELECT * FROM all_triggers WHERE table_name LIKE '%PARTIALTABLENAME%';
```

- Efficiently get the first match given a where condition (no need to scan through all rows)
```
-- Returns an int (1) when found, otherwise no rows aka null. 
SELECT 1 
FROM SOMETABLE 
WHERE IDCOLUMN = :id 
FETCH FIRST 1 ROWS ONLY
```
-- Get details of a column in a table
SELECT *
FROM all_tab_columns
WHERE table_name = 'VAN_CARRIERTRANSFER'
AND column_name = 'CARRIERTRANSFER';

-- check if a column is nullable on the tables where the column is used as a foreign key
SELECT TABLE_NAME, COLUMN_NAME, NULLABLE, DATA_TYPE
FROM ALL_TAB_COLUMNS
WHERE TABLE_NAME IN (
    'VAN_CARRIERTRANSFERAREA',
    'VAN_SHIPMENTCARRIERGROUP',
    'VAN_ADDITIONALCARRIERS',
    'VAN_SHIPMENTCARRIER'
)
AND OWNER = 'DEVVANESSA'
AND COLUMN_NAME = 'CARRIERTRANSFERID';
