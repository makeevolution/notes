3# notes

## Notes on shortcuts/lessons learned for different topics
    addTestCasesFromOtherPagesCombined(
    test_case_response: TestCaseGetResponse,
    show_only_latest: boolean = false,
    is_search: boolean = false
): void {
    const nextUrl = test_case_response.next;
    if (nextUrl) {
        const url = new URL(nextUrl);
        const nextPage = Number(url.searchParams.get('page'));

        // Determine which API call to make based on the `is_search` flag
        const apiCall$ = is_search
            ? this.api.getTestCasesSearchedOnBackend({
                  key_string: this.keywords as string,
                  is_or_query: this.isOrQuery as boolean,
                  sort_column: this.sortColumn as string,
                  descending: this.sortDescending as boolean,
                  page: nextPage,
                  ids: this.filteredTestCases.map((x) => x.id).join(','),
                  search_on_tags: true,
              })
            : this.api.getTestCasesByFilter(this.filters_for_api, nextPage, show_only_latest);

        apiCall$.subscribe({
            next: (next_page_response: TestCaseGetResponse) => {
                next_page_response['results'].forEach((test_case: TestCase) => {
                    this.testCasesToAddToSuite.push(test_case);
                    if (is_search) {
                        this.filteredSearchedTestCases.push(test_case);
                    } else {
                        this.filteredTestCases.push(test_case);
                    }
                });

                // Recursive call to handle pagination for subsequent pages
                this.addTestCasesFromOtherPagesCombined(next_page_response, show_only_latest, is_search);
            },
            error: /* istanbul ignore next: to be fixed in ticket XPI-8740 */ (error: Error) =>
                console.log(error),
        });
    }
}