3# notes

## Notes on shortcuts/lessons learned for different topics

        
        log_file_stream = Artifactory().stream_file_by_url(log_file)
        current_page = int(request.query_params.get("page", 1))
        items_per_page = int(request.query_params.get("itemsPerPage", 25))
        next_match_in_other_page = request.query_params.get("nextMatchInOtherPage", False) != 'false'
        prev_match_in_other_page = request.query_params.get("prevMatchInOtherPage", False) != 'false'
        last_match_in_file = request.query_params.get("lastMatchInFile", False) != 'false'
        if next_match_in_other_page and prev_match_in_other_page:
            raise ValidationError("Cannot ask for both next match and prev match in other pages simultaneously!")
        if next_match_in_other_page:
            stream_content = log_file_stream.iter_lines(decode_unicode=True)
            pattern_regex = re.compile(rf'\b{PATTERN}\b')
            page_contents = []
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
                    # Highlight lines already found on the page
                    page_contents = [
                        f"<div class='match'> {relevant_line} </div>" if pattern_regex.search(relevant_line) else relevant_line
                        for relevant_line in page_lines
                    ]
                    while len(page_contents) < items_per_page:
                        try:
                            next_line_in_page = next(stream_content)
                            page_contents.append(f"<div class='match'> {next_line_in_page} </div>" if pattern_regex.search(next_line_in_page) else next_line_in_page)
                        except StopIteration:
                            # We are at the last page (which may contain less items than items_per_page)
                            break
                    break
            page_no = page_iter
        elif prev_match_in_other_page:
            stream_content = log_file_stream.iter_lines(decode_unicode=True)
            pattern_regex = re.compile(rf'\b{PATTERN}\b')

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
            page_contents = last_matching_page if last_matching_page else []

            page_no = page_iter
        elif last_match_in_file:
            stream_content = log_file_stream.iter_lines(decode_unicode=True)
            pattern_regex = re.compile(rf'\b{PATTERN}\b')

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

            page_contents = last_matching_page if last_matching_page else []

            page_no = page_iter
        else:
            page_no, page_contents = self._get_page_in_log_file(log_file_stream, current_page, items_per_page)
        response = JsonResponse(data={"page_no": page_no, "page_contents": page_contents})
        return response
