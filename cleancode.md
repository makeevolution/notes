https://github.com/thangchung/clean-code-dotnet
https://github.com/zedr/clean-code-python


```
def get_groups_membership_of_user(self, username: str) -> typing.List[str]: 

        """ 

        Contacts the user groups microservice to obtain group membership info of a user 

 

        Args: 

            username: The username 

 

        Returns: 

            List of group names 



        Raises: 

            KeyError: If the response from the service is not processable 

        """ 

        with requests.Session() as sess: 

            retries = Retry(total=5, backoff_factor=0.1, status_forcelist=[500, 502, 503, 504]) 

            sess.mount(f"{self.host}/", HTTPAdapter(max_retries=retries)) 

            resp = sess.get(f"{self.host}/{self.user_endpoint}/{username}/", headers=self.header)  # noqa: WPS221 

            resp.raise_for_status() 

        try: 

            groups = json.loads(resp.content)["groups"] 

            logger.info(f"User with username: {username} is found to be in groups: {groups}") 

            return groups 

        except (json.JSONDecodeError, KeyError) as exc: 

            logger.error( 

                f"Error occurred processing groups info for user with username {username} " 

                + f"from user mgmt service, response: {resp.content!r}", 

            ) 
            raise exc
```

```
import pytest
import requests
from unittest import mock
import json
from your_module import YourClass  # Replace with the actual import

# Mock response class
class MockResponse:
    def __init__(self, json_data, status_code, text_data=None):
        self.json_data = json_data
        self.status_code = status_code
        self.text_data = text_data

    def json(self):
        if self.json_data is not None:
            return self.json_data
        raise json.JSONDecodeError("Mocked error", self.text_data, 0)

    @property
    def content(self):
        return json.dumps(self.json_data).encode() if self.json_data is not None else self.text_data.encode()

    def raise_for_status(self):
        if self.status_code != 200:
            raise requests.exceptions.HTTPError(f"HTTP Error: {self.status_code}")

def test_get_groups_membership_of_user_success():
    username = 'testuser'
    expected_groups = ['group1', 'group2']
    response_data = {'groups': expected_groups}

    with mock.patch('requests.Session.get') as mock_get:
        mock_get.return_value = MockResponse(json_data=response_data, status_code=200)
        
        instance = YourClass()
        instance.host = 'http://example.com'
        instance.user_endpoint = 'user_endpoint'
        instance.header = {'Authorization': 'Bearer token'}
        
        result = instance.get_groups_membership_of_user(username)
        assert result == expected_groups

def test_get_groups_membership_of_user_key_error():
    username = 'testuser'
    response_data = {'not_groups': []}

    with mock.patch('requests.Session.get') as mock_get:
        mock_get.return_value = MockResponse(json_data=response_data, status_code=200)
        
        instance = YourClass()
        instance.host = 'http://example.com'
        instance.user_endpoint = 'user_endpoint'
        instance.header = {'Authorization': 'Bearer token'}
        
        with pytest.raises(KeyError):
            instance.get_groups_membership_of_user(username)

def test_get_groups_membership_of_user_json_decode_error():
    username = 'testuser'
    response_text = "Invalid JSON"

    with mock.patch('requests.Session.get') as mock_get:
        mock_get.return_value = MockResponse(json_data=None, status_code=200, text_data=response_text)
        
        instance = YourClass()
        instance.host = 'http://example.com'
        instance.user_endpoint = 'user_endpoint'
        instance.header = {'Authorization': 'Bearer token'}
        
        with pytest.raises(json.JSONDecodeError):
            instance.get_groups_membership_of_user(username)

def test_get_groups_membership_of_user_http_error():
    username = 'testuser'

    with mock.patch('requests.Session.get') as mock_get:
        mock_get.return_value = MockResponse(json_data=None, status_code=500)
        
        instance = YourClass()
        instance.host = 'http://example.com'
        instance.user_endpoint = 'user_endpoint'
        instance.header = {'Authorization': 'Bearer token'}
        
        with pytest.raises(requests.exceptions.HTTPError):
            instance.get_groups_membership_of_user(username)

```
