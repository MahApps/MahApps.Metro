# Release Notes Generator for MahApps.Metro

## Table of Contents

1. [Usage](#usage)
2. [Setup](#setup)
3. [Source](#source)
4. [Example JSON Object](#json)

<a name="usage"/>
## Usage

Double clicking on the .exe will produce a `_posts` folder (if it does not already exist) and markdown files for every issue tagged `Next Release` in markdown format.

The program will strip all checkboxes away and add links to all Github issues (in the format `#123`)

<a name="setup"/>
## Setup

You can edit template.md to change the way the post is structured. [Mustache](http://mustache.github.io/) is the templating engine used.

    Note: You must include template.md in the same directory as the program as I have hardcoded the location of the file into the program.

For example, take a look at the default template

```
---
layout: no-sidebar
title: {{title}}
---
{{body}}

```

The text nested between handlebars `{{}}` are variables that will be replaced by the program. Although I only use 2 variables, you can use other variables such as `html_url` or `number` or `milestone` 
to add in more information to the release post.

The list of variables and their values that you can substitute in are the key/value pairs of the JSON object that the [GitHub API](http://developer.github.com/v3/issues/#list-issues-for-a-repository)
returns. An example of the response at the end of this [post](#json).

<a name="source"/>
## Modifying the source:

This program was written in python 3.2 I have included the source code for the program inside the source code folder. Feel free to modify the program if you want.

The `exe` file was built using cxfreeze for python 3.2.

<a name="json"/>
## Example JSON Object

```json
[
  {
    "url": "https://api.github.com/repos/octocat/Hello-World/issues/1347",
    "html_url": "https://github.com/octocat/Hello-World/issues/1347",
    "number": 1347,
    "state": "open",
    "title": "Found a bug",
    "body": "I'm having a problem with this.",
    "user": {
      "login": "octocat",
      "id": 1,
      "avatar_url": "https://github.com/images/error/octocat_happy.gif",
      "gravatar_id": "somehexcode",
      "url": "https://api.github.com/users/octocat",
      "html_url": "https://github.com/octocat",
      "followers_url": "https://api.github.com/users/octocat/followers",
      "following_url": "https://api.github.com/users/octocat/following{/other_user}",
      "gists_url": "https://api.github.com/users/octocat/gists{/gist_id}",
      "starred_url": "https://api.github.com/users/octocat/starred{/owner}{/repo}",
      "subscriptions_url": "https://api.github.com/users/octocat/subscriptions",
      "organizations_url": "https://api.github.com/users/octocat/orgs",
      "repos_url": "https://api.github.com/users/octocat/repos",
      "events_url": "https://api.github.com/users/octocat/events{/privacy}",
      "received_events_url": "https://api.github.com/users/octocat/received_events",
      "type": "User",
      "site_admin": false
    },
    "labels": [
      {
        "url": "https://api.github.com/repos/octocat/Hello-World/labels/bug",
        "name": "bug",
        "color": "f29513"
      }
    ],
    "assignee": {
      "login": "octocat",
      "id": 1,
      "avatar_url": "https://github.com/images/error/octocat_happy.gif",
      "gravatar_id": "somehexcode",
      "url": "https://api.github.com/users/octocat",
      "html_url": "https://github.com/octocat",
      "followers_url": "https://api.github.com/users/octocat/followers",
      "following_url": "https://api.github.com/users/octocat/following{/other_user}",
      "gists_url": "https://api.github.com/users/octocat/gists{/gist_id}",
      "starred_url": "https://api.github.com/users/octocat/starred{/owner}{/repo}",
      "subscriptions_url": "https://api.github.com/users/octocat/subscriptions",
      "organizations_url": "https://api.github.com/users/octocat/orgs",
      "repos_url": "https://api.github.com/users/octocat/repos",
      "events_url": "https://api.github.com/users/octocat/events{/privacy}",
      "received_events_url": "https://api.github.com/users/octocat/received_events",
      "type": "User",
      "site_admin": false
    },
    "milestone": {
      "url": "https://api.github.com/repos/octocat/Hello-World/milestones/1",
      "number": 1,
      "state": "open",
      "title": "v1.0",
      "description": "",
      "creator": {
        "login": "octocat",
        "id": 1,
        "avatar_url": "https://github.com/images/error/octocat_happy.gif",
        "gravatar_id": "somehexcode",
        "url": "https://api.github.com/users/octocat",
        "html_url": "https://github.com/octocat",
        "followers_url": "https://api.github.com/users/octocat/followers",
        "following_url": "https://api.github.com/users/octocat/following{/other_user}",
        "gists_url": "https://api.github.com/users/octocat/gists{/gist_id}",
        "starred_url": "https://api.github.com/users/octocat/starred{/owner}{/repo}",
        "subscriptions_url": "https://api.github.com/users/octocat/subscriptions",
        "organizations_url": "https://api.github.com/users/octocat/orgs",
        "repos_url": "https://api.github.com/users/octocat/repos",
        "events_url": "https://api.github.com/users/octocat/events{/privacy}",
        "received_events_url": "https://api.github.com/users/octocat/received_events",
        "type": "User",
        "site_admin": false
      },
      "open_issues": 4,
      "closed_issues": 8,
      "created_at": "2011-04-10T20:09:31Z",
      "updated_at": "2014-03-03T18:58:10Z",
      "due_on": null
    },
    "comments": 0,
    "pull_request": {
      "html_url": "https://github.com/octocat/Hello-World/pull/1347",
      "diff_url": "https://github.com/octocat/Hello-World/pull/1347.diff",
      "patch_url": "https://github.com/octocat/Hello-World/pull/1347.patch"
    },
    "closed_at": null,
    "created_at": "2011-04-22T13:33:48Z",
    "updated_at": "2011-04-22T13:33:48Z"
  }
]
```

