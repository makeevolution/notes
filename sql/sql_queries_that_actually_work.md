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
