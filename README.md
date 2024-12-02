3# notes

## Notes on shortcuts/lessons learned for different topics
    def stream_file_by_url(self, file_url: str) -> requests.Response:  # noqa: WPS212, WPS221, WPS231
        """
        Streams the file available at the provided link

        Args:
            file_url: artifactory link of the file

        Returns:
            downloaded file path object
        """
        with requests.Session() as sess:
            retries = Retry(
                total=5,  # total retries; will retry on all connection errors (e.g. ReadTimeoutError, etc.)
                # and response status codes specified below
                backoff_factor=0.1,
                status_forcelist=[500, 502, 503, 504],  # will also retry if response is in these
            )
            sess.mount(f"{self.base_url}/", HTTPAdapter(max_retries=retries))
            resp = sess.get(
                f"{self.base_url}{file_url.replace(self.base_url, '')}",
                verify=False,
                stream=True,
                auth=self.auth,
            )
            resp.raise_for_status()
            return resp
