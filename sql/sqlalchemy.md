# sqlalchemy

- Changing polymorphic type of an instance also changes the type of the instance. This will then delete the instance from the current session when we do ```db.session.commit()```. The change will be successfully commited to the database, but we need to create a new instance to interact again with that same object.
- Use ```event``` from ```sqlalchemy``` to listen to database events i.e. it listens to events happening during ```session.commit()```. See docs for more info.