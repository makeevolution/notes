3# notes

## Notes on shortcuts/lessons learned for different topics

# To start the services, run: docker-compose up -d
# To manually create the network beforehand, run: docker network create mynetwork

from rest_framework import viewsets
from rest_framework.response import Response
from myapp.models import RootCause
from myapp.serializers import RootCauseSerializer
from typing import Any


class RootCauseViewSet(viewsets.ModelViewSet):
    """
    A viewset for handling CRUD operations on the RootCause model.
    Provides actions to list, retrieve, create, update, and delete root cause documents.
    """
    queryset = RootCause.objects.all()
    serializer_class = RootCauseSerializer

    def list(self, request: Any) -> Response:
        """
        List all RootCause documents in the database.

        Args:
            request (HttpRequest): The incoming HTTP request.

        Returns:
            Response: A DRF Response object containing the serialized data.
        """
        root_causes = RootCause.objects.all()
        serializer = self.get_serializer(root_causes, many=True)
        return Response(serializer.data)

    def retrieve(self, request: Any, pk: str = None) -> Response:
        """
        Retrieve a single RootCause document by its ID.

        Args:
            request (HttpRequest): The incoming HTTP request.
            pk (str): The ID of the RootCause document to retrieve.

        Returns:
            Response: A DRF Response object containing the serialized data of the document.
        """
        try:
            root_cause = RootCause.objects.get(id=pk)
        except RootCause.DoesNotExist:
            return Response({"detail": "Not found."}, status=404)

        serializer = self.get_serializer(root_cause)
        return Response(serializer.data)

    def create(self, request: Any) -> Response:
        """
        Create a new RootCause document from the incoming request data.

        Args:
            request (HttpRequest): The incoming HTTP request containing the data for the new RootCause.

        Returns:
            Response: A DRF Response object containing the created RootCause data.
        """
        serializer = self.get_serializer(data=request.data)
        if serializer.is_valid():
            serializer.save()
            return Response(serializer.data, status=201)
        return Response(serializer.errors, status=400)

    def update(self, request: Any, pk: str = None) -> Response:
        """
        Update an existing RootCause document with the provided data.

        Args:
            request (HttpRequest): The incoming HTTP request containing the updated data.
            pk (str): The ID of the RootCause document to update.

        Returns:
            Response: A DRF Response object containing the updated RootCause data.
        """
        try:
            root_cause = RootCause.objects.get(id=pk)
        except RootCause.DoesNotExist:
            return Response({"detail": "Not found."}, status=404)

        serializer = self.get_serializer(root_cause, data=request.data)
        if serializer.is_valid():
            serializer.save()
            return Response(serializer.data)
        return Response(serializer.errors, status=400)

    def destroy(self, request: Any, pk: str = None) -> Response:
        """
        Delete a RootCause document by its ID.

        Args:
            request (HttpRequest): The incoming HTTP request.
            pk (str): The ID of the RootCause document to delete.

        Returns:
            Response: A DRF Response indicating the outcome of the delete operation.
        """
        try:
            root_cause = RootCause.objects.get(id=pk)
            root_cause.delete()
            return Response(status=204)
        except RootCause.DoesNotExist:
            return Response({"detail": "Not found."}, status=404)
