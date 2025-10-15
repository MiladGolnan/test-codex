# Simple Calculator

This repository contains a small command-line calculator written in Python. It supports
basic arithmetic operations (addition, subtraction, multiplication, and division) and can
be used either by providing an expression as a command-line argument or in an interactive
REPL-style mode.

## Requirements

- Python 3.8+
- `pytest` for running the tests

## Usage

Evaluate an expression directly from the command line:

```bash
python calculator.py "2 + 2"
```

If you run the script without arguments, it enters an interactive mode:

```bash
python calculator.py
```

You can then type expressions like `3 * 7` or `10 / 5`. Type `quit` or `exit` to leave.

## Running Tests

Install `pytest` if it is not already installed and run the tests:

```bash
pip install pytest
pytest
```
