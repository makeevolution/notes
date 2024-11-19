3# notes

## Notes on shortcuts/lessons learned for different topics
    stream_content = log_file_stream.iter_lines(decode_unicode=True)
    pattern_regex = re.compile(rf'\b{pattern}\b')

    page_lines = []  # Collects lines for the current page
    page_iter = 1    # Tracks the current page number

    for line_no, line in enumerate(stream_content):
        # Determine the page number for the current line
        current_line_page = (line_no // items_per_page) + 1
        
        # When we move to a new page, reset the page_lines
        if current_line_page > page_iter:
            page_iter = current_line_page
            page_lines = []

        # Append the line to the current page's data
        page_lines.append(line)

        # If we've moved past the current_page and find a match, return the page
        if page_iter > current_page and pattern_regex.search(line):
            # Highlight matched lines on the page
            return [
                f"<div class='match'> {l} </div>" if pattern_regex.search(l) else l
                for l in page_lines
            ]
    
    # If no page with a match is found, return an empty list
    return []
