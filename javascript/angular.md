- remove all instlations of node and npm https://stackoverflow.com/questions/57957973/npm-cannot-find-module-semver-after-reinstall

- Stop HTTP requests when route change (i.e. component is destroyed). If you don't unsubscribe on your API calls, if you change the page while there is still an API call, the server will still process this request unnecessarily and cause performance issues! Unsubscribe by:

```
export class Test implements OnInit, OnDestroy{
    ngUnsubscribe: Subject<void> = new Subject<void>()

    constructor(
      private api: ApiService
    ) {
    }

    ngOnInit() {
        this.api.SomeApiEndpointWithLongResponseTime().pipe(takeUntil(ngUnsubscribe)).subscribe( () => { ...... } )
    }

    ngOnDestroy(){
       this.ngUnsubscribe.next()
          this.ngUnsubscribe.complete()
     }
}
```

- How to define and use directives to hide/show stuff in Angular 16: `https://www.youtube.com/watch?v=1lOA3Opkw3o`. How to extend this to allow conditionals to the directive: `https://stackoverflow.com/a/77406436`

- Mocking a behaviour subject: Say you have this service
```
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { AccessTokenResponse } from '../models';
import { jwtDecode } from 'jwt-decode';
@Injectable({ providedIn: 'root' })
export class UsernameService {
    subject = new BehaviorSubject(this.username);

    set username(username) {
        this.subject.next(username);
        localStorage.setItem('username', username as string);
    }

    get username() {
        const token = jwtDecode(localStorage.getItem('access_token')!) as AccessTokenResponse
        return token.username
    }
}
```
Then you will use it in a component e.g. `this.usernameService.subject.subscribe(... `.
To mock this call when testing that component, it's different to a function, since subject is a property. To mock it you need to do:
```
        usernameService = jasmine.createSpyObj<UsernameService>('UsernameService',
            [], {'subject': new BehaviorSubject('somefakeusername')}
        )
```
- Jasmine change ret val of spied function multiple times https://stackoverflow.com/questions/56118277/how-to-fix-function-has-already-been-spied-on-error-in-jasmine

- In Jasmine, if you want to mock a dependency, you need to call it at least once in the beforeEach before you can use it/redefine it somewhere else! See the comment below in the `beforeEach` function!
- Example also on how to mock a matdialog `open.afterclosed` (i.e. how to mock chained calls) is in below
```
describe('MyObjectPatternsComponent', () => {
    let component: MyObjectPatternsComponent;
    let fixture: ComponentFixture<MyObjectPatternsComponent>;
    let apiService: jasmine.SpyObj<ApiService>;
    let mockDialog: jasmine.SpyObj<MatDialog>;

    const mockResponse: MyObjectResponse = { count: 1, results: [{ root_cause: 'Test Cause' }] };
    beforeEach(() => {
        apiService = jasmine.createSpyObj<ApiService>('ApiService', [
            'createMyObject',
            'deleteMyObject',
            'getMyObjects',
        ]);
        mockDialog = jasmine.createSpyObj('MatDialog', ['open']);
        TestBed.configureTestingModule({
            imports: [MyObjectPatternsComponent, HttpClientModule, RouterTestingModule, BrowserAnimationsModule],
            providers: [
                { provide: ApiService, useValue: apiService },
                { provide: MatDialog, useValue: mockDialog },

                {
                    provide: ActivatedRoute,
                    useValue: {
                        params: of({}),
                    },
                },
                MatSnackBar,
            ],
        });
        // YOU NEED TO DO THE THREE CALLS BELOW TO INITIALIZE THE STUBS!
        apiService.getMyObjects.and.returnValue(of(mockResponse));
        apiService.createMyObject.and.returnValue(of());
        apiService.deleteMyObject.and.returnValue(
            of({
                count: 1,
                results: [],
            }),
        );
        const dialogRefSpy = jasmine.createSpyObj({ afterClosed: of({ name: 'New Cause' }) });
        fixture = TestBed.createComponent(MyObjectPatternsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should call getMyObjectsComplete on observable complete', () => {
        // WITHOUT THE CALLS IN THE BEFOREEACH, THIS WILL THROW UNDEFINED READING SUBSCRIBE!
        apiService.getMyObjects.and.returnValue(of({ count: 1, results: [] }));

        spyOn(component, 'getMyObjectsComplete');

        component.populateTableWithData();

        expect(component.getMyObjectsComplete).toHaveBeenCalled();
    });

    it('should not call addMyObject if dialog closes without valid data', () => {
        // EXAMPLE ON HOW TO STUB A MOCKDIALOG OPEN METHOD'S AFTERCLOSED
        spyOn(component.dialog, 'open').and.returnValue({
            afterClosed: () => of({ somekey: 'InvalidKey' }),
        } as MatDialogRef<typeof component>);
        spyOn(component, 'addMyObject');

        component.openAddMyObjectDialog();

        expect(component.addMyObject).not.toHaveBeenCalled();
    });
})
```
