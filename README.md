3# notes

## Notes on shortcuts/lessons learned for different topics

# To start the services, run: docker-compose up -d
# To manually create the network beforehand, run: docker network create mynetwork
from rest_framework import serializers

class RootCauseSerializer(serializers.Serializer):
    id = serializers.CharField(read_only=True)  # MongoDB's ObjectId
    root_cause = serializers.ListField(
        child=serializers.CharField(max_length=200)
    )

    def create(self, validated_data):
        # Create a new document in MongoDB
        return RootCause.objects.create(**validated_data)

    def update(self, instance, validated_data):
        # Update the existing document
        instance.root_cause = validated_data.get("root_cause", instance.root_cause)
        instance.save()
        return instance
