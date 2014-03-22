import urllib
import urllib.request
import json
import pystache
import re
import os

def issue_ref_generator(n):
    while True:
        yield '{0}'.format(n)
        n += 1

def format_issue(issue, template):
    #Constants
    pattern_for_checkbox = r"((-)( ))(\[.\] )"
    pattern_for_issues = r"((#([0-9]+)))"
    link_format = '[{0}]: https://github.com/MahApps/MahApps.Metro/pull/{1}'

    # Substitue JSON into template
    renderer = pystache.Renderer(escape = (lambda u : u))

    templated_text = renderer.render(template, issue)

    # Strip Checkboxes
    text_with_no_checkboxes = re.sub(pattern_for_checkbox, r'\1', templated_text)

    #Get links for issues
    links = []

    for val,issue_number in enumerate(re.finditer(pattern_for_issues, text_with_no_checkboxes)):
        links.append(link_format.format(val, issue_number.group(2)))

    #Replace issues with markdown reference links
    ref_gen = issue_ref_generator(0)
    final_text = re.sub(\
        pattern_for_issues,
        lambda m: '[{0}][{1}]'.format(m.group(2), next(ref_gen)),
        text_with_no_checkboxes)

    #Append links to the bottom of the text
    for link in links:
        final_text += ('\n' + link)

    #Get File Name
    fileName = '_posts/{0}-{1}.md'.format((issue["updated_at"])[0:10], issue["title"])

    if not os.path.exists('_posts'):
        os.makedirs('_posts')

    #Write to File
    with open(fileName, 'w') as text_file:
        text_file.write(final_text)


with open("template.md", "r") as myfile:
    template=myfile.read()

with urllib.request.urlopen('https://api.github.com/repos/MahApps/MahApps.Metro/issues?labels=Next+Release') as response:
    text = response.readall().decode('utf-8')

jobject = json.loads(text)

for issue in reversed(jobject):
    format_issue(issue, template)







