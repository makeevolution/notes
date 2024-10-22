3# notes

## Notes on shortcuts/lessons learned for different topics
// Helper function to push table content
const pushTableContent = (testCase: TestCaseType, alwaysLatest: boolean) => {
    this.tableContents.push({
        checked: this.testCaseIdsToExecute.some((checked_test_case_id) => checked_test_case_id === testCase.id),
        scenario: testCase?.scenario,
        scenario_version: testCase?.scenario_version,
        test_case: testCase?.testcase,
        variant: testCase?.variant as string,
        param_id: Number(testCase?.id),
        alwaysLatest
    });
};

if (this.testCaseIdsInSuiteAlwaysLatest.includes(testCase.id!) && this.testCaseIdsInSuiteSpecificVersions.includes(testCase.id!)) {
    // Push twice: one for 'alwaysLatest' as true, another as false
    pushTableContent(testCase, true);  // alwaysLatest = true
    pushTableContent(testCase, false); // alwaysLatest = false

    // Explanation in comments about why two entries are needed
    // A test case can be in both specific versions and always latest in certain edge cases,
    // so we push both entries to differentiate them during editing/removal in the suite.
} else if (this.testCaseIdsInSuiteAlwaysLatest.includes(testCase.id!)) {
    // Only for 'alwaysLatest' cases
    pushTableContent(testCase, true);
} else {
    // Only for specific version cases
    pushTableContent(testCase, false);
}
