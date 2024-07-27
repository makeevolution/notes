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
import pytest
from django.test import RequestFactory
from myapp.utils import get_id_from_auth_header  # adjust the import based on your project structure

@pytest.fixture
def request_factory():
    return RequestFactory()

def encode_credentials(id, secret):
    credentials = f'{id}:{secret}'
    encoded_credentials = base64.b64encode(credentials.encode('utf-8')).decode('utf-8')
    return f'Basic {encoded_credentials}'

def test_client_id_in_body(request_factory):
    request = request_factory.post('/example/', data={'client_id': 'test_client_id'})
    user_id = get_id_from_auth_header(request)
    assert user_id == 'test_client_id'

def test_valid_basic_auth(request_factory):
    request = request_factory.get('/example/', HTTP_AUTHORIZATION=encode_credentials('test_id', 'test_secret'))
    user_id = get_id_from_auth_header(request)
    assert user_id == 'test_id'

def test_missing_authorization_header(request_factory):
    request = request_factory.get('/example/')
    user_id = get_id_from_auth_header(request)
    assert user_id is None

def test_non_basic_authorization_header(request_factory):
    request = request_factory.get('/example/', HTTP_AUTHORIZATION='Bearer some_token')
    user_id = get_id_from_auth_header(request)
    assert user_id is None

def test_invalid_base64_encoding(request_factory):
    request = request_factory.get('/example/', HTTP_AUTHORIZATION='Basic invalid_base64')
    user_id = get_id_from_auth_header(request)
    assert user_id is None

def test_missing_colon_in_decoded_credentials(request_factory):
    encoded_credentials = base64.b64encode('test_id_test_secret'.encode('utf-8')).decode('utf-8')
    request = request_factory.get('/example/', HTTP_AUTHORIZATION=f'Basic {encoded_credentials}')
    user_id = get_id_from_auth_header(request)
    assert user_id is None

def test_empty_basic_auth(request_factory):
    request = request_factory.get('/example/', HTTP_AUTHORIZATION='Basic ')
    user_id = get_id_from_auth_header(request)
    assert user_id is None

def test_empty_id_in_basic_auth(request_factory):
    request = request_factory.get('/example/', HTTP_AUTHORIZATION=encode_credentials('', 'test_secret'))
    user_id = get_id_from_auth_header(request)
    assert user_id == ''

def test_empty_secret_in_basic_auth(request_factory):
    request = request_factory.get('/example/', HTTP_AUTHORIZATION=encode_credentials('test_id', ''))
    user_id = get_id_from_auth_header(request)
    assert user_id == 'test_id'
