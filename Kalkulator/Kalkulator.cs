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

    internal enum Operator
    {
        None,
        Plus,
        Minus,
        Multiply,
        Divide,
    }

    internal enum InputType
    {
        Number,
        Operator,
        Equals
    }

    [DebuggerDisplay("{State}")]
    internal class Display
    {
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
            set
            {
                string chars = value.ToString(NumberFormat, Culture).TrimEnd('0');
                if ((value % 1) == 0)
                    chars = chars.TrimEnd(',');

                _characters.Clear();
                _characters.AddRange(chars);
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
                Value = value;
                return true;
            }

            State = ErrorState;
            return false;
        }

        public void Clear()
        {
            _characters.Clear();
            _characters.Add('0');
        }

        public void AppendDigit(char c)
        {
            if (!char.IsDigit(c))
                throw new ArgumentException("Argument is not a digit", "c");

            if (IsZeroState)
            {
                Set(c);
            }
            else if (_characters.Count(char.IsDigit) < MaxDigits)
            {
                _characters.Add(c);
            }
        }

        public void AppendDecimal()
        {
            if (!_characters.Contains(','))
                _characters.Add(',');
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

        private bool TryRestrict(ref double value)
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
        private Operator _lastOperator;
        private InputType _lastInputType;

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
            _lastOperator = Operator.None;
            _lastInputType = InputType.Number;
            _display.Clear();
        }

        public void Press(char c)
        {
            if (c == 'O')
                Initialize();
            else if (_display.IsErrorState)
                return;

            if (char.IsDigit(c))
            {
                if (_lastInputType == InputType.Number)
                {
                    _display.AppendDigit(c);
                }
                else
                {
                    _display.Set(c);
                    _lastInputType = InputType.Number;
                }
            }
            else
            {
                switch (c)
                {
                    case ',':
                        if (_lastInputType == InputType.Number)
                        {
                            _display.AppendDecimal();
                        }
                        else
                        {
                            _display.Clear();
                            _display.AppendDecimal();
                        }
                        _lastInputType = InputType.Number;
                        break;

                    case '+': ProcessBinaryOperator(Operator.Plus); break;
                    case '-': ProcessBinaryOperator(Operator.Minus); break;
                    case '*': ProcessBinaryOperator(Operator.Multiply); break;
                    case '/': ProcessBinaryOperator(Operator.Divide); break;

                    case '=':
                        ExecuteOperation();
                        _lastInputType = InputType.Equals;
                        break;

                    case 'M': _display.Perform(x => -x); break;
                    case 'S': _display.Perform(Math.Sin); break;
                    case 'K': _display.Perform(Math.Cos); break;
                    case 'T': _display.Perform(Math.Tan); break;
                    case 'Q': _display.Perform(x => x * x); break;
                    case 'R': _display.Perform(Math.Sqrt); break;
                    case 'I': _display.Perform(x => 1 / x); break;

                    case 'P': _savedValue = _display.Value; break;
                    case 'G': _display.Value = _savedValue; break;

                    case 'C': _display.Clear(); break;
                }
            }
        }
        private void ExecuteOperation()
        {
            if (_lastInputType != InputType.Equals)
                _lastOperand = _display.Value;

            switch (_lastOperator)
            {
                case Operator.None: _result = _lastOperand; break;
                case Operator.Plus: _result += _lastOperand; break;
                case Operator.Minus: _result -= _lastOperand; break;
                case Operator.Multiply: _result *= _lastOperand; break;
                case Operator.Divide: _result /= _lastOperand; break;
            }

            _display.TrySetValue(ref _result);
        }

        private void ProcessBinaryOperator(Operator op)
        {
            // Ignore repeating of operators, only perform last
            if (_lastInputType != InputType.Equals && _lastInputType != InputType.Operator)
                ExecuteOperation();

            _lastOperator = op;
            _lastInputType = InputType.Operator;
        }

        public string GetCurrentDisplayState()
        {
            return _display.State;
        }
    }
}
