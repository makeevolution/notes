3# notes

## Notes on shortcuts/lessons learned for different topics

# To start the services, run: docker-compose up -d
# To manually create the network beforehand, run: docker network create mynetwork

version: '3.9'
services:
  mongo:
    image: bitnami/mongodb:8.0.3-debian-12-r0
    container_name: mongo
    ports:
      - "27017:27017"
    environment:
      # The username for the MongoDB root user (admin user).
      - MONGO_INITDB_ROOT_USERNAME=ramen
      # The password for the MongoDB root user (admin user).
      - MONGO_INITDB_ROOT_PASSWORD=iLoveJapan
    networks:
      - mynetwork

  mongo-express:
    image: mongo-express
    container_name: mongo-express
    ports:
      - "8081:8081"
    environment:
      # The connection string to access the MongoDB database.
      # Format: mongodb://<host>:<port>/
      # It points to the "mongo" service created in this file.
      - ME_CONFIG_MONGODB_URL=mongodb://mongo:27017/
      # Disables basic authentication for accessing the Mongo Express UI.
      # Set to "true" to enable authentication.
      - ME_CONFIG_BASICAUTH=false
      # The username for MongoDB administrative access, required by Mongo Express.
      # Must match the MongoDB root username set in the "mongo" service.
      - ME_CONFIG_MONGODB_ADMINUSERNAME=ramen
      # The password for MongoDB administrative access, required by Mongo Express.
      # Must match the MongoDB root password set in the "mongo" service.
      - ME_CONFIG_MONGODB_ADMINPASSWORD=iLoveJapan
    networks:
      - mynetwork

networks:
  mynetwork:
    driver: bridge
