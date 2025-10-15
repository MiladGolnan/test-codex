import pytest

from calculator import Calculator, CalculatorError


@pytest.fixture()
def calculator():
    return Calculator()


def test_addition(calculator):
    assert calculator.calculate("2 + 3") == 5


def test_subtraction(calculator):
    assert calculator.calculate("10 - 4") == 6


def test_multiplication(calculator):
    assert calculator.calculate("6 * 7") == 42


def test_division(calculator):
    assert calculator.calculate("8 / 2") == 4


def test_division_by_zero(calculator):
    with pytest.raises(CalculatorError):
        calculator.calculate("8 / 0")


def test_invalid_operator(calculator):
    with pytest.raises(CalculatorError):
        calculator.calculate("5 ^ 2")


def test_invalid_format(calculator):
    with pytest.raises(CalculatorError):
        calculator.calculate("5 +")


def test_non_numeric_operands(calculator):
    with pytest.raises(CalculatorError):
        calculator.calculate("five + two")
