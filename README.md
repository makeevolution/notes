3# notes

## Notes on shortcuts/lessons learned for different topics
    stream_content = log_file_stream.iter_lines(decode_unicode=True)
    pattern_regex = re.compile(rf'\b{pattern}\b')

    page_lines = []  # Lines for the current page
    page_iter = 1    # Tracks the current page number
    last_matching_page = None  # Stores the last page with a match

    for line_no, line in enumerate(stream_content):
        # Determine the page number for the current line
        current_line_page = (line_no // items_per_page) + 1

        # If moving to a new page, check for matches in the completed page
        if current_line_page > page_iter:
            if page_iter < current_page and any(pattern_regex.search(l) for l in page_lines):
                # Save the last matching page before the current page
                last_matching_page = [
                    f"<div class='match'> {l} </div>" if pattern_regex.search(l) else l
                    for l in page_lines
                ]
            # Stop iteration if we reach the current page
            if current_line_page >= current_page:
                break
            # Reset for the new page
            page_iter = current_line_page
            page_lines = []

        # Append the current line to the page's lines
        page_lines.append(line)

    # Final check for the last page before reaching `current_page`
    if page_iter < current_page and any(pattern_regex.search(l) for l in page_lines):
        last_matching_page = [
            f"<div class='match'> {l} </div>" if pattern_regex.search(l) else l
            for l in page_lines
        ]

    # Return the last matching page or an empty list if no match is found
    return last_matching_page if last_matching_page else []
