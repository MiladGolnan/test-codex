using System;
using System.Globalization;
using System.Windows.Forms;

namespace CalculatorWinForms
{
    public class CalculatorForm : Form
    {
        private readonly TextBox _display;
        private decimal? _leftOperand;
        private string? _pendingOperator;
        private bool _resetDisplay;

        public CalculatorForm()
        {
            Text = "ماشین حساب";
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new System.Drawing.Size(320, 420);

            _display = new TextBox
            {
                Dock = DockStyle.Top,
                ReadOnly = true,
                Text = "0",
                TextAlign = HorizontalAlignment.Right,
                Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point),
                BorderStyle = BorderStyle.FixedSingle
            };

            Controls.Add(_display);

            var buttonGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 5,
                Padding = new Padding(8),
                BackColor = System.Drawing.Color.FromArgb(245, 245, 245)
            };

            for (int i = 0; i < 4; i++)
            {
                buttonGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            }

            for (int i = 0; i < 5; i++)
            {
                buttonGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            }

            Controls.Add(buttonGrid);

            string[,] buttons = new string[5, 4]
            {
                { "C", "CE", "±", "÷" },
                { "7", "8", "9", "×" },
                { "4", "5", "6", "-" },
                { "1", "2", "3", "+" },
                { "0", "0", ".", "=" }
            };

            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    if (row == 4 && col == 1)
                    {
                        continue; // handled by the first 0 button spanning two columns
                    }

                    var button = CreateButton(buttons[row, col]);

                    if (row == 4 && col == 0)
                    {
                        buttonGrid.Controls.Add(button, col, row);
                        buttonGrid.SetColumnSpan(button, 2);
                    }
                    else
                    {
                        buttonGrid.Controls.Add(button, col, row);
                    }
                }
            }
        }

        private Button CreateButton(string text)
        {
            var button = new Button
            {
                Text = text,
                Dock = DockStyle.Fill,
                Margin = new Padding(4),
                Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            };

            button.Click += OnButtonClick;
            return button;
        }

        private void OnButtonClick(object? sender, EventArgs e)
        {
            if (sender is not Button button)
            {
                return;
            }

            string input = button.Text;

            if (char.IsDigit(input, 0))
            {
                AppendDigit(input);
                return;
            }

            switch (input)
            {
                case ".":
                    AppendDecimalSeparator();
                    break;
                case "C":
                    ClearAll();
                    break;
                case "CE":
                    ClearEntry();
                    break;
                case "±":
                    ToggleSign();
                    break;
                case "÷":
                case "×":
                case "-":
                case "+":
                    ApplyOperator(input);
                    break;
                case "=":
                    CalculateResult();
                    break;
            }
        }

        private void AppendDigit(string digit)
        {
            if (_resetDisplay || _display.Text == "0")
            {
                _display.Text = digit;
                _resetDisplay = false;
                return;
            }

            _display.Text += digit;
        }

        private void AppendDecimalSeparator()
        {
            string separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            if (_resetDisplay)
            {
                _display.Text = "0" + separator;
                _resetDisplay = false;
                return;
            }

            if (_display.Text.Contains(separator))
            {
                return;
            }

            _display.Text += separator;
        }

        private void ClearAll()
        {
            _display.Text = "0";
            _leftOperand = null;
            _pendingOperator = null;
            _resetDisplay = false;
        }

        private void ClearEntry()
        {
            _display.Text = "0";
            _resetDisplay = false;
        }

        private void ToggleSign()
        {
            if (!decimal.TryParse(_display.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal value))
            {
                return;
            }

            value = -value;
            _display.Text = value.ToString(CultureInfo.CurrentCulture);
        }

        private void ApplyOperator(string @operator)
        {
            if (!_resetDisplay)
            {
                if (_leftOperand.HasValue && _pendingOperator is not null)
                {
                    CalculateResult();
                }
                else
                {
                    _leftOperand = ParseDisplay();
                }
            }

            _pendingOperator = @operator;
            _resetDisplay = true;
        }

        private void CalculateResult()
        {
            if (_pendingOperator is null)
            {
                return;
            }

            if (!_leftOperand.HasValue)
            {
                _leftOperand = ParseDisplay();
            }

            decimal rightOperand = ParseDisplay();
            decimal result;

            try
            {
                result = _pendingOperator switch
                {
                    "÷" => Divide(_leftOperand!.Value, rightOperand),
                    "×" => _leftOperand!.Value * rightOperand,
                    "-" => _leftOperand!.Value - rightOperand,
                    "+" => _leftOperand!.Value + rightOperand,
                    _ => rightOperand
                };
            }
            catch (DivideByZeroException)
            {
                MessageBox.Show(this, "تقسیم بر صفر مجاز نیست.", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClearAll();
                return;
            }

            _display.Text = result.ToString(CultureInfo.CurrentCulture);
            _leftOperand = result;
            _pendingOperator = null;
            _resetDisplay = true;
        }

        private decimal ParseDisplay()
        {
            if (decimal.TryParse(_display.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal value))
            {
                return value;
            }

            return 0m;
        }

        private static decimal Divide(decimal left, decimal right)
        {
            if (right == 0m)
            {
                throw new DivideByZeroException();
            }

            return left / right;
        }
    }
}
