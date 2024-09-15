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
curl_command = "curl -X POST \"http://your-api-endpoint.com\" -H \"Content-Type: application/json\" -d '{\"test_suite_id\": 0, \"testcycle\": {\"name\": \"string\"}, \"overrules\": [{\"resolve_type\": \"overrule_devbench\", \"sut_name\": \"Twinscan\", \"overrule_data\": {\"devbench_name\": \"my_cool_devbench_name\", \"apply_predefined_config\": \"true\"}}]}'"
