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
