3# notes

## Notes on shortcuts/lessons learned for different topics
Refactoring this code focuses on improving its efficiency, readability, and maintainability without altering its functionality. Here are the main changes applied:

1. Avoid Redundant Code:

Combine similar logic into reusable helper functions (e.g., match navigation, page loading).

Reduce duplication in scrollToNext, scrollToPrevious, _go_to_later_page_containing_match, and _go_to_prior_page_containing_match.



2. Minimize Side Effects:

Handle isLoading consistently with helper functions to avoid potential race conditions or redundant updates.



3. Improve Error Handling:

Centralize error logging.



4. Leverage Async/Await for Readability:

Simplify asynchronous logic with async/await.




Here’s the refactored version:

export class SomeComponent implements AfterViewInit {
    items: SafeHtml[] = [];
    isLoading = false;
    currentPage = 1;
    itemsPerPage = 50;
    totalPages!: number;
    matches: HTMLElement[] = [];
    currentIndex = -1;

    lastPageButtonDisabled = true;
    previousMatchButtonDisabled = true;
    nextMatchButtonDisabled = false;

    @ViewChild('logLines') content!: ElementRef<HTMLDivElement>;
    private subscription = new Subscription();
    protected readonly Step = ChangePagination;
    private tceId!: number;

    constructor(
        private api: ApiService,
        private sanitizer: DomSanitizer,
        private changeDetectorRef: ChangeDetectorRef,
        @Inject(MAT_DIALOG_DATA) public data: any,
    ) {
        this.tceId = data.tceId;
    }

    ngAfterViewInit(): void {
        this.loadInitialData();
    }

    private loadInitialData(): void {
        this.loadTotalNoOfPages();
        this.loadDataForPage();
    }

    private async loadTotalNoOfPages(): Promise<void> {
        try {
            const response = await lastValueFrom(this.api.getExecutionLogLength(this.tceId));
            this.totalPages = Math.ceil(Number(response) / this.itemsPerPage);
            this.lastPageButtonDisabled = false;
        } catch (error) {
            console.error('Error loading total pages:', error);
        }
    }

    private async loadDataForPage(): Promise<void> {
        this.setLoadingState(true);
        try {
            const response: LogFile = await lastValueFrom(
                this.api.obtainLogForTce(this.tceId, this.currentPage, this.itemsPerPage),
            );
            this.items = response.page_contents.map((x) => this.sanitizer.bypassSecurityTrustHtml(x));
            this.changeDetectorRef.detectChanges();
            this.initializeMatches();
            this.currentIndex = -1;
        } catch (error) {
            console.error('Error loading page data:', error);
        } finally {
            this.setLoadingState(false);
        }
    }

    async changePage(page: ChangePagination): Promise<void> {
        this.setLoadingState(true);
        try {
            this.currentPage = this.calculateNewPage(page);
            this.previousMatchButtonDisabled = await this.pageContainsMatch(true);
            this.nextMatchButtonDisabled = await this.pageContainsMatch(false);
            await this.loadDataForPage();
        } finally {
            this.setLoadingState(false);
        }
    }

    async scrollToNext(): Promise<void> {
        await this.navigateToMatch(1);
    }

    async scrollToPrevious(): Promise<void> {
        await this.navigateToMatch(-1);
    }

    async goToFirstMatch(): Promise<void> {
        await this.changePage(this.Step.First);
        await this.scrollToNext();
        this.previousMatchButtonDisabled = true;
    }

    async goToLastMatch(): Promise<void> {
        this.setLoadingState(true);
        try {
            const response: LogFile = await lastValueFrom(
                this.api.obtainLogForTce(this.tceId, this.currentPage, this.itemsPerPage, false, false, false, true),
            );
            this.items = response.page_contents.map((x) => this.sanitizer.bypassSecurityTrustHtml(x));
            this.currentPage = response.page_no;
            this.changeDetectorRef.detectChanges();
            this.initializeMatches();
            this.currentIndex = this.matches.length - 1;
            this.scrollToElement(this.matches[this.currentIndex]);
        } finally {
            this.setLoadingState(false);
        }
    }

    private calculateNewPage(action: ChangePagination): number {
        switch (action) {
            case ChangePagination.First:
                return 1;
            case ChangePagination.Previous:
                return this.currentPage - 1;
            case ChangePagination.Next:
                return this.currentPage + 1;
            case ChangePagination.Last:
                return this.totalPages;
            default:
                return this.currentPage;
        }
    }

    private async pageContainsMatch(isFirst: boolean): Promise<boolean> {
        try {
            const response: LogFile = await lastValueFrom(
                this.api.obtainLogForTce(this.tceId, this.currentPage, this.itemsPerPage, isFirst, !isFirst),
            );
            return response.page_contents.length === 0;
        } catch (error) {
            console.error('Error checking for matches:', error);
            return false;
        }
    }

    private async navigateToMatch(direction: 1 | -1): Promise<void> {
        if (this.matches.length === 0) {
            direction === 1
                ? await this.goToNextPageContainingMatch()
                : await this.goToPreviousPageContainingMatch();
            return;
        }

        const isFirstPage = direction === -1 && this.currentIndex === 0;
        const isLastPage = direction === 1 && this.currentIndex === this.matches.length - 1;

        if (isFirstPage || isLastPage) {
            const hasMoreMatches = direction === 1
                ? await this.pageContainsMatch(false)
                : await this.pageContainsMatch(true);
            if (!hasMoreMatches) {
                direction === 1
                    ? this.nextMatchButtonDisabled = true
                    : this.previousMatchButtonDisabled = true;
                return;
            }
            direction === 1
                ? await this.goToNextPageContainingMatch()
                : await this.goToPreviousPageContainingMatch();
        } else {
            this.highlightMatch(this.currentIndex, false);
            this.currentIndex += direction;
            this.highlightMatch(this.currentIndex, true);
        }
    }

    private async goToNextPageContainingMatch(): Promise<void> {
        await this.loadPageContainingMatch(false);
    }

    private async goToPreviousPageContainingMatch(): Promise<void> {
        await this.loadPageContainingMatch(true);
    }

    private async loadPageContainingMatch(isPrevious: boolean): Promise<void> {
        this.setLoadingState(true);
        try {
            const response: LogFile = await lastValueFrom(
                this.api.obtainLogForTce(this.tceId, this.currentPage, this.itemsPerPage, isPrevious, !isPrevious),
            );
            this.items = response.page_contents.map((x) => this.sanitizer.bypassSecurityTrustHtml(x));
            this.currentPage = response.page_no;
            this.changeDetectorRef.detectChanges();
            this.initializeMatches();
            this.currentIndex = isPrevious ? this.matches.length - 1 : 0;
            this.scrollToElement(this.matches[this.currentIndex]);
        } finally {
            this.setLoadingState(false);
        }
    }

    private initializeMatches(): void {
        this.matches = Array.from(this.content.nativeElement.querySelectorAll('.root-cause'));
    }

    private scrollToElement(element: HTMLElement): void {
        element.scrollIntoView({ block: 'center', behavior: 'smooth' });
        element.classList.add('root-cause-under-view');
    }

    private highlightMatch(index: number, isHighlighted: boolean): void {
        const match = this.matches[index];
        if (match) {
            match.classList.toggle('root-cause-under-view', isHighlighted);
        }
    }

    private setLoadingState(isLoading: boolean): void {
        this.isLoading = isLoading;
    }
}

Key Changes

1. Extracted repeated logic (e.g., navigateToMatch, setLoadingState, highlightMatch).


2. Centralized match navigation with navigateToMatch.


3. Simplified error handling by using try/catch and lastValueFrom.



