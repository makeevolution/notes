# General

- Good explanation on deadlock in Async Await and how to mitigate https://www.youtube.com/watch?v=I4cnX_odC1M
  This is why therefore you shouldn't access Result directly like the following:
  ```
  public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            using (var reader = new StreamReader(bindingContext.ActionContext.HttpContext.Request.Body))
            {
                var body = reader.ReadToEndAsync().Result;
            }
        }
  ```
  Once the executing thread hits the `await` keyword in `ReadToEndAsync`, it will return to `reader.ReadToEndAsync()` and then wait for `.Result`. But since now there is no one processing the return value of the `await`, the `reader.ReadToEndAsync().Result` will never complete, and the whole thing hangs (deadlock)

  However, this deadlock won't occur in ASP.NET Core, since in this framework, it will automatically assign a thread to process the await once the current executing thread goes back to the `reader.ReadToEndAsync().Result` line, thus no deadlock occurs! https://www.reddit.com/r/csharp/comments/130e4c6/doubt_in_chatgpt_answer_on_deadlock/
  
# Notes on async await in detail
## Look at the code in each folder and read the comments there

