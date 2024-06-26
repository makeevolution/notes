# Docker

- Containers are instances of an image
-  Get all running containers and images inside it: ```sudo docker ps -a```
- Get into failed docker build: ```sudo docker run -i -t (last successfully built image inside the container where the build failed) sh```
- Delete all containers: ```docker container prune```
- Delete one container: ```docker rm CONTAINER```
- Delete all images and containers: ```docker system prune --all"```
- Delete one image: ```docker rmi IMAGE```
- MYSQL image draai niet; exit met 137 error. Oplossing: zie https://unix.stackexchange.com/questions/128642/debug-out-of-memory-with-var-log-messages. In kortom: Linux kan mischieen je container afmaken zonder je het te weten want je heeft geen genoeg RAM :(
- Kan geen verbinding maken met een MYSQL container met een shared volume, hoewel je de environment variabelen hebben toegepast in Docker Compose, en toegang van buiten hebben toelaten. Oplossing: https://github.com/docker-library/mysql/issues/275#issuecomment-292243855. In kortom: In de eerste poging van `docker-compose up`, zorg ervoor dat de volume die in `volume` rij van je `docker-compose.yml` NIET BESTAAT!!! Anders, wordt de environment variabelen niet toegepast!
- Make non-root user (who is sudoer) able to run docker without sudo: `sudo usermod -aG docker $USER`
- Shell vs. Exec form of CMD and Entrypoint; it matters: https://emmer.dev/blog/docker-shell-vs.-exec-form/#signal-trapping-forwarding (use internet archive if the link is gone)
