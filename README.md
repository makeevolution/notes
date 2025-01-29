
3# notes

## Notes on shortcuts/lessons learned for different topics


347    def _append_auth_header( 

348        self, 

349        response: requests.Response, 

350        *args: typing.Any,  # pylint: disable=unused-argument 

351        **kwargs: typing.Any,  # pylint: disable=unused-argument 

352    ) -> requests.Response: 

353        """ 

354        Helper method to append auth headers on requests automatically made after a response i.e. a redirection 

355 

356        Args: 

357            response: The response from Jenkins 

358            args: variable arguments 

359            kwargs: variable keyword arguments 

360 

361        Returns: 

362            The response object with redirection request headers appended with the authorization headers 

363        """ 

364        auth_encoded = base64.b64encode(f"{self.username}:{self.password}".encode()).decode()  # noqa: WPS221 

365        response.request.headers["Authorization"] = f"Basic {auth_encoded}" 

366        return response 
        verify=False,
        stream=True,
        auth=test_class_instance.auth,
    )
