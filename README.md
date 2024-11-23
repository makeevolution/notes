3# notes

## Notes on shortcuts/lessons learned for different topics

# To start the services, run: docker-compose up -d
# To manually create the network beforehand, run: docker network create mynetwork


from rest_framework import serializers
from myapp.models import RootCause
from typing import Any, Dict


class RootCauseSerializer(serializers.Serializer):
    """
    Serializer for the RootCause model. It converts RootCause documents into
    JSON format and validates incoming data for creating or updating documents.
    """
    id: str  # MongoDB ObjectId (as a string)
    root_cause: list[str]  # List of strings representing the root cause items

    def create(self, validated_data: Dict[str, Any]) -> RootCause:
        """
        Create a new RootCause document from the validated data.

        Args:
            validated_data (dict): The validated data for the new RootCause document.

        Returns:
            RootCause: The created RootCause document.
        """
        return RootCause.objects.create(**validated_data)

    def update(self, instance: RootCause, validated_data: Dict[str, Any]) -> RootCause:
        """
        Update an existing RootCause document with the validated data.

        Args:
            instance (RootCause): The existing RootCause document to be updated.
            validated_data (dict): The validated data for the update.

        Returns:
            RootCause: The updated RootCause document.
        """
        instance.root_cause = validated_data.get('root_cause', instance.root_cause)
        instance.save()
        return instance
