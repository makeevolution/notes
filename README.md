3# notes

## Notes on shortcuts/lessons learned for different topics
exclude: '^($|.*.bin)'
fail_fast: false
default_stages: [push, commit]

repos:
  - repo: local
    hooks:
      - id: seed-isort-config
        name: seed-isort-config
        entry: seed-isort-config
        language: python

  - repo: local
    hooks:
      - id: check-added-large-files
        name: check-added-large-files
        entry: check-added-large-files
        language: python
        args: ['--maxkb=50000']
      - id: check-case-conflict
        name: check-case-conflict
        entry: check-case-conflict
        language: python
      - id: check-json
        name: check-json
        entry: check-json
        language: python
      - id: debug-statements
        name: debug-statements
        entry: debug-statements
        language: python
      - id: check-symlinks
        name: check-symlinks
        entry: check-symlinks
        language: python
      - id: check-merge-conflict
        name: check-merge-conflict
        entry: check-merge-conflict
        language: python
      - id: name-tests-test
        name: name-tests-test
        entry: name-tests-test
        language: python
        exclude: '^backend/tests/unittests/helpers/'
        args: ['--django']
      - id: requirements-txt-fixer
        name: requirements-txt-fixer
        entry: requirements-txt-fixer
        language: python
      - id: trailing-whitespace
        name: trailing-whitespace
        entry: trailing-whitespace
        language: python
      - id: end-of-file-fixer
        name: end-of-file-fixer
        entry: end-of-file-fixer
        language: python
      - id: debug-statements
        name: debug-statements
        entry: debug-statements
        language: python
        exclude: 'static'
      - id: fix-encoding-pragma
        name: fix-encoding-pragma
        entry: fix-encoding-pragma
        language: python
        args: ['--remove']
      - id: mixed-line-ending
        name: mixed-line-ending
        entry: mixed-line-ending
        language: python
        args: ['--fix=crlf']
      - id: sort-simple-yaml
        name: sort-simple-yaml
        entry: sort-simple-yaml
        language: python

  - repo: local
    hooks:
      - id: isort
        name: isort
        entry: isort
        language: python

  - repo: local
    hooks:
      - id: black
        name: black
        entry: black
        language: python
        args: [--safe, --quiet]

  - repo: local
    hooks:
      - id: flake8
        name: flake8
        entry: flake8
        language: python
        additional_dependencies: 
          - flake8-blind-except
          - flake8-bugbear
          - flake8-comprehensions
          - flake8-debugger
          - flake8-docstrings
          - flake8-isort
          - flake8-polyfill
          - flake8-pytest
          - flake8-quotes
          - yesqa
          - wemake_python_styleguide

  - repo: local
    hooks:
      - id: prospector
        name: prospector
        entry: prospector
        language: system
        files: .py$
        args: [ --profile=.prospector.yml, -X ] # -X to show stacktrace upon error while running prospector

  - repo: local
    hooks:
      - id: pylint
        name: pylint
        entry: pylint backend --rcfile=.pylintrc
        language: python
        types: [python]

  - repo: local
    hooks:
      - id: mypy
        name: mypy
        entry: mypy backend --config-file setup.cfg --exclude backend/venv/*
        language: system
        types: [python]
        pass_filenames: false

  - repo: local
    hooks:
      - id: pytest-cov
        name: pytest Cov backend
        entry: pytest backend/tests/unittests --cov=backend --cov-fail-under=100 --cov-branch
        language: system
        types: [python]
        pass_filenames: false

  - repo: local
    hooks:
      - id: pytest-cov
        name: pytest Cov user_management
        entry: pytest authorization/tests/unittests/ --cov=authorization --cov-fail-under=100 --cov-branch --ds=authorization.settings
        language: system
        types: [python]
        pass_filenames: false

  - repo: local
    hooks:
      - id: nitpick
        name: nitpick
        entry: nitpick
        language: python

  - repo: https://github.com/asottile/add-trailing-comma
    rev: v3.1.0
    hooks:
      - id: add-trailing-comma

  - repo: local
    hooks:
      - id: blacken-docs
        name: blacken-docs
        entry: blacken-docs
        language: python
        additional_dependencies: ['black==20.8b1']

  - repo: local
    hooks:
      - id: darglint
        name: darglint
        entry: darglint
        language: python

  - repo: local
    hooks:
      - id: pyupgrade
        name: pyupgrade
        entry: pyupgrade
        language: python

  - repo: local
    hooks:
      - id: python-check-blanket-noqa
        name: python-check-blanket-noqa
        entry: python-check-blanket-noqa
        language: python
      - id: python-check-mock-methods
        name: python-check-mock-methods
        entry: python-check-mock-methods
        language: python
      - id: python-no-eval
        name: python-no-eval
        entry: python-no-eval
        language: python
      - id: python-no-log-warn
        name: python-no-log-warn
        entry: python-no-log-warn
        language: python

pip install seed-isort-config==2.2.0 pre-commit-hooks==4.1.0 isort==5.10.1 black==22.6.0 flake8==3.9.2 flake8-blind-except flake8-bugbear flake8-comprehensions flake8-debugger flake8-docstrings flake8-isort flake8-polyfill flake8-pytest flake8-quotes yesqa wemake-python-styleguide prospector pylint mypy pytest pytest-cov nitpick==0.23.0 blacken-docs==1.8.0 black==20.8b1 darglint==1.8.1 pyupgrade==2.30.1
