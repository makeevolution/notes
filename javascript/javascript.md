- How to kill all timers from console:
```
var highestTimeoutId = setTimeout(";");
for (var i = 0 ; i < highestTimeoutId ; i++) {
    clearTimeout(i); 
}
```

- Various notes about Rxjs with examples:
https://stackblitz.com/edit/rxjs-dwwryf?devtoolsheight=60&file=index.ts

- How to debug Angular tests using vscode:
  1. Set in your karma.conf.js
     ```
     browsers: ['ChromeDebugging'],
     customLaunchers: {
            ChromeDebugging: {
                base: 'Chrome',
                flags: ['--remote-debugging-port=9333'],
            },
        },
     ```
  2. Run your tests using `ng test` or similar, and keep it running even if it is done
  3. In .vscode, add launch config:
  ```
    {
    // Use IntelliSense to learn about possible attributes.
    // Hover to view descriptions of existing attributes.
    // For more information, visit: https://go.microsoft.com/fwlink/?linkid=830387
    "version": "0.2.0",
    "configurations": [

        {
            "type": "chrome",
            "request": "attach",
            "name": "attach karma chrome",
            "address": "127.0.0.1",
            "port": 9333,
            "pathMapping": {
                "/": "${workspaceRoot}",
                "/base": "${workspaceRoot}"
            },
            "webRoot": "${workspaceFolder}/frontend"
        }
    ]
    }
  ```
  4. Set breakpoint on test and debug
