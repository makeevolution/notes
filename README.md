3# notes

## Notes on shortcuts/lessons learned for different topics
if (this.testCaseIdsInSuiteAlwaysLatest.includes(testCase.id!) && this.testCaseIdsInSuiteSpecificVersions.includes(testCase.id!)) {
                this.tableContents.push({
                    checked: this.testCaseIdsToExecute.some((checked_test_case_id) => checked_test_case_id === testCase.id),
                    scenario: testCase?.scenario,
                    scenario_version: testCase?.scenario_version,
                    test_case: testCase?.testcase,
                    variant: testCase?.variant as string,
                    param_id: Number(testCase?.id),
                    alwaysLatest: true,
                    // alwaysLatest tabelContent here is needed since a test case can be in both
                    // test_cases_specific_versions and test_cases_always_latest, and so need to differentiate.
                    // A test case can be in both in the (very edge case) situation:
                    // Given a test case with version 100, user wants to have both this exact specific_scenario and also Latest in the suite,
                    // and they add this test case to the suite at the moment in time where test case has scenario version 100 is currently Latest (i.e. highest in Artifactory).
                    // This case shouldn't happen since users very rarely/ever actually select specific scenario when making
                    // suites...they even entertained the idea to remove this specific scenario thing...
                    // This differentiation field is required so that when they select edit test case in suite, we know which one they want to remove.
                    // During populate from artifactory, all test cases in the always_latest column will be updated with new ids, effectively removing this
                    // duplication.
                });
                this.tableContents.push({
                    checked: this.testCaseIdsToExecute.some((checked_test_case_id) => checked_test_case_id === testCase.id),
                    scenario: testCase?.scenario,
                    scenario_version: testCase?.scenario_version,
                    test_case: testCase?.testcase,
                    variant: testCase?.variant as string,
                    param_id: Number(testCase?.id),
                    alwaysLatest: false,
                });
            } else if (this.testCaseIdsInSuiteAlwaysLatest.includes(testCase.id!)) {
                this.tableContents.push({
                    checked: this.testCaseIdsToExecute.some((checked_test_case_id) => checked_test_case_id === testCase.id),
                    scenario: testCase?.scenario,
                    scenario_version: testCase?.scenario_version,
                    test_case: testCase?.testcase,
                    variant: testCase?.variant as string,
                    param_id: Number(testCase?.id),
                    alwaysLatest: true
                });
            } else {
                this.tableContents.push({
                    checked: this.testCaseIdsToExecute.some((checked_test_case_id) => checked_test_case_id === testCase.id),
                    scenario: testCase?.scenario,
                    scenario_version: testCase?.scenario_version,
                    test_case: testCase?.testcase,
                    variant: testCase?.variant as string,
                    param_id: Number(testCase?.id),
                    alwaysLatest: false
                });
            }
