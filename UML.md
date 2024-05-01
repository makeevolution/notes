![https://i.stack.imgur.com/YCegU.png](https://i.stack.imgur.com/YCegU.png)
---
Association: Ownership of another type (e.g. 'A' owns a 'B')
```java
//@assoc  The Player(A) has some Dice(B)
class Player {
    Dice myDice;
}
```
---
Aggregation is a more specific version of Association, and Composition is a more specific version of Aggregation. Aggregation

[https://imgur.com/7myeA6o](https://imgur.com/7myeA6o)

```java
class Person {
    private Heart heart;
    private List<Hand> hands;
}
```
```java
class City {
    private List<Tree> trees;
    private List<Car> cars
}
```
In composition (Person, Heart, Hand), "sub objects" (Heart, Hand) will be destroyed as soon as Person is destroyed.

In aggregation (City, Tree, Car) "sub objects" (Tree, Car) will NOT be destroyed when City is destroyed.

The bottom line is, composition stresses on mutual existence, and in aggregation, this property is NOT required.

---

Realization: The class extends a parent class, the parent class is at the arrow end. The dashed line is the signal that the block at the arrow end is a class, not an interface.

Inheritance: The class implements an interface. The solid line is the signal that the block at the arrow end is an interface, not a class. This helps to differentiate it with realization.

If you have a abstract base class and want to extend it, use Realization. If you want to implement the abstract base class, use Inheritance.

---

Dependency: Use of another type (e.g. 'C' uses a 'D')
```
//@dep    The Player(C) uses some Dice(D) when playing a game
class Player {
    rollYahtzee(Dice someDice);
}
```

---

Example on how to make plantUML diagram:
```
@startuml

entity "Group" as group {
    + id: int
    --
    name: string
}

entity "Role" as role {
    + id: int
    --
    name: string
}

entity "Permission" as permission {
    + id: int
    --
    name: string
}

entity "User" as user {
    + id: int
    --
    username: string
}

group }|--|{ role : "contains"
role }|--|{ permission : "grants"
group }|--|{ user : "includes"

@enduml
newpage
@startuml
/'=================================================================='/
'Create participants'
actor User
actor Superuser
participant Client_VFM_frontend
participant Resource_server_VFM_backend
participant Authorization_server
database Authorization_server_db
database VFM_db

/'=================================================================='/
'Events'


activate Authorization_server
Authorization_server -> Authorization_server: First startup ever
Authorization_server -> Authorization_server: Register the VFM frontend application with the authorization server
deactivate Authorization_server

activate Superuser
Superuser -> Authorization_server: Create a new permission (e.g. view:testcase)
activate Authorization_server
Authorization_server -> Authorization_server_db: Saves the changes
deactivate Authorization_server
deactivate Superuser

User -> Client_VFM_frontend: login with username and password
activate User
activate Client_VFM_frontend
Client_VFM_frontend -> Authorization_server: send username and password as payload
activate Authorization_server
Authorization_server -> Authorization_server_db: gets user data and group membership
Authorization_server_db -> Authorization_server: returns user data and group membership
Authorization_server -> Authorization_server: generates access token (JWT containing all user data)
Authorization_server -> Client_VFM_frontend: returns access token and refresh token
Client_VFM_frontend -> Resource_server_VFM_backend: create/update (if needed) user profile in VFM backend
Resource_server_VFM_backend -> Client_VFM_frontend: reports user profile update is successful
Client_VFM_frontend -> User: User directed to execute test case page
deactivate Authorization_server
deactivate Client_VFM_frontend
deactivate User

User-[hidden]->User
User -> Client_VFM_frontend: User requests a resource
activate User
activate Client_VFM_frontend
Client_VFM_frontend -> Resource_server_VFM_backend: makes a request to a protected endpoint with access token on header
Activate Resource_server_VFM_backend
Resource_server_VFM_backend -> Authorization_server: validate access token by making a call to introspection endpoint
Authorization_server -> Resource_server_VFM_backend: returns whether the access token is valid or not
Resource_server_VFM_backend -> Authorization_server: decodes the JWT access token, and process the request based on the scope i.e. permissions of the user found inside the decoded token
alt response code is > 3xx
  Client_VFM_frontend -> Authorization_server: Request to refresh the access token
  alt refresh fails
    Authorization_server -> Client_VFM_frontend: Send failure response
    Client_VFM_frontend -> Client_VFM_frontend: Log out user
  else refresh successful
    Authorization_server -> Authorization_server: Generate new access token
    Authorization_server -> Client_VFM_frontend: Return access token
    Client_VFM_frontend -> Resource_server_VFM_backend: Retry the request with new access token
    Resource_server_VFM_backend -> Client_VFM_frontend: Responds with resource
  end
else response code is 2xx or 3xx
    Resource_server_VFM_backend -> Client_VFM_frontend: Responds with resource
end
@enduml
```
