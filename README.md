3# notes

## Notes on shortcuts/lessons learned for different topics

exclude: '^($|.*\.bin)'
fail_fast: false
default_stages: [push, commit]
repos:
  - repo: https://github.com/asottile/seed-isort-config
    rev: v2.2.0
    hooks:
      - id: seed-isort-config
  - repo: https://github.com/pre-commit/pre-commit-hooks
    rev: v4.1.0
    hooks:
      - id: check-added-large-files
        args: ['--maxkb=50000']
      - id: check-case-conflict
      - id: check-json
      - id: debug-statements
      - id: check-symlinks
      - id: check-merge-conflict
      - id: name-tests-test
        exclude: '^backend/tests/unittests/helpers/'
        args: ['--django']
      - id: requirements-txt-fixer
      - id: trailing-whitespace
      - id: end-of-file-fixer
      - id: debug-statements
        exclude: 'static'
      - id: fix-encoding-pragma
        args: ['--remove']
      - id: mixed-line-ending
        args: ['--fix=crlf']
      - id: sort-simple-yaml
  - repo: https://github.com/pre-commit/mirrors-isort
    rev: v5.10.1
    hooks:
      - id: isort
  - repo: https://github.com/ambv/black
    rev: 22.6.0
    hooks:
    - id: black
      args: [--safe, --quiet]
  - repo: https://github.com/PyCQA/flake8
    rev: 3.9.2
    hooks:
    - id: flake8
      additional_dependencies: [flake8-blind-except, flake8-bugbear, flake8-comprehensions,
                                flake8-debugger, flake8-docstrings, flake8-isort, flake8-polyfill,
                                flake8-pytest, flake8-quotes, yesqa, wemake_python_styleguide]
  - repo: https://github.com/guykisel/prospector-mirror
    rev: 7ff847e779347033ebbd9e3b88279e7f3a998b45
    hooks:
      - id: prospector
        language: system
        files: \.py$
        args: [ --profile=.prospector.yml, -X ]  # -X to show stacktrace upon error while running prospector
  - repo: local
    hooks:
    - id: pylint
      name: pylint
      language: python
      types: [python]
      entry: pylint backend --rcfile=.pylintrc
  - repo: https://github.com/asottile/add-trailing-comma
    rev: v3.1.0
    hooks:
    -   id: add-trailing-comma
  - repo: local
    hooks:
    - id: mypy
      name: mypy
      language: system
      entry: mypy backend --config-file setup.cfg --exclude backend/venv/*
      types: [python]
      pass_filenames: false
  - repo: https://github.com/asottile/blacken-docs
    rev: v1.8.0
    hooks:
    - id: blacken-docs
      additional_dependencies: [black==20.8b1]
  - repo: https://github.com/terrencepreilly/darglint
    rev: v1.8.1
    hooks:
    - id: darglint
  - repo: https://github.com/asottile/pyupgrade
    rev: v2.30.1
    hooks:
    - id: pyupgrade
  - repo: https://github.com/pre-commit/pygrep-hooks
    rev: v1.9.0
    hooks:
    - id: python-check-blanket-noqa
    - id: python-check-mock-methods
    - id: python-no-eval
    - id: python-no-log-warn
  - repo: local
    hooks:
    - id: pytest-cov
      name: pytest Cov backend
      language: system
      entry: pytest backend/tests/unittests --cov=backend --cov-fail-under=100 --cov-branch
      types: [ python ]
      pass_filenames: false
  - repo: local
    hooks:
      - id: pytest-cov
        name: pytest Cov user_management
        language: system
        entry: pytest authorization/tests/unittests/ --cov=authorization --cov-fail-under=100 --cov-branch --ds=authorization.settings
        types: [ python ]
        pass_filenames: false
  - repo: https://github.com/andreoliwa/nitpick
    rev: v0.23.0
    hooks:
    - id: nitpick
  - repo: https://github.com/avilaton/add-msg-issue-prefix-hook
    rev: v0.0.5
    hooks:
    - id: add-msg-issue-prefix
