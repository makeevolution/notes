3# notes

## Notes on shortcuts/lessons learned for different topics

        
class LogFileHandler:
    def __init__(self, log_file_stream, items_per_page, pattern_regex):
        self.stream_content = log_file_stream.iter_lines(decode_unicode=True)
        self.items_per_page = items_per_page
        self.pattern_regex = pattern_regex

    def handle_next_match(self, current_page):
        """Find the next match after the current page."""
        page_lines = []
        page_iter = 1

        for line_no, line in enumerate(self.stream_content):
            current_line_page = (line_no // self.items_per_page) + 1

            if current_line_page > page_iter:
                page_iter = current_line_page
                page_lines = []

            page_lines.append(line)

            if page_iter > current_page and self.pattern_regex.search(line):
                return page_iter, self._highlight_and_fill_page(page_lines)

        return None, []

    def handle_prev_match(self, current_page):
        """Find the last match before the current page."""
        page_lines = []
        page_iter = 1
        last_matching_page = None

        for line_no, line in enumerate(self.stream_content):
            current_line_page = (line_no // self.items_per_page) + 1

            if current_line_page > page_iter:
                if page_iter < current_page and any(self.pattern_regex.search(l) for l in page_lines):
                    last_matching_page = self._highlight_page(page_lines)
                if current_line_page >= current_page:
                    break
                page_iter = current_line_page
                page_lines = []

            page_lines.append(line)

        if page_iter < current_page and any(self.pattern_regex.search(l) for l in page_lines):
            last_matching_page = self._highlight_page(page_lines)

        return page_iter, last_matching_page if last_matching_page else []

    def handle_last_match(self):
        """Find the last match in the file."""
        page_lines = []
        page_iter = 1
        last_matching_page = None

        for line_no, line in enumerate(self.stream_content):
            current_line_page = (line_no // self.items_per_page) + 1

            if current_line_page > page_iter:
                if any(self.pattern_regex.search(l) for l in page_lines):
                    last_matching_page = self._highlight_page(page_lines)
                page_iter = current_line_page
                page_lines = []

            page_lines.append(line)

        if any(self.pattern_regex.search(l) for l in page_lines):
            last_matching_page = self._highlight_page(page_lines)

        return page_iter, last_matching_page if last_matching_page else []

    def _highlight_page(self, page_lines):
        """Highlight matching lines in a page."""
        return [
            f"<div class='match'> {line} </div>" if self.pattern_regex.search(line) else line
            for line in page_lines
        ]

    def _highlight_and_fill_page(self, page_lines):
        """Highlight matching lines and fill the rest of the page."""
        page_contents = self._highlight_page(page_lines)
        while len(page_contents) < self.items_per_page:
            try:
                next_line = next(self.stream_content)
                page_contents.append(
                    f"<div class='match'> {next_line} </div>" if self.pattern_regex.search(next_line) else next_line
                )
            except StopIteration:
                break
        return page_contents


def handle_request(request, log_file):
    """Main handler for the request."""
    log_file_stream = Artifactory().stream_file_by_url(log_file)
    current_page = int(request.query_params.get("page", 1))
    items_per_page = int(request.query_params.get("itemsPerPage", 25))
    next_match_in_other_page = request.query_params.get("nextMatchInOtherPage", False) != 'false'
    prev_match_in_other_page = request.query_params.get("prevMatchInOtherPage", False) != 'false'
    last_match_in_file = request.query_params.get("lastMatchInFile", False) != 'false'

    if next_match_in_other_page and prev_match_in_other_page:
        raise ValidationError("Cannot ask for both next match and prev match in other pages simultaneously!")

    pattern_regex = re.compile(rf'\b{PATTERN}\b')
    handler = LogFileHandler(log_file_stream, items_per_page, pattern_regex)

    # Map strategies to request parameters
    strategies = {
        "nextMatchInOtherPage": lambda: handler.handle_next_match(current_page),
        "prevMatchInOtherPage": lambda: handler.handle_prev_match(current_page),
        "lastMatchInFile": lambda: handler.handle_last_match(),
    }

    # Determine the active strategy
    active_strategy = None
    for key in strategies:
        if request.query_params.get(key, False) != 'false':
            active_strategy = strategies[key]
            break

    # Execute the selected strategy
    if active_strategy:
        page_no, page_contents = active_strategy()
    else:
        page_no, page_contents = self._get_page_in_log_file(log_file_stream, current_page, items_per_page)

    return JsonResponse(data={"page_no": page_no, "page_contents": page_contents})
