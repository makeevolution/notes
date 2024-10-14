3# notes

## Notes on shortcuts/lessons learned for different topics
    updateTestCasesToAddToSuite(tc_id: number): void {
        const testCaseAlreadyAdded = this.testCasesToAddToSuite.some((test_case) => test_case.id === tc_id);
        if (testCaseAlreadyAdded) {
            this.testCasesToAddToSuite = this.testCasesToAddToSuite.filter((test_case) => test_case.id != tc_id);
        } else {
            if (this.isSearchOnBackendEnabled) {
                this.testCasesToAddToSuite.push(
                    this.filteredSearchedTestCases.find((test_case) => test_case.id === tc_id) as TestCase,
                );
            } else {
                this.testCasesToAddToSuite.push(
                    this.filteredTestCases.find((test_case) => test_case.id === tc_id) as TestCase,
                );
            }
            this.cdr.detectChanges();
        }
        if (this.isSearchOnBackendEnabled) {
            this.selectAllCheckboxesChecked =
                this.testCasesToAddToSuite.length === this.filteredSearchedTestCases.length;
        } else {
            this.selectAllCheckboxesChecked = this.testCasesToAddToSuite.length === this.filteredTestCases.length;
        }
        this.enableAddTestCasesToSuiteButton();
    }





updateTestCasesToAddToSuite(tc_id: number): void {
    const testCaseAlreadyAdded = this.testCasesToAddToSuite.some((test_case) => test_case.id === tc_id);

    if (testCaseAlreadyAdded) {
        this.testCasesToAddToSuite = this.testCasesToAddToSuite.filter((test_case) => test_case.id !== tc_id);
    } else {
        const sourceTestCases = this.isSearchOnBackendEnabled ? this.filteredSearchedTestCases : this.filteredTestCases;
        const testCaseToAdd = sourceTestCases.find((test_case) => test_case.id === tc_id);

        if (testCaseToAdd) {
            this.testCasesToAddToSuite.push(testCaseToAdd);
        }
    }

    this.cdr.detectChanges();

    const totalTestCases = this.isSearchOnBackendEnabled ? this.filteredSearchedTestCases.length : this.filteredTestCases.length;
    this.selectAllCheckboxesChecked = this.testCasesToAddToSuite.length === totalTestCases;

    this.enableAddTestCasesToSuiteButton();
}
