- Good explanation on deadlock in Async Await and how to mitigate https://www.youtube.com/watch?v=I4cnX_odC1M
  This is why therefore you shouldn't access Result directly like the following:
  ```
  public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            using (var reader = new StreamReader(bindingContext.ActionContext.HttpContext.Request.Body))
            {
                var body = reader.ReadToEndAsync().Result;
            }
  ```
  The await in `ReadToEndAsync` will make the current executing thread go back to `reader.ReadToEndAsync().Result`. But since now there is no one processing
  the result of the `await`, the `reader.ReadToEndAsync().Result` will never complete, and the whole thing hangs (deadlock)
