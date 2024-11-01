3# notes

## Notes on shortcuts/lessons learned for different topics
                for tc_in_original in test_suite_to_update.test_case_ids.all():
                    for tc_in_latest in updated_latest_tcs_list:
                        if tc_in_latest.scenario == tc_in_original.scenario \
                            and tc_in_latest.testcase == tc_in_original.testcase \
                            and tc_in_latest.variant == tc_in_original.variant:
                                test_suite_to_update.test_case_ids.remove(tc_in_original)
                                test_suite_to_update.test_case_ids.add(tc_in_latest)
