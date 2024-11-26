3# notes

## Notes on shortcuts/lessons learned for different topics
xport class SomeComponent implements AfterViewInit {
    items: SafeHtml[] = [];
    isLoading = false;
    currentPage = 1;
    itemsPerPage = 50;
    lastPageButtonDisabled = true;
    totalPages!: number;

    previousMatchButtonDisabled = true;
    nextMatchButtonDisabled = false;

    matches: HTMLElement[] = [];
    currentIndex = 0;
    private subscription: Subscription = new Subscription();
    protected readonly Step = ChangePagination;
    tceId!: number;

    @ViewChild('logLines') content!: ElementRef<HTMLDivElement>;
    // it will be called when this component gets initialized.
    loadInitialData = () => {
        this.loadDataForPage();
        this.loadTotalNoOfPages();
    };

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
    
    loadTotalNoOfPages = () => {
        this.api.getExecutionLogLength(this.tceId).subscribe({
            next: (response) => {
                console.log(response);
                this.totalPages = Math.floor(Number(response) / this.itemsPerPage) + 1;
                this.lastPageButtonDisabled = false;
            },
            error: (error) => {
                console.log(error);
            },
        });
    };

    loadDataForPage = () => {
        this.isLoading = true;
        this.api.obtainLogForTce(this.tceId, this.currentPage, this.itemsPerPage).subscribe({
            next: (response: LogFile) => {
                console.log(response);
                this.items = response.page_contents.map((x: string) => this.sanitizer.bypassSecurityTrustHtml(x));
                this.changeDetectorRef.detectChanges();
                this._initializeMatches();
                this.currentIndex = -1;
            },
            error: (err) => {
                console.log(err);
                this.isLoading = false;
            },
            complete: () => {
                this.isLoading = false;
            },
        });
    };

    async changePage(page: ChangePagination): Promise<void> {
        this.isLoading = true
        switch (page) {
            case ChangePagination.First:
                this.currentPage = 1;
                this.previousMatchButtonDisabled = true;
                this.nextMatchButtonDisabled = await this._page_contains_last_ever_match();
                break;
            case ChangePagination.Previous:
                this.currentPage -= 1;
                this.previousMatchButtonDisabled = await this._page_contains_first_ever_match();
                this.nextMatchButtonDisabled = await this._page_contains_last_ever_match();
                break;
            case ChangePagination.Next:
                this.currentPage += 1;
                this.previousMatchButtonDisabled = await this._page_contains_first_ever_match();
                this.nextMatchButtonDisabled = await this._page_contains_last_ever_match();
                break;
            case ChangePagination.Last:
                this.currentPage = this.totalPages;
                this.previousMatchButtonDisabled = await this._page_contains_first_ever_match();
                this.nextMatchButtonDisabled = await this._page_contains_last_ever_match();
                break;
            default:
                return;
        }
        this.loadDataForPage();
    }

    async scrollToNext(): Promise<void> {
        if (this.matches.length) {
            if (this.currentIndex === this.matches.length - 1) {
                if (await this._page_contains_last_ever_match()) {
                    this.nextMatchButtonDisabled = true;
                    return;
                }
                this._go_to_later_page_containing_match();
                return;
            }
            // Increment the index since we're not at the last match yet
            const currentMatch = this.matches[this.currentIndex]
            if (currentMatch && currentMatch.classList.contains('root-cause-under-view')) {
                currentMatch.classList.remove('root-cause-under-view')
            }
            this.currentIndex++;
            if (this.currentPage == this.totalPages) {
                this.isLoading = true
                this.nextMatchButtonDisabled = await this._page_contains_last_ever_match();
                this.isLoading = false
            }
            const nextMatch = this.matches[this.currentIndex];
            this._scrollToElement(nextMatch);
        } else {
            this._go_to_later_page_containing_match();
        }
        this.previousMatchButtonDisabled = this.currentIndex == 0 && (await this._page_contains_first_ever_match());
        this.nextMatchButtonDisabled =
            this.currentIndex == this.matches.length - 1 && (await this._page_contains_last_ever_match());
    }

    async scrollToPrevious(): Promise<void> {
        this.nextMatchButtonDisabled = false;
        if (this.matches.length) {
            if (this.currentIndex <= 0) {
                if (await this._page_contains_first_ever_match()) {
                    this.previousMatchButtonDisabled = true;
                    return;
                }
                this._go_to_prior_page_containing_match();
                return;
            }
            // Decrement the index if we're not at the first match
            const currentMatch = this.matches[this.currentIndex]
            if (currentMatch && currentMatch.classList.contains('root-cause-under-view')) {
                currentMatch.classList.remove('root-cause-under-view')
            }
            this.currentIndex--;
            const prevMatch = this.matches[this.currentIndex];
            this._scrollToElement(prevMatch);
        } else {
            this._go_to_prior_page_containing_match();
        }
        this.previousMatchButtonDisabled = this.currentIndex <= 0 && (await this._page_contains_first_ever_match());
    }

    async goToFirstMatch(): Promise<void> {
        this.changePage(this.Step.First);
        this.nextMatchButtonDisabled =
            (await this._page_contains_first_ever_match()) && (await this._page_contains_last_ever_match());
        await this.scrollToNext();
        this.previousMatchButtonDisabled = true;
    }

    async goToLastMatch(): Promise<void> {
        await this._goToLastMatch();
        this.nextMatchButtonDisabled = true;
        this.previousMatchButtonDisabled = false;
    }

    async _goToLastMatch(): Promise<void> {
        try {
            this.isLoading = true;
            const response = await lastValueFrom(
                this.api.obtainLogForTce(this.tceId, this.currentPage, this.itemsPerPage, false, false, false, true),
            );
            this.items = response.page_contents.map((x: string) => this.sanitizer.bypassSecurityTrustHtml(x));
            this.currentPage = response.page_no;
            this.changeDetectorRef.detectChanges();
            this._initializeMatches();
            this.currentIndex = this.matches.length - 1;
            console.log(this.currentIndex);
            this._scrollToElement(this.matches.slice(-1)[0]);
            this.isLoading = false;
        } catch (err) {
            console.log(err);
            this.isLoading = false;
        }
    }

    private _page_contains_last_ever_match(): Promise<boolean> {
        return new Promise((resolve, reject) => {
            this.api.obtainLogForTce(this.tceId, this.currentPage, this.itemsPerPage, false, true).subscribe({
                next: (response: LogFile) => {
                    resolve(response.page_contents.length == 0);
                },
                error: (err) => {
                    console.log(err);
                    reject(err);
                },
            });
        });
    }

    private _page_contains_first_ever_match(): Promise<boolean> {
        return new Promise((resolve, reject) => {
            this.api.obtainLogForTce(this.tceId, this.currentPage, this.itemsPerPage, true).subscribe({
                next: (response: LogFile) => {
                    resolve(response.page_contents.length == 0);
                },
                error: (err) => {
                    console.log(err);
                    reject(err);
                },
            });
        });
    }

    private _go_to_later_page_containing_match() {
        this.isLoading = true;
        this.api.obtainLogForTce(this.tceId, this.currentPage, this.itemsPerPage, false, true).subscribe({
            next: (response: LogFile) => {
                this.items = response.page_contents.map((x: string) => this.sanitizer.bypassSecurityTrustHtml(x));
                this.currentPage = response.page_no;
                this.currentIndex = 0;
                this.changeDetectorRef.detectChanges();
                this._initializeMatches();
                this._scrollToElement(this.matches[0]);
            },
            error: (err) => {
                console.log(err);
                this.isLoading = false;
            },
            complete: () => {
                this.isLoading = false;
            },
        });
    }

    private _go_to_prior_page_containing_match() {
        this.isLoading = true;
        this.api.obtainLogForTce(this.tceId, this.currentPage, this.itemsPerPage, true, false).subscribe({
            next: (response: LogFile) => {
                this.isLoading = true;
                const res = response.page_contents.map((x: string) => this.sanitizer.bypassSecurityTrustHtml(x));
                if (this.items.length == 0) {
                    return;
                }
                this.items = res;
                this.currentPage = response.page_no;
                this.changeDetectorRef.detectChanges();
                this._initializeMatches();
                this.currentIndex = this.matches.length - 1;
                this._scrollToElement(this.matches.slice(-1)[0]);
            },
            error: (err) => {
                console.log(err);
                this.isLoading = false;
            },
            complete: () => {
                this.isLoading = false;
            },
        });
    }

    private _initializeMatches(): void {
        this.matches = Array.from(this.content.nativeElement.querySelectorAll('.root-cause'));
    }

    private _scrollToElement(element: HTMLElement): void {
        this.isLoading = true
        element.scrollIntoView({
            block: 'center',
            behavior: 'smooth',
        });
        element.classList.add("root-cause-under-view")
        this.isLoading = false
    }
}

