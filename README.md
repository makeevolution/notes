3# notes

## Notes on shortcuts/lessons learned for different topics
Refactoring this code focuses on improving its efficiency, readability, and maintainability without altering its functionality. Here are the main changes applied:
 for line in Artifactory().stream_file_by_url(log_file).iter_lines(decode_unicode=True):
            for pattern in root_causes:
                if re.search(pattern, line):
                    no_of_root_causes += 1
     no_of_lines += 1

 return no_of_lines, no_f_root_causes

list1 = [1, 2, 3, 4, 5]
list2 = [1, 2, 0, 4, 6]

# Compare elements and find unequal ones
unequal_elements = [(i, a, b) for i, (a, b) in enumerate(zip(list1, list2)) if a != b]

print("Unequal elements (index, list1_value, list2_value):", unequal_elements)
