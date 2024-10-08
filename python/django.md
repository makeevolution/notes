- DO NOT USE `.all()` method! Remember `populate_from_artifactory()` so slow that we had so many issues!!!!!! See `https://nextlinklabs.com/resources/insights/django-big-data-iteration` for alternatives
- check if status code is 2xx
```
from rest_framework import status
status.is_success(yourcode)
```

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
- If you want to iterate through each existing for an object, don't use `.all()`! It will save the list in memory and if there's millions of records you're dead. Use `get_queryset()`, it also supports __iter__ and does not load the items to memory, but rather one by one when you are actually iterating.
- If you're on debug mode and committing a save, and it raises any kind of error, Django won't allow any other database related actions since you're still in the middle of an atomic operation! To force Django to get out of atomic operation, do the following:
  ```from django.db import transaction
     transaction.get_connection().in_atomic_block=False
  ```
  Additionally, since your debugging has raised an exception, your test run will fail inevitably :(
- To avoid errors in saving data using Django, it's best to call `full_clean()` before `save()` https://docs.djangoproject.com/en/2.0/ref/models/instances/#django.db.models.Model.full_clean. There the info is not low-level, low level info is here https://django.readthedocs.io/en/stable/ref/forms/validation.html
- Reset all tables (including sessions, auth, admin etc) and data inside: `python manage.py reset_db`. Need to install django extensions too
- Use https://django-extensions.readthedocs.io/en/latest/sqldiff.html to evaluate db migration status with that of your code's migration; doesn't work for mysql
- If you change your django migration file(s) and your db is out of sync with your migrations db code, to make them in sync again easily (data will be lost):
    -  go to `django_migrations` table in the db, and identify the migration scripts that are different to your code
    -  drop the tables/columns being created/altered in these scripts, use best judgement when doing this
    -  delete these scripts rows from `django_migrations`
    -  Run `python manage.py migrate` again
-  How to check all reverse urls in your Django app: https://stackoverflow.com/questions/1275486/how-can-i-list-urlpatterns-endpoints-on-django
- Django's `get_or_create` method is case sensitive against SQLite but its `__exact` method is not; be careful. To make `__exact` case sensitive, use this before your query:
```
                with connection.cursor() as cursor:
                    cursor.execute("PRAGMA case_sensitive_like = true;")
```
- Useful Django apps and middlewares:
    - Whitenoise (to see static files served behind WSGI)
    - log request id (to have logging ID of all requests)
    - django_structlog (to have structlog as django logger)
    - django extensions
    - watchman (for heartbeat checks to /ping endpoint, useful for k8s liveness probe)
    - The structlog config that I found to work for django_structlog (put this in settings.py)
  ```
  structlog.configure(
    processors=[
        structlog.contextvars.merge_contextvars,
        structlog.processors.CallsiteParameterAdder(
            {
                structlog.processors.CallsiteParameter.PATHNAME,
                structlog.processors.CallsiteParameter.FUNC_NAME,
                structlog.processors.CallsiteParameter.LINENO,
            }
        ),
        structlog.processors.add_log_level,
        structlog.processors.TimeStamper(fmt="iso", utc=False),
        structlog.dev.ConsoleRenderer(),
    ],
)

- Django OAuth toolkit notes:
    - If a refresh token does not have a corresponding access token in the db, then it will throw (confusingly) a 400 bad request

- vscode to debug django that works, put this in launch.json:
  ```
  {
    // Use IntelliSense to learn about possible attributes.
    // Hover to view descriptions of existing attributes.
    // For more information, visit: https://go.microsoft.com/fwlink/?linkid=830387
    "version": "0.2.0",
    "configurations": [
        {
            "name": "Python: Django",
            "type": "python",
            "request": "launch",
            "program": "${workspaceFolder}\\backend\\manage.py",
            "cwd": "${workspaceFolder}\\backend",
            "args": [
                "runserver",
                "localhost:8001"
            ],
            "django": true,
            "justMyCode": true
        },
    ]
  }
  ```
