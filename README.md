3# notes

## Notes on shortcuts/lessons learned for different topics
    pageContainsLastRootCause(): Promise<boolean> {
        return new Promise((resolve, reject) => {
            this.api.obtainLogContentForTce(this.tce.id, this.currentPage, this.itemsPerPage, false, true).subscribe({
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

    pageContainsFirstRootCause(): Promise<boolean> {
        return new Promise((resolve, reject) => {
            this.api.obtainLogContentForTce(this.tce.id, this.currentPage, this.itemsPerPage, true).subscribe({
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
