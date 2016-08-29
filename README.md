# git_edit
Simple text editor for Git.

## Features
- Quick launch.
- Syntax highlighting:
    - `COMMIT_EDITMSG` with diff (See ``git commit -v``).
    - `.gitignore`
- Input completion:
    - ``Ctrl+Space`` to open the window.
- ``Ctrl+W`` to save and exit.

## Install
Download [the latest binary](https://github.com/vain0/git_edit/releases/latest).

Set git_edit as your editor for git:

```
# To use git_edit for current repository.
git config --local core.editor "the-unarchived-directory/git_edit.exe"

# To use git_edit for all repositories.
git config --global core.editor "the-unarchived-directory/git_edit.exe"
```

## Libraries
- [AvalonEdit](https://github.com/icsharpcode/AvalonEdit)

## License
- [MIT License](LICENSE.md)
