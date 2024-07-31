3# notes

## Notes on shortcuts/lessons learned for different topics

# tests/test_clear_expired_tokens.py
import pytest
from django.utils import timezone
from django.urls import reverse
from rest_framework.test import APIClient
from myapp.models import CustomToken, AccessToken, RefreshToken
from datetime import timedelta
from oauth2_provider.settings import oauth2_settings
from typing import Tuple

@pytest.fixture
def create_tokens(db) -> Tuple[AccessToken, AccessToken, RefreshToken, RefreshToken]:
    """
    Fixture to create access and refresh tokens for testing.
    
    Args:
        db: The database fixture for creating tokens.

    Returns:
        Tuple containing expired access token, valid access token, expired refresh token, valid refresh token.
    """
    def _create_tokens(access_expired: bool, refresh_expired: bool) -> Tuple[AccessToken, AccessToken, RefreshToken, RefreshToken]:
        now = timezone.now()
        REFRESH_TOKEN_EXPIRE_SECONDS = oauth2_settings.REFRESH_TOKEN_EXPIRE_SECONDS
        if not isinstance(REFRESH_TOKEN_EXPIRE_SECONDS, timedelta):
            REFRESH_TOKEN_EXPIRE_SECONDS = timedelta(seconds=REFRESH_TOKEN_EXPIRE_SECONDS)

        refresh_expire_time = now - REFRESH_TOKEN_EXPIRE_SECONDS if refresh_expired else now + REFRESH_TOKEN_EXPIRE_SECONDS
        access_expire_time = now - REFRESH_TOKEN_EXPIRE_SECONDS if access_expired else now + REFRESH_TOKEN_EXPIRE_SECONDS

        expired_access_token = AccessToken.objects.create(user_id=1, expires=access_expire_time)
        valid_access_token = AccessToken.objects.create(user_id=2, expires=now + timedelta(days=1))
        expired_refresh_token = RefreshToken.objects.create(user_id=1, revoked=refresh_expire_time)
        valid_refresh_token = RefreshToken.objects.create(user_id=2, revoked=now + timedelta(days=1))

        return expired_access_token, valid_access_token, expired_refresh_token, valid_refresh_token

    return _create_tokens

@pytest.mark.django_db
@pytest.mark.parametrize(
    "access_expired, refresh_expired, expected_deleted_access, expected_deleted_refresh",
    [
        (True, True, True, True),     # Both expired
        (True, False, True, False),   # Access expired, refresh not
        (False, True, False, True),   # Access not, refresh expired
        (False, False, False, False), # Neither expired
    ]
)
def test_clear_expired_tokens(
    create_tokens,
    access_expired: bool,
    refresh_expired: bool,
    expected_deleted_access: bool,
    expected_deleted_refresh: bool
) -> None:
    """
    Test the clear_expired_tokens endpoint to ensure expired tokens are correctly deleted.

    Args:
        create_tokens: Fixture to create tokens for testing.
        access_expired: Whether the access token should be expired.
        refresh_expired: Whether the refresh token should be expired.
        expected_deleted_access: Whether the expired access token should be deleted.
        expected_deleted_refresh: Whether the expired refresh token should be deleted.
    """
    client = APIClient()
    url = reverse('token-clear-expired-tokens')  # Adjust the name based on your URL configuration

    expired_access_token, valid_access_token, expired_refresh_token, valid_refresh_token = create_tokens(access_expired, refresh_expired)

    # Ensure tokens are in the database
    assert AccessToken.objects.count() == 2
    assert RefreshToken.objects.count() == 2

    # Call the clear_expired_tokens endpoint
    response = client.delete(url)

    # Verify the response
    assert response.status_code == 202

    # Verify expired tokens are removed
    if expected_deleted_access:
        assert not AccessToken.objects.filter(id=expired_access_token.id).exists()
    else:
        assert AccessToken.objects.filter(id=expired_access_token.id).exists()

    if expected_deleted_refresh:
        assert not RefreshToken.objects.filter(id=expired_refresh_token.id).exists()
    else:
        assert RefreshToken.objects.filter(id=expired_refresh_token.id).exists()

    # Verify valid tokens are still present
    assert AccessToken.objects.filter(id=valid_access_token.id).exists()
    assert RefreshToken.objects.filter(id=valid_refresh_token.id).exists()
