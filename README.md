3# notes

## Notes on shortcuts/lessons learned for different topics
 getItems(page = 1, itemsPerPage = 10): Observable<string[]> {
        this.http.get('assets/scenario_Overlay_slit_fingerprint_control_FlexOVO_HD_DLM_2100_LCP_test_oct_23_11.log', {responseType: 'text'}).
            subscribe(data => {
            data.split(/r?\n/).forEach(x => {
                    console.log(x)
                    const startIndex = (page - 1) * itemsPerPage;
                    const endIndex = startIndex + itemsPerPage;
                    const items = [""];
                    for (let i = startIndex; i < endIndex; i++) {
                        if (i < this.totalItems) {
                    this.logs!.push(x)
                        }
                    }
                    return of(items).pipe(delay(500));
                }
            );
        })
    }
