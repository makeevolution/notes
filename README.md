3# notes

## Notes on shortcuts/lessons learned for different topics
    @action(methods=["DELETE"], detail=False, url_path="stale-xxx")
    def stale_xxx(self, _: Request) -> typing.Union[Response, JsonResponse]:  # noqa: WPS210, WPS212
        resp_msg = "Deletion of stale xxx in db is complete!"
        testcases = fsfsfs().get_all_testcases()
        if len(testcases) < TestCase.objects.count():
            logger.warning(
                f"There are {len(xxx)} xxx"
                + f", but sfsfsf db has more: {TestCase.objects.count()}! "
                + "source of truth; deleting these xxx from db",
            )
            # Find the stale xxx in db
            sdfsdf_set = {tuple(record.items()) for record in xxx}
            db_set = {
                tuple(tc.items()) for tc in TestCase.objects.values(aaaaaaaaaaaaaaaaaa, xxxxx, xxxxxx, yyyyyyyyy)
            }
            differences: set[tuple] = db_set - artifactory_set
            for stale_tc in differences:
                try:
                    TestCaseService().remove_stale_tc(stale_tc)
                except Exception as exc:  # noqa: B902 pylint: disable=broad-exception-caught
                    logger.error(traceback.format_exc())
                    return JsonResponse(
                        {DETAIL_STRING: exc},
                        status=status.HTTP_500_INTERNAL_SERVER_ERROR,
                        safe=False,
                    )
        return Response(
            {DETAIL_STRING: resp_msg},
            status=status.HTTP_202_ACCEPTED,
        )



    @classmethod
    def remove_stale_tc(cls, stale_latest: typing.Iterable[tuple[str, str]]) -> None:  # noqa: WPS213
        with transaction.atomic():
            tc = hhhhh.objects.get(**dict(stale_latest))
            logger.info(f"Processing tc '{tc.id}, {tc}' by removing dependencies")
            tc.tags.clear()
            for tce in tc.testcaseexecution_set.order_by("-run_number"):
                logger.info(f"Removing tce '{tce}' from hhhhhhhhhhh {tce.testcycle}")
                tce.testcycle.testcaseexecution_set.get(id=tce.id).delete()
                tce.testcycle.save()
                logger.info(f"Removing tce '{tce}")
                tce.delete()
            logger.info(f"Removing stale tc '{tc}")
            tc.delete()







    import pytest
from rest_framework import status
from unittest.mock import MagicMock

# Assuming the view is in a file named `views.py` and imported as follows:
# from views import YourViewClass

def test_stale_xxx_normal_execution(mocker):
    """Test normal execution where stale records are deleted successfully."""
    mock_fsfsfs = mocker.patch("views.fsfsfs")
    mock_testcase_objects = mocker.patch("views.TestCase.objects")
    mock_testcase_service = mocker.patch("views.TestCaseService")

    # Mock return values
    mock_fsfsfs.return_value.get_all_testcases.return_value = [{"key1": "value1"}]
    mock_testcase_objects.values.return_value = [{"key1": "value1"}, {"key2": "value2"}]

    # Call the function
    view = YourViewClass()
    request = MagicMock()
    response = view.stale_xxx(request)

    # Assertions
    assert response.status_code == status.HTTP_202_ACCEPTED
    assert response.data == {"detail": "Deletion of stale xxx in db is complete!"}
    mock_testcase_service.return_value.remove_stale_tc.assert_called_once_with(("key2", "value2"))


def test_stale_xxx_no_stale_records(mocker):
    """Test when no stale records are found."""
    mock_fsfsfs = mocker.patch("views.fsfsfs")
    mock_testcase_objects = mocker.patch("views.TestCase.objects")
    mock_logger = mocker.patch("views.logger")

    # Mock return values
    mock_fsfsfs.return_value.get_all_testcases.return_value = [{"key1": "value1"}]
    mock_testcase_objects.values.return_value = [{"key1": "value1"}]

    # Call the function
    view = YourViewClass()
    request = MagicMock()
    response = view.stale_xxx(request)

    # Assertions
    assert response.status_code == status.HTTP_202_ACCEPTED
    assert response.data == {"detail": "Deletion of stale xxx in db is complete!"}
    mock_logger.warning.assert_not_called()


def test_stale_xxx_error_during_deletion(mocker):
    """Test when an error occurs during the deletion of stale records."""
    mock_fsfsfs = mocker.patch("views.fsfsfs")
    mock_testcase_objects = mocker.patch("views.TestCase.objects")
    mock_testcase_service = mocker.patch("views.TestCaseService")
    mock_logger = mocker.patch("views.logger")
    mock_traceback = mocker.patch("views.traceback.format_exc", return_value="Mock Traceback")

    # Mock return values
    mock_fsfsfs.return_value.get_all_testcases.return_value = [{"key1": "value1"}]
    mock_testcase_objects.values.return_value = [{"key1": "value1"}, {"key2": "value2"}]

    # Simulate an exception during deletion
    mock_testcase_service.return_value.remove_stale_tc.side_effect = Exception("Deletion Error")

    # Call the function
    view = YourViewClass()
    request = MagicMock()
    response = view.stale_xxx(request)

    # Assertions
    assert response.status_code == status.HTTP_500_INTERNAL_SERVER_ERROR
    assert response.data == {"detail": "Deletion Error"}
    mock_logger.error.assert_called_once_with("Mock Traceback")


    def test_stale_xxx_logs_warning(mocker):
        """Test that a warning is logged when discrepancies are found."""
        mock_fsfsfs = mocker.patch("views.fsfsfs")
        mock_testcase_objects = mocker.patch("views.TestCase.objects")
        mock_logger = mocker.patch("views.logger")
    
    # Mock return values
    mock_fsfsfs.return_value.get_all_testcases.return_value = [{"key1": "value1"}]
    mock_testcase_objects.count.return_value = 2

    # Call the function
    view = YourViewClass()
    request = MagicMock()
    response = view.stale_xxx(request)

    # Assertions
    assert response.status_code == status.HTTP_202_ACCEPTED
    mock_logger.warning.assert_called_once_with(
        "There are 1 xxx, but sfsfsf db has more: 2! source of truth; deleting these xxx from db"
    )
    
