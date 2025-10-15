"""Simple command-line calculator supporting basic arithmetic operations."""
from __future__ import annotations

import operator
from dataclasses import dataclass
from typing import Callable, Dict


@dataclass(frozen=True)
class Operation:
    symbol: str
    function: Callable[[float, float], float]

    def __call__(self, left: float, right: float) -> float:
        return self.function(left, right)


class CalculatorError(Exception):
    """Custom exception type for calculator errors."""


class Calculator:
    """A simple calculator supporting +, -, *, and / operations."""

    _OPERATIONS: Dict[str, Operation] = {
        "+": Operation("+", operator.add),
        "-": Operation("-", operator.sub),
        "*": Operation("*", operator.mul),
        "/": Operation("/", operator.truediv),
    }

    def calculate(self, expression: str) -> float:
        """Evaluate a simple binary expression like "2 + 3".

        Args:
            expression: A string containing two operands and an operator.

        Returns:
            The result of applying the operator to the operands.

        Raises:
            CalculatorError: If the expression format is invalid or division by zero occurs.
        """

        tokens = expression.strip().split()
        if len(tokens) != 3:
            raise CalculatorError(
                "Expression must consist of two operands and one operator separated by spaces."
            )

        left_str, operator_symbol, right_str = tokens

        operation = self._OPERATIONS.get(operator_symbol)
        if operation is None:
            raise CalculatorError(f"Unsupported operator: {operator_symbol}")

        try:
            left = float(left_str)
            right = float(right_str)
        except ValueError as exc:
            raise CalculatorError("Operands must be numbers.") from exc

        if operator_symbol == "/" and right == 0:
            raise CalculatorError("Division by zero is not allowed.")

        return operation(left, right)


def main() -> None:
    import argparse

    parser = argparse.ArgumentParser(description="Simple calculator for basic arithmetic")
    parser.add_argument(
        "expression",
        nargs="?",
        help="Expression to evaluate in the form 'A op B' (e.g., '2 + 2').",
    )

    args = parser.parse_args()
    calculator = Calculator()

    if args.expression:
        try:
            result = calculator.calculate(args.expression)
        except CalculatorError as error:
            parser.error(str(error))
        else:
            print(result)
            return

    # Interactive mode when no expression argument is provided.
    print("Enter expressions like '2 + 2'. Type 'quit' to exit.")
    while True:
        try:
            user_input = input(">> ").strip()
        except (EOFError, KeyboardInterrupt):
            print()
            break

        if user_input.lower() in {"quit", "exit"}:
            break

        if not user_input:
            continue

        try:
            result = calculator.calculate(user_input)
        except CalculatorError as error:
            print(f"Error: {error}")
        else:
            print(result)


if __name__ == "__main__":
    main()
