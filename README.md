3# notes

## Notes on shortcuts/lessons learned for different topics
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
