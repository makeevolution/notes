3# notes

## Notes on shortcuts/lessons learned for different topics

None selected

Skip to content
Using Gmail with screen readers
Conversations
12.18 GB of 15 GB (81%) used
Terms · Privacy · Program Policies
Last account activity: 0 minutes ago
Details
Compose:
New Message
	MinimizePop-outClose
	
Recipients
import base64
from django.http import HttpRequest

def get_id_from_auth_header(request: HttpRequest):
    user_id = None

    # First, check if the request body has a 'client_id' key
    client_id = request.POST.get('client_id')
    if client_id:
        user_id = client_id
    else:
        # If 'client_id' is not in the body, check the Authorization header
        auth_header = request.headers.get('Authorization')
        if auth_header and auth_header.startswith('Basic '):
            # Extract the Base64 encoded part
            encoded_credentials = auth_header.split(' ')[1]
            try:
                # Decode the Base64 encoded string
                decoded_credentials = base64.b64decode(encoded_credentials).decode('utf-8')
                # Ensure the decoded credentials are in the format id:secret
                if ':' in decoded_credentials:
                    user_id = decoded_credentials.split(':', 1)[0]
            except (TypeError, ValueError):
                pass

    return user_id




import base64
import base64
import pytest
from django.test import RequestFactory
from django.http import QueryDict
from myapp.utils import get_id_from_auth_header  # adjust the import based on your project structure

@pytest.fixture
def request_factory():
    return RequestFactory()

def encode_credentials(id, secret):
    credentials = f'{id}:{secret}'
    encoded_credentials = base64.b64encode(credentials.encode('utf-8')).decode('utf-8')
    return f'Basic {encoded_credentials}'

@pytest.mark.parametrize("body, auth_header, expected", [
    # Test client_id in body
    (QueryDict('client_id=test_client_id'), None, 'test_client_id'),
    
    # Test valid Basic Auth
    (QueryDict(), encode_credentials('test_id', 'test_secret'), 'test_id'),
    
    # Test missing Authorization header
    (QueryDict(), None, None),
    
    # Test non-Basic Authorization header
    (QueryDict(), 'Bearer some_token', None),
    
    # Test invalid Base64 encoding
    (QueryDict(), 'Basic invalid_base64', None),
    
    # Test missing colon in decoded credentials
    (QueryDict(), 'Basic ' + base64.b64encode(b'test_id_test_secret').decode('utf-8'), None),
    
    # Test empty Basic Auth
    (QueryDict(), 'Basic ', None),
    
    # Test empty ID in Basic Auth
    (QueryDict(), encode_credentials('', 'test_secret'), ''),
    
    # Test empty secret in Basic Auth
    (QueryDict(), encode_credentials('test_id', ''), 'test_id'),
])
def test_get_id_from_auth_header(request_factory, body, auth_header, expected):
    request = request_factory.post('/example/', data=body)
    if auth_header:
        request.META['HTTP_AUTHORIZATION'] = auth_header
    
    user_id = get_id_from_auth_header(request)
    assert user_id == expected'
