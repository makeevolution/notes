## As a flow
[[networking/DNS, A, AAAA, CName etc/Flow.md#^vfqnDa7O]]

## What happens when you type in a domain name to url bar (how DNS works)

Note:
- the root DNS server, the TLD server manaing .com, amazon.com, ... are/can be interchanged with the word **zone** zones

Your router resolves the domain name you type in using a Domain Name System (DNS) resolver. There are various DNS resolvers that it will ask, returning the first one with an answer.

When your router DNS resolver receives a query (for example, a request to resolve a domain name to an IP address), it checks its local cache. If the information is not present in the cache or has expired, the resolver starts A DNS recursion process.

Root DNS Servers: The resolver first contacts a root DNS server (there are 13 of them in the world https://www.iana.org/domains/root/servers). These servers are the starting point for resolving domain names. The root servers don't contain the specific information about domain names but instead will help direct the resolver to the TLD servers responsible for top-level domains (like .com, .org, .net, etc.). For our website it would direct the resolver to the TLD server for the .com zone.

- Example: if I type in example.com, the router will ask each of the 13 servers hey what is the IP of the TLD server owning .com (let's say 2.2.2.2)

Top-Level Domain (TLD) Servers: As in the example above, the root DNS servers direct the resolver to the appropriate TLD servers based on the domain's extension (.com, .org, .net, etc.). For instance, if the domain is "example.com," the resolver contacts the .com TLD server (e.g. 2.2.2.2)

Authoritative DNS Servers: The TLD servers then guide the resolver to the authoritative DNS servers responsible for the specific domain, in this case, "example.com." These authoritative servers contain the actual DNS records for the domain, such as IP addresses associated with the domain's hostname.

- Example: The 2.2.2.2 TLD server will then look up which Authoritative DNS servers own `example.com`. Let's say it found Hetzner, which have `hydrogen.ns.hetzner.com., oxygen.ns.hetzner.com. and helium.ns.hetzner.de. `. 

So, how do we then register a domain to a nameserver(s) e.g. register `example.com` to the Hetzner nameservers above? 
After we buy let's say the example.com domain from `namecheap`, we need to also find a nameserver provider that will "own" this domain, let's say `Hetzner`. So we go to Hetzner.com, and find its DNS provider service. We then can make an account and register our domain there. In `namecheap`, we also then need to specify that this domain we bought is owned by `Hetzner` nameservers.

![alt text](networking/images/image.png)

Or, we can instead use our domain directly with namecheap's nameserver (so the owner will be Namecheap), through Advanced DNS tab in the above picture, see [[networking/DNS, A, AAAA, CName etc/Flow.md#^vfqnDa7O]] for more info


There are many DNS service providers e.g. Cloudflare `https://developers.cloudflare.com/dns/nameservers/` or Google `https://cloud.google.com/dns/docs/tutorials/create-domain-tutorial`
DNS Record Retrieval: The resolver sends a query directly to the authoritative DNS servers, asking for the specific DNS record it needs (like an A record for the IP address). The authoritative servers respond with the requested information.

Response Back to Resolver: The authoritative DNS servers send the resolved information (such as the IP address associated with the domain) back to the initial resolver that made the query.

Caching: The resolver caches the received information locally for future use, reducing the need to repeat the entire process if someone else requests the same domain name in the near future.

