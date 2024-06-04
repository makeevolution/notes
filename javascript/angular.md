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
