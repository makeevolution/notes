3# notes

## Notes on shortcuts/lessons learned for different topics

docker run -it --name mongo-express -e ME_CONFIG_MONGODB_URL="mongodb://mongo:27017/" -e ME_CONFIG_BASICAUTH=false -e ME_CONFIG_MONGODB_ADMINUSERNAME=ramen -e ME_CONFIG_MONGODB_ADMINPASSWORD=iLoveJapan --network mynetwork -p 8081:8081 -d mongo-express

docker run -d --name mongo -p 27017:27017 -e MONGO_INITDB_ROOT_USERNAME=ramen -e MONGO_INITDB_ROOT_PASSWORD=iLoveJapan --network mynetwork bitnami/mongodb:8.0.3-debian-12-r0
