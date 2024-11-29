3# notes

## Notes on shortcuts/lessons learned for different topics
pageContainsRootCause(isFirstRootCause: boolean): Promise<boolean> {
    return new Promise((resolve, reject) => {
        this.api.obtainLogContentForTce(
            this.tce.id,
            this.currentPage,
            this.itemsPerPage,
            isFirstRootCause, // Pass the parameter dynamically
            !isFirstRootCause // Flip the value for the second flag
        ).subscribe({
            next: (response: LogFile) => {
                resolve(response.page_contents.length === 0);
            },
            error: (err) => {
                console.error('Error fetching log content:', err);
                reject(err);
            },
        });
    });
}

pageContainsLastRootCause(): Promise<boolean> {
    return this.pageContainsRootCause(false);
}

pageContainsFirstRootCause(): Promise<boolean> {
    return this.pageContainsRootCause(true);
}





import { of, throwError } from 'rxjs';

describe('PageContainsRootCause Tests', () => {
    let component: YourComponent; // Replace with your actual component name
    let mockApiService: any;

    beforeEach(() => {
        mockApiService = {
            obtainLogContentForTce: jest.fn(),
        };

        // Mock your component and inject dependencies
        component = new YourComponent(mockApiService);
        component.tce = { id: 123 }; // Mock `tce` object
        component.currentPage = 1;
        component.itemsPerPage = 10;
    });

    it('should resolve true when pageContents is empty for first root cause', async () => {
        // Mock the API response with an empty page_contents
        mockApiService.obtainLogContentForTce.mockReturnValue(of({ page_contents: [] }));

        const result = await component.pageContainsFirstRootCause();
        expect(result).toBe(true);
        expect(mockApiService.obtainLogContentForTce).toHaveBeenCalledWith(123, 1, 10, true);
    });

    it('should resolve false when pageContents is not empty for last root cause', async () => {
        // Mock the API response with non-empty page_contents
        mockApiService.obtainLogContentForTce.mockReturnValue(of({ page_contents: ['log1'] }));

        const result = await component.pageContainsLastRootCause();
        expect(result).toBe(false);
        expect(mockApiService.obtainLogContentForTce).toHaveBeenCalledWith(123, 1, 10, false, true);
    });

    it('should reject with an error when API call fails for first root cause', async () => {
        // Mock the API error
        mockApiService.obtainLogContentForTce.mockReturnValue(throwError(() => new Error('API Error')));

        await expect(component.pageContainsFirstRootCause()).rejects.toThrow('API Error');
        expect(mockApiService.obtainLogContentForTce).toHaveBeenCalledWith(123, 1, 10, true);
    });

    it('should reject with an error when API call fails for last root cause', async () => {
        // Mock the API error
        mockApiService.obtainLogContentForTce.mockReturnValue(throwError(() => new Error('API Error')));

        await expect(component.pageContainsLastRootCause()).rejects.toThrow('API Error');
        expect(mockApiService.obtainLogContentForTce).toHaveBeenCalledWith(123, 1, 10, false, true);
    });
});

