# CORS    
- CORS stands for Cross Origin Resource Sharing
- CORS is enforced by the browser, so it only protects your users’ browsers (not your server)
- A mechanism that uses additional HTTP headers to tell browsers to give a web application running at one origin, 
- access to selected resources from a different origin.
- A web application makes a cross-origin HTTP request when it requests a resource that has a different origin (domain, protocol, or port) from its own.
- Why is this needed?
    - For security reasons, browsers restrict cross-origin HTTP requests initiated from scripts.
    - For example, XMLHttpRequest and the Fetch API follow the same-origin policy. This means that a web application 
    - using those APIs can only request resources from the same origin the application was loaded from unless the 
    - response from other origins includes the right CORS headers.
    - i.e. if your frontend is hosted on `mycoolfrontend.com` and it tries to make a request to `api.anotherdomain.com`,
    - the browser will block this request unless `api.anotherdomain.com` explicitly allows it via CORS headers.
    - the CORS headers are set by the server (i.e. `api.anotherdomain.com` in this case) to indicate whether the request
    - from `mycoolfrontend.com` is allowed or not, and the browser gets these values 
- In short: cross domain requests should not be allowed by default.
    - On every request, we always send a header called `ORIGIN` that indicates our hostname (e.g. `localhost:9000`, 
    - `mycoolfrontend.com`)
    - The server will need to check this origin against a list of origins it allows (e.g. ACCESS_CONTROL_ALLOW_ORIGIN 
    - of Django)
    - If ok, then the server will send its response, with ACCESS_CONTROL_ALLOW_ORIGIN and its value in the header
        - If response doesn't allow, then this header will be missing, and modern browser will not want to display 
        - anything

## An example
Lets clarify this with an example, lets say you own a a social media platform, and have millions of users. 
You have the following endpoint to retrieve user information example.com/user. 
Your security is shit and you don't check for basic things like `host` (more on that later) header or don't have 
SameSite attribute on you cookies.

Now, I, as malicious actor, can do the following:

1- Make a whatever spam site,

2- Once a user lands on my website, I make a request to example.com/user, if the user happens to have an account on your 
social media platform, and valid cookies (they have recently signed in to your platform), 
the cookies will be sent along with the request (since the request was made to the domain that issued them and you don't
have SameSite attribute on the cookies)

3- Your server sees the cookie and responds with the data

4- My malicious website receives the data, and now can do a bunch of malicious stuff with it, mainly:

A- Show it to the user, try to deceive them that they are actually on your website (easy to replicate a FE). And get
them to do something stupid like re-enter their password.

B- Make a request to my BE to save the data in DB to use it later.

The browser, through the CORS policy, stops me, the malicious actor, at point 4, and deny me access to the data. That is,
of course, unless your server EXPLICITLY responds with my domain or * in the Access-Control-Allow-Origin header. 
This is to tell the browser that it is okay for this domain to get the data from my BE. The whole idea to make you 
EXPLICITLY (rather than implicitly) allow certain domains.

So, CORS isn't there to control data access granularly (i.e. which data can be sent), it is there to prevent malicious actors to get data of others 
trough their browsers. The malicious user can make all the requests they want from their server, postman ,,, etc, but 
they won't have the victim's cookies.

In other words, CORS isn't there to prevent access to public data, there is nothing that can do that, public data is 
public by design. CORS is there to prevent access to private data of other users by malicious actors.

This is very similar to using the host header to check the origin of a request. When making a request from postman or a 
server, you can set whatever host header you want, but not when you are making the request from the browser. This applies to User-Agent too.