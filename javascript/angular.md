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
