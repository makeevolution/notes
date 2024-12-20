# Notes on system design

- There are many different software architectures, but how do we deploy them?

- Let's say we use the classic layered architecture as an example
![alt text](softwaredesign/image.png)
- We can deploy this in a server
- But this won't scale, in the sense that we won't be able to serve 100k users!
- We then can do `vertical scaling` i.e. make the server stronger
![alt text](softwaredesign/image-1.png)
- But still we can only make it stronger so far! What if we need to serve `10M` users?
- We then look into `horizontal scaling`; here we attempt to separate the `UI`, `backend` and 