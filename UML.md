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
Aggregation is a more specific version of Association, and Composition is a more specific version of Aggregation.
![https://imgur.com/7myeA6o](https://imgur.com/7myeA6o)
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
