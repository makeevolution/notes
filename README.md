3# notes

## Notes on shortcuts/lessons learned for different topics

# To start the services, run: docker-compose up -d
# To manually create the network beforehand, run: docker network create mynetwork
import logging
from mongoengine import connect, disconnect
from pymongo import MongoClient

logging.basicConfig(level=logging.INFO, format="%(asctime)s - %(levelname)s - %(message)s")
logger = logging.getLogger(__name__)

def ensure_mongo_connection(db_name, host="localhost", port=27017, username=None, password=None):
    try:
        connect(
            db=db_name,
            host=host,
            port=port,
            username=username,
            password=password
        )
        logger.info(f"Successfully connected to MongoDB at {host}:{port} and database '{db_name}'.")

        client = MongoClient(host, port, username=username, password=password)
        if db_name in client.list_database_names():
            logger.info(f"Database '{db_name}' exists.")
            return True
        else:
            logger.error(f"Database '{db_name}' does not exist.")
            disconnect()
            return False

    except Exception as e:
        logger.error(f"MongoDB connection failed: {e}")
        return False

