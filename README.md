3# notes

## Notes on shortcuts/lessons learned for different topics
    addTestCasesFromOtherPages(test_case_response: TestCaseGetResponse, show_only_latest: boolean): void {
        const nextUrl = test_case_response.next;
        if (nextUrl) {
            const url = new URL(nextUrl);
            const nextPage = Number(url.searchParams.get('page'));
            this.api.getTestCasesByFilter(this.filters_for_api, nextPage, show_only_latest).subscribe({
                next: (next_page_response: TestCaseGetResponse) => {
                    next_page_response['results'].forEach((test_case: TestCase) => {
                        this.testCasesToAddToSuite.push(test_case);
                        this.filteredTestCases.push(test_case);
                    });
                    this.addTestCasesFromOtherPages(next_page_response, show_only_latest);
                },
                error: /* istanbul ignore next: to be fixed in ticket XPI-8740 */ (error: Error) => console.log(error),
            });
        }
    }
 
    addTestCasesFromOtherPagesSearch(test_case_response: TestCaseGetResponse): void {
        const nextUrl = test_case_response.next;
        Iif (nextUrl) {
            const url = new URL(nextUrl);
            const nextPage = Number(url.searchParams.get('page'));
            this.api
                .getTestCasesSearchedOnBackend({
                    key_string: this.keywords as string,
                    is_or_query: this.isOrQuery as boolean,
                    sort_column: this.sortColumn as string,
                    descending: this.sortDescending as boolean,
                    page: nextPage,
                    ids: this.filteredTestCases.map((x) => x.id).join(','),
                    search_on_tags: true,
                })
                .subscribe({
                    next: (next_page_response: TestCaseGetResponse) => {
                        next_page_response['results'].forEach((test_case: TestCase) => {
                            this.testCasesToAddToSuite.push(test_case);
                            this.filteredSearchedTestCases.push(test_case);
                        });
                        this.addTestCasesFromOtherPagesSearch(next_page_response);
                    },
                    error: /* istanbul ignore next: to be fixed in ticket XPI-8740 */ (error: Error) =>
                        console.log(error),
                });
        }
    }
