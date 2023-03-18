- Find which db is used from a model instance (useful when debugging), if you use multiple database other than 'default': modelInstance._state.db. 
- How to ascertain which db is currently used (useful when debugging, run in console)
  ```
  from django.conf import settings
  settings.DATABASES
  ```
- Useful places to look and interrogate Django ORM related stuff:
  base.py of backend/django
  transaction.py in backed/django/db
  models/base.py
- If you're on debug mode and committing a save, and it raises any kind of error, Django won't allow any other database related actions since you're still in the middle of an atomic operation! To force Django to get out of atomic operation, do the following:
  ```from django.db import transaction
     transaction.get_connection().in_atomic_block=False
  ```
  Additionally, since your debugging has raised an exception, your test run will fail inevitably :(
- To avoid errors in saving data using Django, it's best to call `full_clean()` before `save()` https://docs.djangoproject.com/en/2.0/ref/models/instances/#django.db.models.Model.full_clean. There the info is not low-level, low level info is here https://django.readthedocs.io/en/stable/ref/forms/validation.html
