---

excalidraw-plugin: parsed
tags: [excalidraw]

---
==⚠  Switch to EXCALIDRAW VIEW in the MORE OPTIONS menu of this document. ⚠== You can decompress Drawing data with the command palette: 'Decompress current Excalidraw file'. For more info check in plugin settings under 'Saving'


# Excalidraw Data
## Text Elements
How to communicate null intent using AllowNull (usually for singleton pattern mainly) ^NH70gY98

Often for service classes, we just want to make a singleton of it instead of instantiating it always. ^gr26HSNP

1. Singleton pattern, the user needs to call Create() to
be able to use the service ^ESc4vzjM

2. The problem: we do not intend the instance to be null, but,
here we need ? to communicate to the compiler that it can be null.

But that means it can be null at any time, while for our singleton pattern, we
only allow it to be null before first time instantiation, at other times not. While
the ? means we intend it to be null at any point.

How to communicate intent to the compiler, that the fiel can be set to null, but internally we will handle
it after the set, such that it won't be null? ^qXU1IBIj

Use AllowAny attribute ^dTkDd8Te

Now the warning green squiggle is gone

This attribute tells the compiler "hey, this attribute can be
set to null (e.g. before instantiation/by user), but the class will
internally ensure that when we try to get, it will not be null

In this case here, the fact that we need to call Create()
is that insurance that it will never be null, even if it was set
to null

Notice also that we make that assurance by making the
field private i.e. users of the service cannot do instance=null

So the compiler don't need to worry. ^N4AzaT9m

DisallowNull ^SO38W6LG

What if I want to be able to set the reference property to be null after instantiation, but after it is assigned to a non-null value by the user,
prevent them from assigning it to null again? ^8JuvIwRv

1. We want to prevent this from being possible/warn the user to not do this since the intent is it to not be null after assignment
e.g. A use case would be, we want the client to call Cancel() method to cancel a reservation, instead of unassigning the vehicle
from the reservation like what is done here

Notice there is no squiggly green line, which means our intention above is not captured! ^jhfmCMHk

Use DisallowNull ^qUBBQ7Gi

Now the client gets a warning! ^v02oVEni

Cool, but they can still ignore this warning right? How can we safeguard our code even more?
(this is not related directly to DisallowNull, but just an idee)

Don't use the syntatic auto-property sugar, and create a setter that checks for null assignment! ^vOswG1Rc

Use traditional way to declare properties (i.e. without the sugar)
and customize your setter! Safer against rebels ^vbJRZZv5

How to tell the compiler that, after a method is ran, a field will never be null? ^LNnrLocq

How do we tell the compiler that, after we call Create(),
CreatedOn will not be null, so don't warn us? ^X57wXRgW

Use MemberNotNull ^K1arPWHt

Now it's ok! ^awEDUlKj

MemberNotNull ^whLpPHg6

NotNullIfNotNull ^uer50Bnj

How to tell compiler that, if a parameter passed in to a function is not null, then the result of that function we guarantee not to be null? ^JwGY8WMJ

Vehicle is not intended to be able to be set to null by user (see DisallowNull above).
Now, the only condition this service call can return null is if we pass in a null into the argument.
How can we be more flexible and convince the compiler, that if we call the upgrade vehicle with a
non-null argument, then the result we guarantee will not be null  ??? ^RgjzWffM

Use NotNullIfNotNull, passing in the argument that will ensure that, if that argument is not null, then the output of the function is not null! ^4ODptTrK

No more warning! ^Irze3Nfs

[AllowNull] ^gZbjN7rH

public string DisplayName { get; set; } // ⚠️ You can assign null, but must return non-null ^oNATtOIc

## Embedded Files
60ed9607cff24e6a803e3fdeecc828fefd5165cd: [[Pasted Image 20250419202838_220.png]]

0b23000b1d24dfe8ee8fc9eaf166bbc422a049d6: [[Pasted Image 20250419203228_913.png]]

df354e116e8d76cdd1f8e7730cb2e0dbecf0e8dc: [[Pasted Image 20250419204338_215.png]]

8bc84584e01506979d88f54278f7ec9fa9179a76: [[Pasted Image 20250419204536_356.png]]

e1373617f6f7b2c3660d3f30e8fe4e6bd0744235: [[Pasted Image 20250419204701_805.png]]

36d18bc0107ce28a39ba45a6d16ee76e2a2aef47: [[Pasted Image 20250419204740_132.png]]

1b8d5d7607ce4ddcfe97da431a828eb3aba6fd67: [[Pasted Image 20250419205049_735.png]]

6d3afc893fc1ae92989b733b219ba1cbb2cac393: [[Pasted Image 20250419210429_921.png]]

cc4c42dcad89fb5982bac80e376ed7f0654683fe: [[Pasted Image 20250419210545_619.png]]

08ccf2af2d4f6e22abf7395cce5e6015015b71c9: [[Pasted Image 20250419210606_257.png]]

46d588dbcb63e3e2a0b8fbc35b3a88235ab6ff83: [[Pasted Image 20250419210721_040.png]]

1099456aeb85b62864d2ee826fa43fd75268f72f: [[Pasted Image 20250419211040_937.png]]

1d9c0c9694ea441e4e9e54b95ba7c866b3349ceb: [[Pasted Image 20250419211103_160.png]]

0a7b4bb9dfb1c17323ce2d86476c040dda533412: [[Pasted Image 20250419211634_542.png]]

8ccbf561370f68808e5b851e69bbe60207eccd90: [[Pasted Image 20250419211651_152.png]]

%%
## Drawing
```compressed-json
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=
```
%%