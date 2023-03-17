- Find db string from a model instance (useful when debugging): modelInstance._state.db
- How to find which db is currently used (useful when debugging, run in console)
  ```
  from django.conf import settings
  settings.DATABASES
  ```
