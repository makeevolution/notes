# Python

- https://pytest-with-eric.com/mocking/pytest-common-mocking-problems/#Common-Problems-with-Mocking

- Check if you are running python from virtual environment: run python, import ```sys```, then ```sys.prefix```, or do ```pip -V``` or ```python -m pip -V```

-  In python your ```.py``` file is regarded as an object, with the classes in it as attributes of the object.
    For a ```.py``` file, ```sys.modules[__name__]``` returns ```<module '__main__' (built-in)>```, with each class inside the file as attributes.

    So, to check if a class exists in a .py file, you can use ```getattr(sys.modules[__name__], yourclassname)``` inside the file

- For timeouting while loops, use ```interruptingcow``` library
- ```Time.sleep``` is dependent on CPU. Any while loop needs ```time.sleep``` if you want to reduce cpu use of while loops
- What was the exception thrown by a ```try```? Use ```traceback``` module
  ```python
    try: 
       ...
    except Exception:
      logger.console(traceback.format_exc())
   ```

    Always implement this on modules/classes (e.g. code that will to be imported),  so that if python is called from robot framework (or any other parent process), you know which line is the error in

-  To print the whole exception (including the parent who called the code), ```use traceback.print_stack()``` instead. The disadvantage is that this is very verbose.
-  Assignment in python i.e. ```a = b``` does not create a copy of ```b``` for reference types; it makes a point to ```b```. If ```b``` is a dict, and ```a[key]=4``` is done, then ```b[key]``` will also be changed to 4. Be careful. use ```copy``` or ```deepcopy``` module to copy.
- Read ```pep8``` for how to write clean code
- ```functools.partial``` instead of ```lambda```, it is an option
- Easier ask for permission than forgiveness (always use ```try except``` instead of ```if```)
- Use ```itertools``` to eliminate multiple loops
- ```sorted(list, key = lambda x:x, reverse=False)``` https://holypython.com/intermediate-python-exercises/exercise-11-python-lambda/
- Note on lists: if ```list2=[1,2,3]``` then
  
  ```list2[0] = 1```
  
  ```list2[0:1] = [1]```  (i.e. it's NOT ```[1,2]```) !
  
  ```list2[0:2] = [1,2]```

  Be careful

 
- Practice enumerable magic in codewars

- utf-8 vs 16 and how unicode works (in jupyter notebook)

- dict comprehension: https://holypython.com/intermediate-python-exercises/exercise-17-python-dict-comprehensions/

- https://www.datacamp.com/community/tutorials/python-dictionary-comprehension?utm_source=adwords_ppc&utm_campaignid=898687156&utm_adgroupid=48947256715&utm_device=c&utm_keyword=&utm_matchtype=b&utm_network=g&utm_adpostion=&utm_creative=255798340456&utm_targetid=aud-299261629574:dsa-473406574715&utm_loc_interest_ms=&utm_loc_physical_ms=1010704&gclid=Cj0KCQjwxJqHBhC4ARIsAChq4asR1eyAnY1fJnPa-skSZSbHxNzetwfZ_dTqhE3bisVHmzHFXyKH4hMaAt9OEALw_wcB
Do the fahrenheit ones

- Note on iterator (i.e. return value of ```zip```, ```filter```, etc.) on how to print things from iterator e.g. the assignment below:
  
  ```result = filter(lambda x:x==3, list)```
gives an iterator stored in ```result```. To get the values you cannot do ```list(result)``` because ```result``` is an iterator. You need to do ```[e for e in result]``` or [*result] (i.e. unpacking)

    Note that once iterated through, the iterator cannot be iterated again (it is exhausted).

- item 14 of 90 effective ways to write python book: how to create a list of class instances, and use map() or sorted() or others'' key=lambda on each instance's attributes
- How to get parent dir path of current file and append to sys path:
    ```python
    parentdir = os.path.abspath(os.path.join(os.getcwd(), os.pardir))
    sys.path.append(os.path.join(parentdir))
    ```
- Check if all elements in ```list1``` is in another list ```list2``` (assuming both lists have unique elements, otherwise this method will remove duplicates!):
  ```python
  (set(list1) & set(list2)) == set(list1)
  ```
- Get elements of ```list1``` not in ```list2```:
  ```python
    ret = list1 - list2 if len(list1)>len(list2) else list2 - list1
  ```
- How string representation works:
```str``` in Python is represented in Unicode.
```UTF-8``` is an encoding standard to encode Unicode string to bytes. There are many encoding standards out there (e.g. UTF-16, ASCII, SHIFT-JIS, etc.).
When a client sends data to your Python program, they are sending a bunch of bytes not str.
Your Python program needs to correctly decode it. The client must also send you the information on the encoding used (e.g. in response class of ```requests``` library, it should be somewhere inside it).
When you want to write/pipe the string to somewhere else (e.g. print to console, write to file), you should also encode it with the same encoding!

- How to get where pip packages are installed:
   ``` 
   import site
   site.getsitepackages() # List of global package locations
   site.getusersitepackages() # String for user-specific package location
   ```
   usually its in `/usr/local/lib/python3.6/site-packages` though

- Pytest how to check if object is class and if it is subclass of exception:
  ``` inspect.isclass(myobject) and issubclass(myobject, Exception) ```

- encode decode base64 trick (at least password is not barely seen):
  ```
  word = "myword"
  word_as_bytes = bytes(word, 'utf-8')
  encoded_word = base64.b64encode(word_as_bytes)
  ```
  Now you can use this encoded word as a token-like string in config file, etc.
  To decode this token (where token is now a Python string):
  ```
  decoded_word = base64.b64decode(token)
  ```

-------------------

## pre commit stuff

- If flake8 outputs empty wemake errors, run black before it to remove extra empty lines that causes this confusing message



-------------------

## Copies and mutability

- An assignment to another variable creates only a shallow copy. The objects inside that variable are not copied!

```
l1 = [3, [66, 55, 44], (7, 8, 9)]
l2 = list(l1)      
l1.append(100)     
l1[1].remove(55)   
print('l1:', l1)
print('l2:', l2)
l2[1] += [33, 22]  
l2[2] += (10, 11)  
print('l1:', l1)
print('l2:', l2)
```
The above gives
```
l1: [3, [66, 44], (7, 8, 9), 100]
l2: [3, [66, 44], (7, 8, 9)]
l1: [3, [66, 44, 33, 22], (7, 8, 9), 100]
l2: [3, [66, 44, 33, 22], (7, 8, 9, 10, 11)]
```

Appending 100 to l1 doesn't effect l2 since l2 is a copy. However, modifying a mutable type inside l2 (i.e. the list inside it) also mutates that inside l1 since the copy is shallow. But modifying the tuple inside l2 doesn't affect the tuple in l1 since a tuple is immutable.

- The mutability of lists or dicts or reference value types are dangerous and should be watched out for. Given the following mutable type optional argument:

```

class HauntedBus:
    """A bus model haunted by ghost passengers"""

    def __init__(self, passengers=[]):  1
        self.passengers = passengers  2

    def pick(self, name):
        self.passengers.append(name)  3

    def drop(self, name):
        self.passengers.remove(name)
```

The following will unexpectedly happen

```
>>> bus2 = HauntedBus()  
>>> bus2.pick('Carrie')
>>> bus2.passengers
['Carrie']
>>> bus3 = HauntedBus()  
>>> bus3.passengers  
['Carrie']
```
This is because the default is a reference type and so every assignment will refer to the same object every time!!

-------------------

## Tricks

- To compare two YAML files easily: https://stackoverflow.com/questions/63702238/compare-keys-in-two-yaml-files-and-print-differences
  ```
  import yaml
  from deepdiff import DeepDiff

  def yaml_as_dict(my_file):
    my_dict = {}
    with open(my_file, 'r') as fp:
        docs = yaml.safe_load_all(fp)
        for doc in docs:
            for key, value in doc.items():
                my_dict[key] = value
    return my_dict

  if __name__ == '__main__':
    a = yaml_as_dict(yaml_file1)
    b = yaml_as_dict(yaml_file2)
    ddiff = DeepDiff(a, b, ignore_order=True)
    print(ddiff)
  ```

- My own generic retry function for API requests:
  use generic library:
  ```
        from requests.sessions import HTTPAdapter
        with requests.Session() as s:
            retries = Retry(total=2,  # total retries; will retry on all connection errors (e.g. ReadTimeoutError, etc.) and response status codes specified below
                    backoff_factor=0.1,  # see docs for more info
                    status_forcelist=[ 500, 502, 503, 504 ])  # will also retry if response is in these
            s.mount(f"{self.host}/", HTTPAdapter(max_retries=retries))
            resp = s.get(f"{self.host}/{self.endpoint}", headers={"Authorization": self.access_token})
            resp.raise_for_status()
  ```
  To test the above (if it does anything), make self.host go to invalid host, and you'll see it is actually effective
  
  made by myself:
  ```
    def _retry_requests_with_backoff(  # noqa: C901, WPS231, WPS212
        request_function: typing.Callable[..., requests.Response],
        url: str,
        headers: typing.Dict[str, str],
        allowed_error_status_codes: typing.Optional[typing.List[int]] = None,
    ) -> typing.Optional[requests.Response]:
    """
    Helper method to make an API request with retries and exponential backoff between each attempt, when an attempt
    raises an exception or a non ok (i.e. => 400) status code

    Args:
        request_function: The requests function to execute
        url: URL to request to
        headers: Headers for the request function
        allowed_error_status_codes: Exceptional error codes that should not cause retries to occur

    Returns:
        The response if the code is < 400 or is an allowed error code, or None if all retries failed
    """
    if allowed_error_status_codes is None:
        allowed_error_status_codes = []
    for attempt in range(MAX_ATTEMPTS_DEFINE_YOURSELF):
        try:  # noqa: WPS229
            response = request_function(url, verify=False, headers=headers)
            if response.status_code in allowed_error_status_codes or response.ok:
                return response
            if attempt == DEFAULT_RETRY_API_CALL_ATTEMPTS - 1:
                LOGGER.warning(
                    f"All attempts to reach {url} failed! "
                    + f"Last attempt gave status code: {response.status_code}.",
                )
            else:
                LOGGER.warning(
                    f"Attempt {attempt + 1} contacting: {url} failed with status code {response.status_code}!",
                )
                _wait_before_request_retry(attempt)
        except Exception as exc:  # noqa: B902 pylint: disable=broad-except
            if attempt == DEFAULT_RETRY_API_CALL_ATTEMPTS - 1:
                LOGGER.warning(
                    f"All attempts to reach {url} failed! Last attempt threw an exception: {str(exc)}",
                )
            else:
                LOGGER.warning(
                    f"Attempt {attempt + 1} reaching {url} failed with exception {str(exc)}!",  # noqa: WPS221
                )
                _wait_before_request_retry(attempt)
    return None


    def _wait_before_request_retry(attempt: int) -> None:
        """
        Helper method to log and wait between retries of requests

        Args:
            attempt: The attempt number

        """
    delay = SOME_BASE_DELAY * (
        SOME_BACKOFF_MULTIPLIER**attempt
    ) + random.uniform(0, 1)  # This is to minimize chances parallel calls of this function hitting the API at the same time
    LOGGER.warning(f"Retry reaching the endpoint in {delay} seconds...")
    time.sleep(delay)
  ```

-------------------

### Testing
- How to spy using mocker fixture of Pytest example:
  ```
    def test_check_servers_and_inform_users(
    mocker,
    server_status_pairs_in_db,
    current_server_status_pair,
    no_of_times_user_mailed,
    ):
    
    _seed_servers_table(server_status_pairs_in_db)
    t_object = job_queue.PeriodicServerAlert()
    spy_mail_users = mocker.spy(t_object, "_mail_users_server_status_msg")
    t_object.check_servers_and_inform_users()
    assert spy_mail_users.call_count == no_of_times_user_mailed
  ```
- How to change dynamically mock return value: in your email
```
queue_success_list = [elem[1] for elem in some_tuple]
def mock_queue_test_case_execution(*args, **kwargs):
    return queue_success_list.pop(0)
mocked_queue_test_case_execution = mocker.patch(TestCaseExecutionService, "queue_test_case_execution")
mocked_queue_test_case_execution.side_effect = mock_queue_test_case_execution
```

- How to keep original method functionality if you mock a function; and also how to raise exception in general
```
queue_error_lst = [elem[1] for elem in test_case_error_cases]
test_case_execution_create = TestCaseExecution.objects.create  # Original method to
def mock_queue_test_case_execution(*args, **kwargs):
    is_queue_error = queue_error_lst.pop(0)
    if is_queue_error:
        raise Exception
    return JsonResponse(
        {"some_data": "some_value"},
        status=status.HTTP_201_CREATED
    )
mocked_queue_test_case_execution = mocker.patch("somemodule.queue_test_case_execution")
mocked_queue_test_case_execution.side_effect = mock_queue_test_case_execution
```

- If you use pytest and structlog, caplog will not work; use https://pypi.org/project/pytest-structlog/ instead!

- How to write Django DRF tests using APIClient:

```
from rest_framework.test import APIClient
from django.urls import reverse

client = APIClient()
# If you want to transmit data as query params in url, add it as args in the get method
query_params = {"param1": 1, "param2": 2}
import urllib
request_helper.client.get(reverse('v2:some-file'), args=urllib.parse.urlencode(query_params), headers={"accept": "application/json"})
# If you have the data as post body instead, put it as data argument
request_helper.client.post(reverse('v2:some-file'), data=urllib.parse.urlencode(query_params), headers={"accept": "application/json"})
```

- A clean way to write Django tests and mock:
```
from django.test import TestCase

class TestFoodViewSet(TestCase):
    def setUp(self) -> None:
        self.client = APIClient()
        self.user = auth_models.User.objects.create_user(username="test-user", password="test-password")
        self.client.force_authenticate(user=self.user)

    @pytest.mark.django_db
    @mock.patch("somemodule.get_file_by_url")
    @mock.patch("somemodule.get_all_testcases")
    def test_food_variant1(
        self,
        mock_get_all_testcases: mock.MagicMock,
        mock_get_file_by_url: mock.MagicMock,
    ) -> None:

        mock_get_all_testcases.return_value = [
            {"scenario": "test", "scenario_version": "1", "testcase": "test", "variant": "test"},
            {"scenario": "test", "scenario_version": "1", "testcase": "test", "variant": "test"},
        ]
        mock_get_file_by_url.return_value = DATA_PATH / "definition1.xml"

        request = reverse("food-someendpointinsidefoodviewset")
        response = self.client.get(request)

        assert response.status_code == status.HTTP_201_CREATED
        assert response.data == {"something"}
```

- Another way to write Django tests (if you are using DRF) without using APIClient of Django:

if you have a viewset:
```
class MyObjectViewSet(ModelViewSet):  # pylint: disable=too-many-ancestors
   
    queryset = MyObject.objects.all()

    @action(methods=[HTTP_GET], detail=True)
    def abort(self, request: Request, pk: int) -> JsonResponse:  # noqa: WPS212 pylint: disable=unused-argument
        ....
```
A test to test the above method:
```
@pytest.mark.django_db
def test_something(
    request_factory: rest_framework.test.APIRequestFactory,
) -> None:
    """
    Args:
        request_factory: The RequestFactory object.
    """
    id_to_abort = 10
    request = request_factory.get(rest_framework.reverse.reverse("myobject-abort", args=[id_to_abort]))  # Get the request object as a WSGIRequest object
    # Note if you don't specify any args, you will get a 
    assert request.path == f"/api/my-object/{id_to_abort}/abort/"  # assuming you register in urls.py as router.register("my-object", MyObjectViewSet)
    request = rest_framework.views.APIView().initialize_request(request)  # Change the WSGIRequest object to Django's RestFramework Request object
    response = MyObjectViewSet().abort(request, id_to_abort)  # Call your viewset method with appropriate args
    assert response.status_code == rest_framework.status.HTTP_200_OK
    
```
- If you are not overriding the ModelViewSets and would like to test them, the above example will not work (e.g. this won't work `MyObjectViewSet().list(request)`); it will throw `ViewSet has no attribute request`. To test this, do the following
```
request = request_factory.get(rest_framework.reverse.reverse("execution-list"), headers = ..., whatever else)  # Get the request object as a WSGIRequest object
response = ExecutionViewSet.as_view({"get": "list"})(request).render()  # i.e. do not transform the request to rest_framework.request.Request type!
```
Alternative below, but the bad news with the below is you cannot debug inside the viewset itself:
```
client = APIClient()
resposne = client.get(rest_framework.reverse.reverse("execution-list"), headers = ..., whatever else)
```
The reason is that `list()` method internally calls `self.request`, which is unset if you make your request using `MyObjectViewSet().list(request)`
So use that only to test custom made endpoints. Otherwise, use either of the two described above
- Example if you have If you have a FORM type payload to test, without DRF (i.e. no serializers that access request.data for example):
```
    headers = {"Authorization": f"Bearer {refresh_token}", "Content-Type":"application/x-www-form-urlencoded"}
    data = f'client_id=VFM2&client_secret=VFM2_secret&grant_type=refresh_token&refresh_token={refresh_token.token}'
    request = RequestFactory().post(TOKEN_PATH, bytes(data, "UTF-8"), content_type="application/x-www-form-urlencoded", headers=headers)  # Returns a WSGIRequest object
    response = TokenView().post(request)  # Make sure 
    assert response.status_code == status.HTTP_200_OK
```
- Always use `HttpResponse` of django or `Response` of rest framework to make response objects for testing, and `RequestFactory` of Django to make requests; they are the most robust
- There are two types of Requests that you can make for testing in DRF: `WSGIRequest` (obtained from using `django.http.client.RequestFactory` or `rest_framework.test.APIRequestFactory`) or `rest_framework.request.Request`. `WSGIRequest` is a class that inherits Django `HttpRequest` object, but without extra attributes like `request.data` that you can use in serializers (which is in `rest_framework.request.Request`). You can convert `WSGIRequest` to `rest_framework.request.Request` using `rest_framework.views.APIView().initialize_request(request)`
- To check if response is ok or not, don't compare to specific code; your test will be brittle. Use `rest_framework.status.is_success(response.status_code)` and other methods inside that class to check.

- If you want to make a mock response object with a `.json()` implemented i.e. make the content type compatible with `application/json` accept headers, this is a general request stub:
  ```
  def create_response(status_code=200, content: typing.Any=None, **kwargs: typing.Any) -> requests.Response:
    """
    make response obj with json body type to set the attributes of the response to what we want.

    Example use:
        stubs.create_response({"myjsonkey": "someretval", "hahaha": 1111},
                             {
                                "mycustomattributeforthisresponseobject1":"someval",
                                "mycustomattributeforthisresponseobject2":"someval"
                             })
        The above will create a response with attributes:
        status_code = 200
        content = '{"myjsonkey": "someretval", "hahaha": 1111}',
        mycustomattributeforthisresponseobject1 = someval
        mycustomattributeforthisresponseobject2 = someval

    """
    response = requests.Response()
    response._content = bytes(json.dumps(content), "UTF-8")  # noqa: WPS437 Found protected attribute usage
    for response_attribute, stub_value in kwargs.items():
        setattr(response, response_attribute, stub_value)
    response.status_code = status_code
    return response
  ```

  - Use DjangoModelFactory `https://factoryboy.readthedocs.io/en/stable/orms.html#using-factory-boy-with-orms` for Django to quickly and reliably create models that are consistent (i.e. respects all validations of the model) and robust; example use that works:
    ```
    import factory
    
    class TestSuiteExecutionGroupFactory(factory.django.DjangoModelFactory):
        class Meta:
            model = myModelWithAnAttributeCallednameInIt
            django_get_or_create = ("name",)

        name = "some_group_name"
    ```
    

--------------------
### Profiling slow code

To profile a block of code that you suspect is slow:
```
import cProfile
def test():
    profiler = cProfile.Profile()
    profiler.enable()
    yourcodehere
    .
    .
    .
    profiler.disable()
    profiler.dump_stats("res.prof")
```
and to evaluate the results, open a Python interpreter and:
```
import pstats
stats = pstats.Stats('res.prof')
stats.sort_stats('cumulative').print_stats(10) # 10 is the no of results to show, slowest on top; you can change this to show more
```
--------------------------
### VSCode configs
- Pytest and running a Django server:
  ```
  {
    // Use IntelliSense to learn about possible attributes.
    // Hover to view descriptions of existing attributes.
    // For more information, visit: https://go.microsoft.com/fwlink/?linkid=830387
    "version": "0.2.0",
    "configurations": [
        {
            "name": "Python: Django",
            "type": "python",
            "request": "launch",
            "program": "${workspaceFolder}\\backend\\manage.py",
            "cwd": "${workspaceFolder}\\backend",
            "args": [
                "runserver",
                "localhost:8001"
            ],
            "django": true,
            "justMyCode": true
        },
        {
            "name": "python debug pytest",
            "type": "python",
            "request": "launch",
            "program": "${workspaceFolder}/backend/venv/Scripts/pytest.exe",
            "args": [
                "${workspaceFolder}/backend/tests/unittests",
            ],
            "console": "integratedTerminal",
            "justMyCode": false
        }
    ]
}

---------------------------
### MongoDB connection configuration; example how to make a singleton for it and how to test it
Notice that in the test class, we need to access the singleton through importing the module instead of directly importing
the singleton.
This is so that changes to the singleton in the client module (or anywhere else), are reflected too in the test!
![image](./pics/mongodb_singleton_setup.jpg)
