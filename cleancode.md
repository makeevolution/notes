https://github.com/thangchung/clean-code-dotnet
https://github.com/zedr/clean-code-python

| Group                                        | Role    | Permissions (unused yet) | Frontend UI tabs that should be seen     | Test suites and executions that can be seen                                                                                                           |
|----------------------------------------------|---------|--------------------------|------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------|
| VFI                                          | Admin   |                          | Define, execute and admin tab            | All                                                                                                                                                      |
| VFAB                                         | VFAdmin |                          | Define, execute tabs                     | All                                                                                                                                                      |
| VFOD (i.e. a group created for Non VF users) | VFUser  |                          | Execute test suite and view all jobs tab | Only suites whose group membership is the same as the user, and only executions that are part of a test suite, whose group membership is the same as the user |

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
