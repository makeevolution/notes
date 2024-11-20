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
