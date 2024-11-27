3# notes

## Notes on shortcuts/lessons learned for different topics
Refactoring this code focuses on improving its efficiency, readability, and maintainability without altering its functionality. Here are the main changes applied:
 for line in Artifactory().stream_file_by_url(log_file).iter_lines(decode_unicode=True):
            for pattern in root_causes:
                if re.search(pattern, line):
                    no_of_root_causes += 1
     no_of_lines += 1

 return no_of_lines, no_f_root_causes
