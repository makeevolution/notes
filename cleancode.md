https://github.com/thangchung/clean-code-dotnet
https://github.com/zedr/clean-code-python


```
apiVersion: apps/v1
kind: Deployment
metadata:
  name: logger-deployment
spec:
  replicas: 1
  selector:
    matchLabels:
      app: logger
  template:
    metadata:
      labels:
        app: logger
    spec:
      containers:
      - name: logger
        image: busybox
        command: ["/bin/sh", "-c", "sh /scripts/logger.sh"]
        volumeMounts:
        - name: script-volume
          mountPath: /scripts
      volumes:
      - name: script-volume
        configMap:
          name: logger-script

```

```
import pytest
import requests
from unittest import mock
import json
from typing import List, Dict, Optional, Any
from your_module import YourClass  # Replace with the actual import

class MockResponse:
    """
    MockResponse simulates the response object returned by requests.

    Attributes:
        json_data: The data to return for the json method.
        status_code: The HTTP status code of the response.
        text_data: The raw text data of the response.
    """

    def __init__(self, json_data: Optional[Dict[str, Any]], status_code: int, text_data: Optional[str] = None):
        self.json_data = json_data
        self.status_code = status_code
        self.text_data = text_data

    def json(self) -> Dict[str, Any]:
        """
        Simulates the json method of the requests.Response object.

        Returns:
            The JSON data.
        
        Raises:
            json.JSONDecodeError: If the json_data is None.
        """
        if self.json_data is not None:
            return self.json_data
        raise json.JSONDecodeError("Mocked error", self.text_data, 0)

    @property
    def content(self) -> bytes:
        """
        Simulates the content property of the requests.Response object.

        Returns:
            The content of the response as bytes.
        """
        return json.dumps(self.json_data).encode() if self.json_data is not None else self.text_data.encode()

    def raise_for_status(self) -> None:
        """
        Simulates the raise_for_status method of the requests.Response object.

        Raises:
            requests.exceptions.HTTPError: If the status code is not 200.
        """
        if self.status_code != 200:
            raise requests.exceptions.HTTPError(f"HTTP Error: {self.status_code}")

def test_get_groups_membership_of_user_success() -> None:
    """
    Tests that get_groups_membership_of_user returns the correct group membership on success.
    """
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

def test_get_groups_membership_of_user_key_error() -> None:
    """
    Tests that get_groups_membership_of_user raises a KeyError when the 'groups' key is missing.
    """
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

def test_get_groups_membership_of_user_json_decode_error() -> None:
    """
    Tests that get_groups_membership_of_user raises a JSONDecodeError when the response is not valid JSON.
    """
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

def test_get_groups_membership_of_user_http_error() -> None:
    """
    Tests that get_groups_membership_of_user raises an HTTPError for non-200 status codes.
    """
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
