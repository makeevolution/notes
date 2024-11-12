3# notes

## Notes on shortcuts/lessons learned for different topics
  getItems(page = 1, itemsPerPage = 10): Observable<string[]> {
    if (this.logs.length === 0) {
      // If logs are not loaded, fetch the file first
      return this.http.get('assets/scenario_Overlay_slit_fingerprint_control_FlexOVO_HD_DLM_2100_LCP_test_oct_23_11.log', {
        responseType: 'text'
      }).pipe(
        map(data => {
          // Split file content into lines
          this.logs = data.split(/\r?\n/);
          this.totalItems = this.logs.length;
          return this.paginate(page, itemsPerPage);
        }),
        delay(500) // Simulate a delay for demonstration
      );
    } else {
      // If logs are already loaded, simply paginate
      return of(this.paginate(page, itemsPerPage)).pipe(delay(500));
    }
  }

  // Helper method to paginate logs
  private paginate(page: number, itemsPerPage: number): string[] {
    const startIndex = (page - 1) * itemsPerPage;
    const endIndex = startIndex + itemsPerPage;
    return this.logs.slice(startIndex, endIndex);
  }
}
