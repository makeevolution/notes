3# notes

## Notes on shortcuts/lessons learned for different topics
    ngOnInit(): void {
        this.loading = true;
        this.populateTableWithData();
    }

    populateTableWithData(): void {
        this.loading = true
        this.api.getRootCauses().subscribe({
            next: (response: RootCauseResponse) => {
                this.response = response;
            },
            error: (error: Error) => {
                console.error(error)
                this.loading = false
            },
            complete: () => {
                this.getRootCausesComplete();
            },
        });
    }

    getRootCausesComplete(): void {
        this.tableContents = [];
        this.response.results.forEach((rootCause: RootCause, index: number) => {
            this.tableContents.push({
                param_id: index,
                root_cause: rootCause.root_cause,
                action: {
                    delete: true,
                },
            });
        });
        this.loading = false;
    }

    openAddRootCauseDialog(): void {
        this.dialog
            .open(AddRootCauseComponent)
            .afterClosed()
            .subscribe((action: DialogData) => {
                if (action && action.name) {
                    this.addRootCause(action);
                }
            });
    }

    addRootCause(data: DialogData): void {
        this.loading = true;
        const root_cause: RootCause = {
            // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
            root_cause: data.name!
        };
        this.api.createRootCause(root_cause).subscribe({
            error: (error: Error) => {
                console.error(error);
                this.loading = false;
                this.notification.show(Messages.AddRootCauseFailed, Action.Close, Status.Fail);
            },
            complete: () => {
                this.populateTableWithData();
                this.notification.show(Messages.AddRootCauseSuccess, Action.Close, Status.Success);
            },
        });
    }
    deleteRootCause(data: DialogData): void {
        this.loading = true
        const rootCauseToDelete = this.tableContents.at(Number(data.id))?.root_cause
        if (!rootCauseToDelete) {
            this.notification.show(Messages.DeleteRootCauseFailed, Action.Close, Status.Fail);
            return
        }
        this.api.deleteRootCause(rootCauseToDelete).subscribe({
            error: (error: Error) => {
                console.error(error);
                this.loading = false;
                this.notification.show(Messages.DeleteRootCauseFailed, Action.Close, Status.Fail);
            },
            complete: () => {
                this.populateTableWithData();
                this.notification.show(Messages.DeleteRootCauseSuccess, Action.Close, Status.Success);
            },
        });
    
