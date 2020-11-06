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
            // vratiti kalkulator
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
        None,
        Number,
        Operator
    }

    [DebuggerDisplay("{State}")]
    internal class Display
    {
        private const int MaxDigits = 10;
        private static readonly CultureInfo Culture = new CultureInfo("hr-HR");

        private readonly List<char> _characters;

        public Display()
        {
            _characters = new List<char> { '0' };
        }

        public IEnumerable<char> Characters
        {
            get
            {
                return _characters;
            }
        }

        public bool IsZero
        {
            get
            {
                return _characters.Count == 1 && _characters[0] == '0';
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
                _characters.Clear();
                _characters.AddRange(value.ToString(Culture));
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

        public void Clear()
        {
            _characters.Clear();
            _characters.Add('0');
        }

        public void Append(char c)
        {
            _characters.Add(c);
        }

        public void AppendDigit(char c)
        {
            if (!char.IsDigit(c))
                throw new ArgumentException("Argument is not a digit", "c");

            if (IsZero)
            {
                Set(c);
            }
            else if (_characters.Count(char.IsDigit) < MaxDigits)
            {
                _characters.Add(c);
            }
        }

        public void Set(char c)
        {
            _characters.Clear();
            _characters.Add(c);
        }

        public void Perform(Func<double, double> func)
        {
            double value = func(Value);
            if (!double.IsInfinity(value) && TryRestrict(ref value))
            {
                Value = value;
            }
            else
            {
                State = "-E-";
            }
        }

        private bool TryRestrict(ref double value)
        {
            if (value == 0)
            {
                return true;
            }

            double absv = Math.Abs(value);
            int digits = absv <= 1
                ? 1
                : (int)Math.Floor(Math.Log10(absv)) + 1;

            if (digits > MaxDigits)
            {
                return false;
            }

            value = Math.Round(value, MaxDigits - digits);
            return true;
        }
    }


    public class Kalkulator : ICalculator
    {
        private readonly Display _display;
        private double _result;
        private double _savedValue;
        private Operator _lastOperator;
        private InputType _lastInput;

        public Kalkulator()
        {
            _display = new Display();
            Reset();
        }

        private void Reset()
        {
            _result = 0;
            _savedValue = 0;
            _lastOperator = Operator.None;
            _lastInput = InputType.None;
            _display.Clear();
        }

        public void Press(char c)
        {
            if (char.IsDigit(c))
            {
                if (_lastInput == InputType.Number)
                {
                    _display.AppendDigit(c);
                }
                else
                {
                    _display.Set(c);
                    _lastInput = InputType.Number;
                }
            }
            else
            {
                switch (c)
                {
                    case ',':
                        if (_lastInput == InputType.Number)
                        {
                            if (!_display.Characters.Contains(c))
                                _display.Append(c);
                        }
                        else
                        {
                            _result = 0;
                            _display.Clear();
                            _display.Append(c);
                        }
                        _lastInput = InputType.Number;
                        break;

                    case '+': ProcessBinaryOperator(Operator.Plus); break;
                    case '-': ProcessBinaryOperator(Operator.Minus); break;
                    case '*': ProcessBinaryOperator(Operator.Multiply); break;
                    case '/': ProcessBinaryOperator(Operator.Divide); break;

                    // Change sign
                    case 'M':
                        //if (_lastInput == InputType.Number)
                        if (!_display.IsZero)
                        {
                            _display.Perform(x => -x);
                        }
                        break;

                    case 'S': _display.Perform(Math.Sin); break;
                    case 'K': _display.Perform(Math.Cos); break;
                    case 'T': _display.Perform(Math.Tan); break;
                    case 'Q': _display.Perform(x => x * x); break;
                    case 'R': _display.Perform(Math.Sqrt); break;
                    case 'I': _display.Perform(x => 1 / x); break;

                    case '=': ExecuteOperation(); break;

                    case 'O': Reset(); break;
                    case 'C': _display.Clear(); break;

                    case 'P': _savedValue = _display.Value; break;
                    case 'G': _display.Value = _savedValue; break;
                }
            }
        }
        private void ExecuteOperation()
        {
            switch (_lastOperator)
            {
                case Operator.None: _display.Perform(x => x); break;
                case Operator.Plus: _display.Perform(x => _result + x); break;
                case Operator.Minus: _display.Perform(x => _result - x); break;
                case Operator.Multiply: _display.Perform(x => _result * x); break;
                case Operator.Divide: _display.Perform(x => _result / x); break;
            }

            if (!_display.TryGetValue(out _result))
                _result = 0;
        }

        private void ProcessBinaryOperator(Operator op)
        {
            if (_lastInput != InputType.Operator)
                ExecuteOperation();

            _lastOperator = op;
            _lastInput = InputType.Operator;
        }

        public string GetCurrentDisplayState()
        {
            return _display.State;
        }
    }
}
