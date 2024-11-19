3# notes

## Notes on shortcuts/lessons learned for different topics

data = []
            page_iter = 1
            stream_content = log_file_stream.iter_lines(decode_unicode=True)
            for line_no, line in enumerate(stream_content):  # line_no starts with 0
                if line_no > page_iter * items_per_page:
                    page_iter += 1
                    data = []
                    data.append(line)
                    continue
                if page_iter > current_page:
                    if re.search(rf'\b{PATTERN}\b', line):
                        data.append(line)
                        for line_remaining in stream_content:
                            
                            data.append(line_remaining)
                            if len(data) > items_per_page:
                                break
                        data2 = []
                        for dt in data:
                            if re.search(rf'\b{PATTERN}\b', dt):
                                dt = f"<div class='match'> {dt} </div>"
                            data2.append(dt)
                        data=data2
                        break
                data.append(line)
