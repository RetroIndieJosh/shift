# Contributing to SHIFT

<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->

- [Creating a Pull Request](#creating-a-pull-request)
- [Bug Reporting](#bug-reporting)
- [Use a Consistent Coding Style](#use-a-consistent-coding-style)
- [License](#license)
- [Maintainer (Contact)](#maintainer-contact)
- [References](#references)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

I want to make contributing to this project as easy and transparent as
possible, whether it's:

- Reporting a bug
- Discussing the current state of the code
- Submitting a fix
- Proposing new features (see NOTE below)

NOTE: Although there is room for new feature requests in the issue tracker, I
would prefer not to overwhelm the tracker with features planned in these
documents, so please limit to bug reporting for now.

SHIFT is in early stages of development and may shift direction (pun?) and
experience large overhauls from time to time.

Please have a look through the [to-do list](TODO.md) and the [SHIFT
specification](doc/ShiftSpec.md) to get an idea of what is already planned. 

## Creating a Pull Request

Use [Github
Flow](https://docs.github.com/en/get-started/quickstart/github-flow). All
external code changes happen through pull requests:

1. Fork the repo and create your branch from `master`.  
2. If you've added code that should be tested, add test scripts to `game/test`.
3. If you've changed APIs, update the documentation.  
4. Ensure the test suite passes. (No automated testing yet, but run through the
tests in `game/test` and verify that the results match the `intro` defined in
the `.shift` file.)
5. Make sure your code compiles and runs. If possible, check multiple
environments.
6. Issue that pull request!

## Bug Reporting

Report bugs through the GitHub [issue
tracker](https://github.com/RetroIndieJosh/shift/issues) by [opening a new
issue](https://github.com/RetroIndieJosh/shift/issues/new). It's that easy!

Here's an [example of a great bug report by Craig
Hockenberry](http://www.openradar.me/11905408).

**Great bug reports** tend to have:

- A quick summary and/or background
- Steps to reproduce
    - Be specific!
    - Give sample code if possible
- What you expected to happen
- What *actually* happens
- Notes (why might this be happening? what did you try that didn't work? any
ideas on a proposed fix?)

I *love* thorough bug reports.

## Use a Consistent Coding Style

- If you use VS Code, use the included .editorconfig file to get started 
- A code style guide is forthcoming 
- For now, try to match the style of existing code

## License

Any contributions you make will be under the GPL v3 software license. When you
submit code changes, your submissions are understood to be under the same [GPL
v3 License](LICENSE.txt) that covers the project. Please contact the maintainer
if this is a concern.

## Maintainer (Contact)

I (Joshua McLean) am the sole maintainer of SHIFT and you can reach me at
`mrjoshuamclean@gmail.com` with any concerns you feel do not belong in the
issue tracker.

## References

This document is adapted from the [MIT-licensed Transciptase contribution
guidelines](https://gist.github.com/briandk/3d2e8b3ec8daf5a27a62), which were
adapted from the open-source contribution guidelines for [Facebook's
Draft](https://github.com/facebook/draft-js/blob/main/CONTRIBUTING.md).
