# Docker

- Containers are instances of an image
-  Get all running containers and images inside it: ```sudo docker ps -a```
- Get into failed docker build: ```sudo docker run -i -t (last successfully built image inside the container where the build failed) sh```
- Delete all containers: ```docker container prune```
- Delete one container: ```docker rm CONTAINER```
- Delete all images and containers: ```docker system prune --all"```
- Delete one image: ```docker rmi IMAGE```