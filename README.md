
3# notes

## Notes on shortcuts/lessons learned for different topics

from unittest.mock import patch, MagicMock
import pytest
from typing import Generator
from your_module import ensure_mongo_connection

# Mocked MONGODB settings
MONGODB = {
    "NAME": "testdb",
    "SERVER": "localhost:27017",
    "USER": "testuser",
    "PASSWORD": "testpassword",
}

@pytest.fixture
def mock_logger() -> Generator[MagicMock, None, None]:
    """Fixture to mock the logger.

    Yields:
        MagicMock: A mock logger object.
    """
    with patch("your_module.logger") as mock_logger:
        yield mock_logger

@pytest.fixture
def mock_connect() -> Generator[MagicMock, None, None]:
    """Fixture to mock the MongoDB connect function.

    Yields:
        MagicMock: A mock connect function object.
    """
    with patch("your_module.connect") as mock_connect:
        yield mock_connect

@pytest.fixture
def mock_mongo_client() -> Generator[MagicMock, None, None]:
    """Fixture to mock the MongoClient.

    Yields:
        MagicMock: A mock MongoClient object.
    """
    with patch("your_module.MongoClient") as mock_client:
        mock_instance = MagicMock()
        mock_client.return_value = mock_instance
        yield mock_instance

def test_ensure_mongo_connection_success(
    mock_logger: MagicMock, mock_connect: MagicMock, mock_mongo_client: MagicMock
) -> None:
    """Test successful MongoDB connection.

    Args:
        mock_logger (MagicMock): Mocked logger.
        mock_connect (MagicMock): Mocked MongoDB connect function.
        mock_mongo_client (MagicMock): Mocked MongoClient.
    """
    mock_mongo_client.admin.command.return_value = {"ok": 1}
    assert ensure_mongo_connection() is True
    mock_logger.info.assert_any_call("Establishing/ensuring connection with MongoDB ...")
    mock_logger.info.assert_any_call("MongoDB is available, ping response: {'ok': 1}")

def test_ensure_mongo_connection_no_auth(
    mock_logger: MagicMock, mock_connect: MagicMock, mock_mongo_client: MagicMock
) -> None:
    """Test MongoDB connection without authentication.

    Args:
        mock_logger (MagicMock): Mocked logger.
        mock_connect (MagicMock): Mocked MongoDB connect function.
        mock_mongo_client (MagicMock): Mocked MongoClient.
    """
    global MONGODB
    MONGODB.pop("USER", None)
    MONGODB.pop("PASSWORD", None)
    mock_mongo_client.admin.command.return_value = {"ok": 1}
    assert ensure_mongo_connection() is True

def test_ensure_mongo_connection_failure(
    mock_logger: MagicMock, mock_connect: MagicMock
) -> None:
    """Test MongoDB connection failure.

    Args:
        mock_logger (MagicMock): Mocked logger.
        mock_connect (MagicMock): Mocked MongoDB connect function.
    """
    mock_connect.side_effect = Exception("Connection Error")
    assert ensure_mongo_connection() is False
    mock_logger.error.assert_any_call("MongoDB connection failed: Connection Error")





import traceback
import typing

import structlog
from mongoengine import connect
from pymongo import MongoClient
from virtual_fab_manager_2.settings import MONGODB

logger = structlog.get_logger(__name__)

mongo_connection: typing.Optional[MongoClient] = None

CONNECTION_CHECK_TIMEOUT = 5000


def ensure_mongo_connection() -> bool:
    """
    Ensure mongo db cluster holding root causes is reachable

    Returns:
        boolean whether cluster is available
    """
    global mongo_connection  # noqa: WPS420
    try:
        logger.info("Establishing/ensuring connection with MongoDB ...")
        if mongo_connection is None:
            host = (
                f"mongodb://{MONGODB['USER']}:{MONGODB['PASSWORD']}@{MONGODB['SERVER']}"  # noqa: WPS221
                if all(key in MONGODB for key in ("USER", "PASSWORD"))
                else f"mongodb://{MONGODB['SERVER']}"
            )

            mongo_connection = connect(  # noqa: WPS442
                db=MONGODB["NAME"],
                host=host,
                serverSelectionTimeoutMS=CONNECTION_CHECK_TIMEOUT,
            )
        logger.info(f"MongoDB is available, ping response: {mongo_connection.admin.command('ping')}")
    except Exception as exc:  # noqa: B902 pylint: disable=broad-exception-caught
        logger.error(traceback.format_exc())
        logger.error(f"MongoDB connection failed: {exc}")
        return False
    return True



class RootCauseViewSet(viewsets.ModelViewSet):
    """
    Viewset for handling CRUD operations on the RootCause model.
    """

    queryset = RootCause.objects.all()
    serializer_class = RootCauseSerializer
    lookup_field = ROOT_CAUSE
    pagination_class = CustomSizePagination

    def dispatch(
        self,
        request: drf_request,
        *args: typing.Any,
        **kwargs: typing.Any,
    ) -> JsonResponse | Response:
        """
        Check MongoDB connection before processing any request

        Args:
            request: The request object to check
            args: variable args mandatory by DRF
            kwargs: key value args mandatory by DRF

        Returns:
            Response object
        """
        if not ensure_mongo_connection():
            return JsonResponse(
                {"error": "MongoDB is currently unavailable, unable to get root cause patterns!"},
                status=status.HTTP_500_INTERNAL_SERVER_ERROR,
            )
        return super().dispatch(request, *args, **kwargs)


    def stream_file_by_url(self, file_url: str) -> requests.Response:  # noqa: WPS212, WPS221, WPS231
        """
        Streams the file available at the provided link

        Args:
            file_url: artifactory link of the file

        Returns:
            downloaded file path object
        """
        with requests.Session() as sess:
            retries = Retry(
                total=5,  # total retries; will retry on all connection errors (e.g. ReadTimeoutError, etc.)
                # and response status codes specified below
                backoff_factor=0.1,
                status_forcelist=[500, 502, 503, 504],  # will also retry if response is in these
            )
            sess.mount(f"{self.base_url}/", HTTPAdapter(max_retries=retries))
            resp = sess.get(
                f"{self.base_url}{file_url.replace(self.base_url, '')}",
                verify=False,
                stream=True,
                auth=self.auth,
            )
            resp.raise_for_status()
            return resp

from unittest.mock import patch, MagicMock
import pytest
from django.http import JsonResponse
from rest_framework.test import APIRequestFactory
from rest_framework import status
from your_app.views import RootCauseViewSet
from your_app.models import RootCause
from your_app.serializers import RootCauseSerializer
from your_app.utils import ensure_mongo_connection


@pytest.fixture
def api_request_factory() -> APIRequestFactory:
    """Fixture to provide an API request factory for testing.

    Returns:
        APIRequestFactory: Factory to create API requests.
    """
    return APIRequestFactory()


@pytest.fixture
def mock_ensure_mongo_connection() -> MagicMock:
    """Fixture to mock the ensure_mongo_connection utility.

    Returns:
        MagicMock: A mock instance of ensure_mongo_connection.
    """
    with patch("your_app.views.ensure_mongo_connection") as mock_connection:
        yield mock_connection


@pytest.fixture
def mock_super_dispatch() -> MagicMock:
    """Fixture to mock the super().dispatch call.

    Returns:
        MagicMock: A mock instance of the parent dispatch method.
    """
    with patch("your_app.views.super") as mock_super:
        mock_dispatch = MagicMock()
        mock_super.return_value.dispatch = mock_dispatch
        yield mock_dispatch


def test_dispatch_mongo_unavailable(
    api_request_factory: APIRequestFactory,
    mock_ensure_mongo_connection: MagicMock,
) -> None:
    """Test dispatch when MongoDB connection is unavailable.

    Args:
        api_request_factory (APIRequestFactory): API request factory for test requests.
        mock_ensure_mongo_connection (MagicMock): Mocked ensure_mongo_connection function.
    """
    mock_ensure_mongo_connection.return_value = False
    view = RootCauseViewSet()
    request = api_request_factory.get("/root-causes/")
    response = view.dispatch(request)
    assert isinstance(response, JsonResponse)
    assert response.status_code == status.HTTP_500_INTERNAL_SERVER_ERROR
    assert response.json() == {
        "error": "MongoDB is currently unavailable, unable to get root cause patterns!"
    }
    mock_ensure_mongo_connection.assert_called_once()


def test_dispatch_mongo_available(
    api_request_factory: APIRequestFactory,
    mock_ensure_mongo_connection: MagicMock,
    mock_super_dispatch: MagicMock,
) -> None:
    """Test dispatch when MongoDB connection is available.

    Args:
        api_request_factory (APIRequestFactory): API request factory for test requests.
        mock_ensure_mongo_connection (MagicMock): Mocked ensure_mongo_connection function.
        mock_super_dispatch (MagicMock): Mocked super().dispatch call.
    """
    mock_ensure_mongo_connection.return_value = True
    view = RootCauseViewSet()
    request = api_request_factory.get("/root-causes/")
    view.dispatch(request)
    mock_ensure_mongo_connection.assert_called_once()
    mock_super_dispatch.assert_called_once_with(request)


@pytest.mark.django_db
def test_model_viewset_operations() -> None:
    """Test CRUD operations for the RootCauseViewSet using the ModelViewSet."""
    # Create a test instance
    instance = RootCause.objects.create(name="Test Root Cause")
    serializer = RootCauseSerializer(instance)
    assert serializer.data["name"] == "Test Root Cause"

    # Retrieve the instance using the queryset
    view = RootCauseViewSet()
    queryset = view.get_queryset()
    assert instance in queryset

    # Test pagination class
    assert view.pagination_class is not None




import pytest
from unittest.mock import patch, MagicMock
from requests import Response, Session
from requests.adapters import HTTPAdapter
from urllib3.util.retry import Retry
from your_module import YourClass  # Replace `YourClass` with the actual class name

@pytest.fixture
def mock_requests_session() -> MagicMock:
    """Fixture to mock the requests.Session object.

    Returns:
        MagicMock: A mocked requests.Session object.
    """
    with patch("your_module.requests.Session") as mock_session:
        yield mock_session


@pytest.fixture
def mock_http_adapter() -> MagicMock:
    """Fixture to mock the HTTPAdapter object.

    Returns:
        MagicMock: A mocked HTTPAdapter object.
    """
    with patch("your_module.HTTPAdapter") as mock_adapter:
        yield mock_adapter


@pytest.fixture
def mock_retry() -> MagicMock:
    """Fixture to mock the Retry object.

    Returns:
        MagicMock: A mocked Retry object.
    """
    with patch("your_module.Retry") as mock_retry:
        yield mock_retry


@pytest.fixture
def test_class_instance() -> YourClass:
    """Fixture to create an instance of the class under test.

    Returns:
        YourClass: An instance of the class containing the method to test.
    """
    return YourClass(base_url="https://example.com", auth=("user", "pass"))


def test_stream_file_by_url_success(
    test_class_instance: YourClass,
    mock_requests_session: MagicMock,
    mock_http_adapter: MagicMock,
    mock_retry: MagicMock,
) -> None:
    """Test streaming a file successfully.

    Args:
        test_class_instance (YourClass): Instance of the class containing the method.
        mock_requests_session (MagicMock): Mocked requests.Session object.
        mock_http_adapter (MagicMock): Mocked HTTPAdapter object.
        mock_retry (MagicMock): Mocked Retry object.
    """
    mock_response = MagicMock(spec=Response)
    mock_response.status_code = 200
    mock_response.raise_for_status.return_value = None
    mock_requests_session.return_value.get.return_value = mock_response

    file_url = "/path/to/file"
    response = test_class_instance.stream_file_by_url(file_url)

    # Assertions
    assert response == mock_response
    mock_requests_session.assert_called_once()
    mock_http_adapter.assert_called_once_with(max_retries=mock_retry.return_value)
    mock_requests_session.return_value.mount.assert_called_once_with(
        f"{test_class_instance.base_url}/", mock_http_adapter.return_value
    )
    mock_requests_session.return_value.get.assert_called_once_with(
        f"{test_class_instance.base_url}{file_url}",
        verify=False,
        stream=True,
        auth=test_class_instance.auth,
    )


def test_stream_file_by_url_http_error(
    test_class_instance: YourClass,
    mock_requests_session: MagicMock,
) -> None:
    """Test streaming a file when an HTTP error occurs.

    Args:
        test_class_instance (YourClass): Instance of the class containing the method.
        mock_requests_session (MagicMock): Mocked requests.Session object.
    """
    mock_response = MagicMock(spec=Response)
    mock_response.raise_for_status.side_effect = Exception("HTTP Error")
    mock_requests_session.return_value.get.return_value = mock_response

    file_url = "/path/to/file"

    with pytest.raises(Exception, match="HTTP Error"):
        test_class_instance.stream_file_by_url(file_url)

    mock_requests_session.return_value.get.assert_called_once_with(
        f"{test_class_instance.base_url}{file_url}",
        verify=False,
        stream=True,
        auth=test_class_instance.auth,
    )


def test_stream_file_by_url_invalid_url(
    test_class_instance: YourClass,
    mock_requests_session: MagicMock,
) -> None:
    """Test streaming a file with an invalid URL.

    Args:
        test_class_instance (YourClass): Instance of the class containing the method.
        mock_requests_session (MagicMock): Mocked requests.Session object.
    """
    invalid_url = "/invalid/url"
    mock_requests_session.return_value.get.side_effect = ValueError("Invalid URL")

    with pytest.raises(ValueError, match="Invalid URL"):
        test_class_instance.stream_file_by_url(invalid_url)

    mock_requests_session.return_value.get.assert_called_once_with(
        f"{test_class_instance.base_url}{invalid_url}",
        verify=False,
        stream=True,
        auth=test_class_instance.auth,
    )