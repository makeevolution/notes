- How to kill all timers from console:
```
var highestTimeoutId = setTimeout(";");
for (var i = 0 ; i < highestTimeoutId ; i++) {
    clearTimeout(i); 
}
```

- Various notes with examples:
https://stackblitz.com/edit/rxjs-dwwryf?devtoolsheight=60&file=index.ts