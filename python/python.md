,# Python

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
    request = request_factory.get(rest_framework.reverse.reverse("myobject-abort", args=[id_to_abort]))  # Get the request object
    assert request.path == f"/api/my-object/{id_to_abort}/abort/"  # assuming you register in urls.py as router.register("my-object", MyObjectViewSet)

    response = MyObjectViewSet().abort(rest_framework.views.APIView().initialize_request(request), id_to_abort)  # Call your viewset method with appropriate args
    assert response.status_code == rest_framework.status.HTTP_200_OK
    
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
