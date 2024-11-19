3# notes

## Notes on shortcuts/lessons learned for different topics
    stream_content = log_file_stream.iter_lines(decode_unicode=True)
    pattern_regex = re.compile(rf'\b{pattern}\b')

    page_lines = []  # Collects lines for the current page
    page_iter = 1    # Tracks the current page number
    last_matching_page = None  # Stores the last matching page

    for line_no, line in enumerate(stream_content):
        # Determine the page number for the current line
        current_line_page = (line_no // items_per_page) + 1

        # If moving to a new page, check for matches in the completed page
        if current_line_page > page_iter:
            if any(pattern_regex.search(l) for l in page_lines):
                # Save the last matching page
                last_matching_page = [
                    f"<div class='match'> {l} </div>" if pattern_regex.search(l) else l
                    for l in page_lines
                ]
            # Reset for the new page
            page_iter = current_line_page
            page_lines = []

        # Append the current line to the page's lines
        page_lines.append(line)

    # Final check for the last page
    if any(pattern_regex.search(l) for l in page_lines):
        last_matching_page = [
            f"<div class='match'> {l} </div>" if pattern_regex.search(l) else l
            for l in page_lines
        ]

    # Return the last matching page or an empty list if no match is found
    return last_matching_page if last_matching_page else []






    import re

def find_page_with_first_match(log_file_stream, pattern, items_per_page):
    """
    Finds and returns the page that contains the first match of a given pattern.
    
    Args:
        log_file_stream: Stream of log file lines.
        pattern: Regex pattern to search for.
        items_per_page: Number of items per page.
    
    Returns:
        List of lines for the page with the first match.
    """
    stream_content = log_file_stream.iter_lines(decode_unicode=True)
    pattern_regex = re.compile(rf'\b{pattern}\b')

    page_lines = []  # Collects lines for the current page
    page_iter = 1    # Tracks the current page number

    for line_no, line in enumerate(stream_content):
        # Determine the page number for the current line
        current_line_page = (line_no // items_per_page) + 1

        # If moving to a new page, reset the page lines
        if current_line_page > page_iter:
            page_iter = current_line_page
            page_lines = []

        # Append the current line to the page's lines
        page_lines.append(line)

        # Check for a match
        if pattern_regex.search(line):
            # Highlight matches and return the page immediately
            return [
                f"<div class='match'> {l} </div>" if pattern_regex.search(l) else l
                for l in page_lines
            ]

    # If no match is found, return an empty list
    return []

