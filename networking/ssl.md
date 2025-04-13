# SSL, CA, HTTPS, TLS, etc. how do they work
- In HTTP, information is sent as plain text between a client and server
- HTTPS is HTTP with security features; the data sent is encrypted, and the identity of both parties being verified before any data is exchanged (SSL)
  - SSL is actually superseded by TLS and is similar, but the word SSL is very catchy and they are interchangeable
- What does a server do to get a certificate?
  1. Key Pair Generation:

     The server generates a public-private key pair. The private key is kept secret, while the public key will be shared. In Linux, use `openssl` to generate the pair

  2. Certificate Signing Request (CSR):

      The server creates a Certificate Signing Request (CSR). This CSR includes:
  The server’s public key.
  Information about the server (e.g., domain name, organization details).
  
  3. Submission to CA:

      The server sends the CSR to a Certificate Authority (CA). The CA is a trusted entity that issues certificates.

      The CA has its own public and private keys for encryption of data; will explain their relevance later.
  
  4. CA Verification and Certificate Issuance:

      The CA verifies the information in the CSR (e.g., domain ownership, organization details).
      Once the CA is satisfied with the verification, it creates a certificate and `signs` the certificate. This certificate includes:

      - The requesting server’s public key.
      - Information about the server.
      - Expiry date
      - Hashing algorithm used for digital signature (see digital signature below for what this is about)
      - Other info
      - The digital signature created by the CA.
        - A signature is: all the info above, `hashed` using an algorithm (e.g. SHA-256), then `encrypted` using the `CA server's private key`
      
      <b>Thus, signing a certificate means:
      - Generate a hash of the certificate
      - encrypt the hash using a private key
      </b>
    
  5. Certificate Delivery:

     CA sends the signed certificate back to the server. This certificate is now signed by the CA's private key and includes the CA's digital signature.
  
  The above is a simple situation, but in reality, the CA actually doesn't send only one certificate but rather a `chain` of certificates
  ![alte](certificatechain.png)
  1. (start from bottom right in the diagram). The CA has in itself a `root certificate` which among other things contains the `public key` we talked about above (from now referred as `root public key`), signed by the `private key` (referred as `root private key`). Since it signs itself, this is called `self-signed` certificate.
  2. The CA will generate an `intermediate certificate` as you can see in pic above. Then, it will:
      - generate another public-private key pair (`intermediate public-private key pair`)
      - embed this intermediate public key inside the `intermediate certificate`
      - sign this certificate i.e.
        - Hash this certificate using SHA-256
        - Encrypt the certificate using the ROOT CERTIFICATE's private key (not self-signed!)
  3. Finally, the CA generates the `server's certificate`. Then it will:
      - embed the server public key inside the `server's certificate`
      - sign this certificate i.e.
        - Hash this certificate using SHA-256
        - Encrypt the certificate using the INTERMEDIATE CERTIFICATE's private key.

- Now, how is this used in HTTPS? How is this useful?
  1. After the client contacts the server, the server gives all the certificates in the picture above
  2. The client (i.e. the browsers) will then:
      - Use the `intermediate certificate`'s public key to decrypt the `server certificate`'s digital signature, which will return the `server certificate`'s hashed data
      - Hash the `server certificate` data (the hashing algorithm to be used is inside the data itself).
      - Compare the two hashes. If they are equal, then the `server's certificate` is legitimate
      - Do the above for `intermediate certificate` too, use the `root certificate`'s public key
      - If the `intermediate certificate` is also legitimate, then we are sure we are talking to a legitimate server.
  3. The premaster secret: The client then sends one more random string of bytes, the "premaster secret." The premaster secret is encrypted with the public key and can only be decrypted with the private key by the server. (The client gets the public key from the server's SSL certificate.)
  4. Private key used: The server decrypts the premaster secret.
  5. Session keys created: Both client and server generate session keys from the client random, the server random, and the premaster secret. They should arrive at the same results.
  6. Client is ready: The client sends a "finished" message that is encrypted with a session key.
  7. Server is ready: The server sends a "finished" message encrypted with a session key.
  8. Secure symmetric encryption achieved: The handshake is completed, and communication continues using the session keys.

- So how is this TLS/SSL thing helpful in mitigating attack??
  - If the attacker is pretending as the server i am trying to connect to:
    - it will send its server and intermediate certificates
    - when I decrypt its intermediate certificate's digital signature using the root certificate's public key, and I hash the intermediate certificate, the hash value I get from decryption won't equal the hash of the intermediate certificate I computed; the hash value I get from decryption is gibberish.
    - This is because its intermediate certificate is not issued by the CA!

### Setup SSL in Digital Ocean

- Uitgebreid: https://www.digitalocean.com/community/tutorials/how-to-secure-nginx-with-let-s-encrypt-on-ubuntu-20-04
- Momenteel in Smallblog is SSL geinstalleerd. De configuratie is zodat alle HTTP verbindingen, onafhankelijk van de poort,
aan HTTPS aangepast worden.