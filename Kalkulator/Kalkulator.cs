using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace PrvaDomacaZadaca_Kalkulator
{
    public class Factory
    {
        public static ICalculator CreateCalculator()
        {
            return new Kalkulator();
        }
    }

    internal enum InputType
    {
        Number,
        BinaryOperator,
        UnaryOperator,
        Equals
    }

    [DebuggerDisplay("{State}")]
    internal class Display
    {
        public const char DecimalPoint = ',';
        private const string ErrorState = "-E-";
        private const int MaxDigits = 10;
        private static readonly string NumberFormat = "F" + (MaxDigits - 1);
        private static readonly CultureInfo Culture = new CultureInfo("hr-HR");

        private readonly List<char> _characters;

        public Display()
        {
            _characters = new List<char> { '0' };
        }

        public bool IsZeroState
        {
            get
            {
                return _characters.Count == 1 && _characters[0] == '0';
            }
        }

        public bool IsErrorState
        {
            get
            {
                return State == ErrorState;
            }
        }

        public double Value
        {
            get
            {
                return double.Parse(new string(_characters.ToArray()), Culture);
            }
        }

        public string State
        {
            get
            {
                return new string(_characters.ToArray());
            }
            set
            {
                _characters.Clear();
                _characters.AddRange(value);
            }
        }

        public bool TryGetValue(out double result)
        {
            return double.TryParse(new string(_characters.ToArray()), NumberStyles.Float, Culture, out result);
        }

        public bool TrySetValue(ref double value)
        {
            if (value == 0)
                value = 0; // make sure it's positive zero

            if (TryRestrict(ref value))
            {
                string chars = value.ToString(NumberFormat, Culture)
                    .TrimEnd('0')
                    .TrimEnd(DecimalPoint);

                State = chars;
                return true;
            }
            else
            {
                State = ErrorState;
                return false;
            }
        }

        public void Clear()
        {
            _characters.Clear();
            _characters.Add('0');
        }

        public void AppendDigit(char ditit)
        {
            if (!char.IsDigit(ditit))
                throw new ArgumentException("Character is not a digit", "c");

            if (IsZeroState)
            {
                Set(ditit);
            }
            else if (_characters.Count(char.IsDigit) < MaxDigits)
            {
                _characters.Add(ditit);
            }
        }

        public void AppendDecimal()
        {
            if (!_characters.Contains(DecimalPoint))
                _characters.Add(DecimalPoint);
        }

        public void Set(char c)
        {
            _characters.Clear();
            _characters.Add(c);
        }

        public void Perform(Func<double, double> func)
        {
            double value;
            if (!TryGetValue(out value))
                return;

            value = func(value);

            TrySetValue(ref value);
        }

        private static bool TryRestrict(ref double value)
        {
            if (double.IsInfinity(value) || double.IsNaN(value))
                return false;

            if (value == 0)
                return true;

            double absv = Math.Abs(value);
            int digits = absv <= 1
                ? 1
                : (int)Math.Floor(Math.Log10(absv)) + 1;

            if (digits > MaxDigits)
                return false;

            value = Math.Round(value, MaxDigits - digits);
            return true;
        }
    }

    public class Kalkulator : ICalculator
    {
        private readonly Display _display;
        private double _result;
        private double _savedValue;
        private double _lastOperand;
        private InputType _lastInputType;
        private Func<double, double, double> _lastOperation;

        public Kalkulator()
        {
            _display = new Display();
            Initialize();
        }

        private void Initialize()
        {
            _result = 0;
            _savedValue = 0;
            _lastOperand = 0;
            _lastOperation = (x, y) => y;
            _lastInputType = InputType.Number;
            _display.Clear();
        }

        public void Press(char c)
        {
            if (c == 'O')
            {
                Initialize();
            }
            else if (_display.IsErrorState)
            {
                return;
            }
            else if (char.IsDigit(c))
            {
                ProcessDigit(c);
            }
            else
            {
                switch (c)
                {
                    case ',': ProcessDecimal(); break;

                    case '=': ProcessEquals(); break;

                    case '+': ProcessBinaryOperator((x, y) => x + y); break;
                    case '-': ProcessBinaryOperator((x, y) => x - y); break;
                    case '*': ProcessBinaryOperator((x, y) => x * y); break;
                    case '/': ProcessBinaryOperator((x, y) => x / y); break;

                    case 'M': _display.Perform(x => -x); break;

                    case 'S': ProcessUnaryOperator(Math.Sin); break;
                    case 'K': ProcessUnaryOperator(Math.Cos); break;
                    case 'T': ProcessUnaryOperator(Math.Tan); break;
                    case 'Q': ProcessUnaryOperator(x => x * x); break;
                    case 'R': ProcessUnaryOperator(Math.Sqrt); break;
                    case 'I': ProcessUnaryOperator(x => 1 / x); break;

                    case 'P': _savedValue = _display.Value; break;
                    case 'G':
                        _display.TrySetValue(ref _savedValue);
                        _lastInputType = InputType.Number;
                        break;

                    case 'C':
                        if (_lastInputType != InputType.BinaryOperator)
                            _display.Clear();
                        break;
                }
            }
        }

        private void ProcessDigit(char digit)
        {
            if (_lastInputType == InputType.Number)
            {
                _display.AppendDigit(digit);
            }
            else
            {
                _display.Set(digit);
                _lastInputType = InputType.Number;
            }
        }

        private void ProcessDecimal()
        {
            if (_lastInputType != InputType.Number)
                _display.Clear();

            _display.AppendDecimal();
            _lastInputType = InputType.Number;
        }

        private void ProcessEquals()
        {
            ExecuteOperation();
            _lastInputType = InputType.Equals;
        }

        private void ExecuteOperation()
        {
            if (_lastInputType != InputType.Equals)
                _lastOperand = _display.Value;

            _result = _lastOperation(_result, _lastOperand);
            _display.TrySetValue(ref _result);
        }

        private void ProcessBinaryOperator(Func<double, double, double> func)
        {
            // Ignore repeating of operators, only perform last
            if (_lastInputType != InputType.Equals && _lastInputType != InputType.BinaryOperator)
                ExecuteOperation();

            _lastOperation = func;
            _lastInputType = InputType.BinaryOperator;
        }

        private void ProcessUnaryOperator(Func<double, double> func)
        {
            _display.Perform(func);
            _lastInputType = InputType.UnaryOperator;
        }

        public string GetCurrentDisplayState()
        {
            return _display.State;
        }
    }
}
