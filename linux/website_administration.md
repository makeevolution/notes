# Website beheren

## NGINX
- Als je een domain op `namecheap.com` heeft gekocht, volg de instruties op https://www.youtube.com/watch?v=zOPH54ltGLQ
om je droplet te configureren met de domain naam.
- En dan, volg de instruties op https://www.digitalocean.com/community/tutorials/how-to-install-nginx-on-ubuntu-20-04 om
je server met NGINX en ufw te configureren, zodat je je server met je domain naam toegang kan krijgen.
- NGINX is een toepassing die internetverbindingen met je server configureren.
- Belangrijke mappen in NGINX, en korte omschrijving erover:
```
/
├── var/
│   ├── www/
│   │   ├── html (Dit bestand omvat je html bestanden)/
│   │   │   └── index.nginx-debian.html
│   │   └── aldosebastian/
│   │       └── html/
│   │           └── index.html
│   └── log/
│       └── nginx/
│           ├── access.log (elke vraag naar je web server wordt in dit bestand opgetekend)
│           └── error.log (elke errors wordt hier opgetekend)
└── etc/
 └── nginx/
     ├── sites-available (die map omvat de zogenaamde `server-blocks`)/
     │   ├── default
     │   └── aldosebastian
     ├── sites-enabled (die map omvat `linux symlinks` die naar bestanden in sites-available bestand verwijzen)/
     │   ├── default
     │   └── aldosebastian
     ├── nginx.conf (het hoofdconfiguratiebestand voor nginx)
     ├── (andere mappen)
     └── ...
```
- Elk bestand in `sites-available` omvat een inhoud vergelijkbaar met het volgende:
```
server {
        listen 80;
        listen [::]:80;

        root /var/www/your_domain/html;
        index index.html index.htm index.nginx-debian.html;

        server_name your_domain.com www.your_domain.com;

        location / {
                try_files $uri $uri/ =404;
        }
}
```
Met deze inhoud, als je naar `www.your_domain.com:80` gaat, zul je de `index.html` pagina 
die in `/var/www/your_domain/html` map zien.
- Meer over NGINX `server-blocks` configuratie: https://www.digitalocean.com/community/tutorials/understanding-nginx-server-and-location-block-selection-algorithms
- Commandos (ze zijn vanzelfsprekend):
  - sudo systemctl start nginx
  - sudo systemctl restart nginx
  - sudo systemctl reload nginx (herstarten zonder huidige verbindingen laten vallen)
  - sudo systemctl disable nginx (zet auto-start van nginx als de server herstarten uit)
  - sudo systemctl enable nginx (zet auto-start van nginx als de server herstarten aan)
  - sudo nginx -t (controleren of je fouten in je Nginx bestanden maakt)
  
## UFW
- Dit is een firewall toepassing, die verbindingen naar bepaalde poorten blokkeert/toestaan.
- In de achtergrond, werkt deze toepassing een andere toepassing heette `iptables` bij.
  - Een Docker container werkt de `iptables` toepassing direct bij, dus poorten die `ufw` als geblokkeert aangemeld is
    misschien niet echt geblokkeerd! Meer informatie: https://askubuntu.com/questions/652556/uncomplicated-firewall-ufw-is-not-blocking-anything-when-using-docker
- Soms na configuratie van deze toepassing, kunt je geen meer SSH verbinding maken. Meer info hier https://superuser.com/questions/1508570/can-not-access-ubuntu-server-with-ssh-after-allowing-firewall.

    
***In kortom, vergeet niet om `ufw allow ssh` uit te voeren!!! Anders moet je met een fysieke keyboard en monitor
naar je server aansluiten, en ssh werkt niet meer!***

- Commandos (ze zijn vanzelfsprekend):
  - sudo ufw app list
  - sudo ufw status
  - sudo ufw allow 'een-van-de-uitvoer-van-sudo-ufw-app-list'
  
## SSL
- Uitgebreid: https://www.digitalocean.com/community/tutorials/how-to-secure-nginx-with-let-s-encrypt-on-ubuntu-20-04
- Momenteel in Smallblog is SSL geinstalleerd. De configuratie is zodat alle HTTP verbindingen, onafhankelijk van de poort,
aan HTTPS aangepast worden.