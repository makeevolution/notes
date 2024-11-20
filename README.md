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





    import pytest
from unittest.mock import MagicMock
from django.db import transaction

# Assuming the class is in a file named `services.py` and imported as follows:
# from services import TestCaseService


def test_remove_stale_tc_normal_execution(mocker):
    """Test normal execution where stale test case and its dependencies are removed."""
    # Mock dependencies
    mock_hhhhh = mocker.patch("services.hhhhh")
    mock_logger = mocker.patch("services.logger")

    # Mock objects and behavior
    mock_tc_instance = MagicMock()
    mock_hhhhh.objects.get.return_value = mock_tc_instance
    mock_tce_instance = MagicMock()
    mock_tc_instance.testcaseexecution_set.order_by.return_value = [mock_tce_instance]
    mock_tce_instance.testcycle.testcaseexecution_set.get.return_value = mock_tce_instance

    # Call the method
    stale_latest = [("key1", "value1")]
    TestCaseService.remove_stale_tc(stale_latest)

    # Assertions
    mock_hhhhh.objects.get.assert_called_once_with(**dict(stale_latest))
    mock_logger.info.assert_any_call(f"Processing tc '{mock_tc_instance.id}, {mock_tc_instance}' by removing dependencies")
    mock_tc_instance.tags.clear.assert_called_once()
    mock_tce_instance.testcycle.testcaseexecution_set.get.assert_called_once_with(id=mock_tce_instance.id)
    mock_tce_instance.delete.assert_called_once()
    mock_tc_instance.delete.assert_called_once()
    mock_logger.info.assert_any_call(f"Removing stale tc '{mock_tc_instance}'")


def test_remove_stale_tc_transaction_error(mocker):
    """Test when a transaction error occurs."""
    mock_hhhhh = mocker.patch("services.hhhhh")
    mock_logger = mocker.patch("services.logger")

    # Simulate a transaction error
    mock_hhhhh.objects.get.side_effect = transaction.TransactionManagementError("Transaction error")

    # Call the method and expect an exception
    stale_latest = [("key1", "value1")]
    with pytest.raises(transaction.TransactionManagementError, match="Transaction error"):
        TestCaseService.remove_stale_tc(stale_latest)

    # Assertions
    mock_hhhhh.objects.get.assert_called_once_with(**dict(stale_latest))
    mock_logger.info.assert_not_called()


def test_remove_stale_tc_empty_dependencies(mocker):
    """Test execution when no dependencies are present."""
    mock_hhhhh = mocker.patch("services.hhhhh")
    mock_logger = mocker.patch("services.logger")

    # Mock objects and behavior
    mock_tc_instance = MagicMock()
    mock_hhhhh.objects.get.return_value = mock_tc_instance
    mock_tc_instance.testcaseexecution_set.order_by.return_value = []

    # Call the method
    stale_latest = [("key1", "value1")]
    TestCaseService.remove_stale_tc(stale_latest)

    # Assertions
    mock_hhhhh.objects.get.assert_called_once_with(**dict(stale_latest))
    mock_logger.info.assert_any_call(f"Processing tc '{mock_tc_instance.id}, {mock_tc_instance}' by removing dependencies")
    mock_tc_instance.tags.clear.assert_called_once()
    mock_tc_instance.delete.assert_called_once()


def test_remove_stale_tc_missing_test_case(mocker):
    """Test execution when the test case is not found."""
    mock_hhhhh = mocker.patch("services.hhhhh")
    mock_logger = mocker.patch("services.logger")

    # Simulate a missing test case
    mock_hhhhh.objects.get.side_effect = hhhhh.DoesNotExist("Test case not found")

    # Call the method and expect an exception
    stale_latest = [("key1", "value1")]
    with pytest.raises(hhhhh.DoesNotExist, match="Test case not found"):
        TestCaseService.remove_stale_tc(stale_latest)

    # Assertions
    mock_hhhhh.objects.get.assert_called_once_with(**dict(stale_latest))
    mock_logger.info.assert_not_called()

    # Assertions
    assert response.status_code == status.HTTP_202_ACCEPTED
    mock_logger.warning.assert_called_once_with(
        "There are 1 xxx, but sfsfsf db has more: 2! source of truth; deleting these xxx from db"
    )
    
