using PrvaDomacaZadaca_Kalkulator;
using Xunit;

namespace Kalkulator.Tests
{
    public class MyTests
    {
        [Fact]
        public void ZeroOnZero_NoChange()
        {
            var c = Factory.CreateCalculator();
            Assert.Equal("0", c.GetCurrentDisplayState());
            c.PressCheck('0', "0");
            c.PressCheck('0', "0");
        }

        [Fact]
        public void MaxDigits_IgnoreExtra()
        {
            var c = Factory.CreateCalculator();
            Assert.Equal("0", c.GetCurrentDisplayState());
            c.PressCheck('1', "1");
            c.PressCheck('2', "12");
            c.PressCheck('3', "123");
            c.PressCheck('4', "1234");
            c.PressCheck('5', "12345");
            c.PressCheck('6', "123456");
            c.PressCheck('7', "1234567");
            c.PressCheck('8', "12345678");
            c.PressCheck('9', "123456789");
            c.PressCheck('0', "1234567890");
            c.PressCheck('1', "1234567890");
        }

        [Fact]
        public void ChangeSignZero_NoChange()
        {
            var c = Factory.CreateCalculator();
            c.PressCheck('M', "0");
        }

        [Fact]
        public void ChangeSign_Various()
        {
            var c = Factory.CreateCalculator();
            c.PressCheck('2', "2");
            c.PressCheck('M', "-2");
            c.PressCheck('3', "-23");
            c.PressCheck('M', "23");
        }

        [Fact]
        public void CheckDisplay_PressSquare_SquareOfANumber()
        {
            var c = Factory.CreateCalculator();
            c.PressCheck('1', "1");
            c.PressCheck('2', "12");
            c.PressCheck('3', "123");
            c.PressCheck(',', "123,");
            c.PressCheck('4', "123,4");
            c.PressCheck('5', "123,45");
            c.PressCheck('Q', "15239,9025");
            c.PressCheck('=', "15239,9025");
        }

        /// <summary>
        /// Provjera {broj1} {binarni} {unarni} {broj1} = {broj1}{binarni}{broj2}
        /// unarni se izračuna i prikaže ali se ne uzima u obzir u binarnoj operaciji
        /// </summary>
        [Fact]
        public void CheckDisplay_PressUnaryOperatorAfterBinaryThenEqual_BinaryOperation()
        {
            var c = Factory.CreateCalculator();
            c.PressCheck('2', "2");
            c.PressCheck('+', "2");
            c.PressCheck('I', "0,5");
            c.PressCheck('3', "3");
            c.PressCheck('=', "5");
        }

        [Fact]
        public void SumTwoNumbers()
        {
            var c = Factory.CreateCalculator();
            c.PressCheck('2', "2");
            c.PressCheck(',', "2,");
            c.PressCheck('0', "2,0");
            c.PressCheck('+', "2");
            c.PressCheck('1', "1");
            c.PressCheck(',', "1,");
            c.PressCheck('0', "1,0");
            c.PressCheck('+', "3");
        }

        [Fact]
        public void DivideByZero_Error()
        {
            var c = Factory.CreateCalculator();
            c.PressCheck('2', "2");
            c.PressCheck('/', "2");
            c.PressCheck('0', "0");
            c.PressCheck('=', "-E-");
        }

        [Fact]
        public void RepeatBinaryOperator_DoNothing()
        {
            var c = Factory.CreateCalculator();
            c.PressCheck('2', "2");
            c.PressCheck(',', "2,");
            c.PressCheck('0', "2,0");
            c.PressCheck('+', "2");
            c.PressCheck('+', "2");
        }

        [Fact]
        public void DecimalPointAfterCalculation_ZeroPoint()
        {
            var c = Factory.CreateCalculator();
            c.PressCheck('3', "3");
            c.PressCheck(',', "3,");
            c.PressCheck('+', "3");
            c.PressCheck(',', "0,");
            c.PressCheck('+', "3");
            c.PressCheck(',', "0,");
            c.PressCheck('+', "3");
        }

        [Fact]
        public void MultipleDecimalPoints_Ignore()
        {
            var c = Factory.CreateCalculator();
            c.PressCheck('2', "2");
            c.PressCheck(',', "2,");
            c.PressCheck(',', "2,");
            c.PressCheck('0', "2,0");
            c.PressCheck(',', "2,0");
            c.PressCheck('1', "2,01");
            c.PressCheck(',', "2,01");
        }


        /// <summary>
        /// Provjera oduzimanja dva negativna broja
        /// </summary>
        [Fact]
        public void CheckDisplay_SubtractOfTwoNegaitiveNumbers_Subtract()
        {
            var c = Factory.CreateCalculator();
            c.PressCheck('4', "4");
            c.PressCheck('2', "42");
            c.PressCheck('7', "427");
            c.PressCheck('M', "-427"); //predznak je moguće dodati u bilo kojem trenutku
            c.PressCheck('8', "-4278");
            c.PressCheck('2', "-42782");
            c.PressCheck(',', "-42782,");
            c.PressCheck('5', "-42782,5");
            c.PressCheck('-', "-42782,5");
            c.PressCheck('1', "1");
            c.PressCheck('6', "16");
            c.PressCheck('M', "-16");
            c.PressCheck(',', "-16,");
            c.PressCheck('8', "-16,8");
            c.PressCheck('3', "-16,83");
            c.PressCheck('1', "-16,831");
            c.PressCheck('=', "-42765,669");
        }

        [Fact]
        public void RepeatEquals_LastOperation()
        {
            var c = Factory.CreateCalculator();
            c.PressCheck('1', "1");
            c.PressCheck('0', "10");
            c.PressCheck('-', "10");
            c.PressCheck('1', "1");
            c.PressCheck('=', "9");
            c.PressCheck('=', "8");
            c.PressCheck('=', "7");
            c.PressCheck('=', "6");
            c.PressCheck('=', "5");
            c.PressCheck('=', "4");
            c.PressCheck('=', "3");
            c.PressCheck('=', "2");
            c.PressCheck('=', "1");
            c.PressCheck('=', "0");
            c.PressCheck('=', "-1");
            c.PressCheck('=', "-2");
        }

        [Fact]
        public void RepeatEqualsThenOperator_DoNothing()
        {
            var c = Factory.CreateCalculator();
            c.PressCheck('5', "5");
            c.PressCheck('+', "5");
            c.PressCheck('=', "10");
            c.PressCheck('=', "15");
            c.PressCheck('/', "15");
        }

        [Fact]
        public void Doc_OrderOfOperations()
        {
            var c = Factory.CreateCalculator();
            c.PressCheck('2', "2");
            c.PressCheck('+', "2");
            c.PressCheck('3', "3");
            c.PressCheck('-', "5");
            c.PressCheck('2', "2");
            c.PressCheck('Q', "4");
            c.PressCheck('*', "1");
            c.PressCheck('2', "2");
            c.PressCheck('=', "2");
        }

        [Fact]
        public void InverseOfZero_Error()
        {
            var c = Factory.CreateCalculator();
            c.PressCheck('I', "-E-");
        }

        [Fact]
        public void RepeatEqualsAfterError_Error()
        {
            var c = Factory.CreateCalculator();
            c.PressCheck('5', "5");
            c.PressCheck('/', "5");
            c.PressCheck(',', "0,");
            c.PressCheck('=', "-E-");
            c.PressCheck('=', "-E-");
            c.PressCheck('=', "-E-");
        }

        [Fact]
        public void SecondOperatorDecimalPoint_NoError()
        {
            var c = Factory.CreateCalculator();
            c.PressCheck('7', "7");
            c.PressCheck('+', "7");
            c.PressCheck(',', "0,");
            c.PressCheck('=', "7");
        }

        [Fact]
        public void RoundNoOp()
        {
            var c = Factory.CreateCalculator();
            c.PressCheck('2', "2");
            c.PressCheck(',', "2,");
            c.PressCheck('0', "2,0");
            c.PressCheck('=', "2");
            c.PressCheck('=', "2");
        }

        // add tests for each math function
        // infinity test for math functions

        // +/- nakon "0," na irl calc mjenja predznak -0 0 -1 0
        // a dok je samo "0" ne mjenja

        [Fact]
        public void RestoreNumber_AllowAppending()
        {
            var c = Factory.CreateCalculator();
            c.PressCheck('8', "8");
            c.PressCheck('1', "81");
            c.PressCheck('3', "813");
            c.PressCheck('P', "813");
            c.PressCheck('C', "0");
            c.PressCheck('1', "1");
            c.PressCheck('+', "1");
            c.PressCheck('5', "5");
            c.PressCheck('G', "813");
            c.PressCheck(',', "813,");
            c.PressCheck('6', "813,6");
        }

        [Fact]
        public void SquareRootOfNegative_Error()
        {
            var c = Factory.CreateCalculator();
            c.PressCheck('4', "4");
            c.PressCheck('M', "-4");
            c.PressCheck('R', "-E-");
            c.PressCheck('=', "-E-");
            c.PressCheck('=', "-E-");
        }

        [Fact]
        public void Reset_ClearsSaved()
        {
            var c = Factory.CreateCalculator();
            c.PressCheck('7', "7");
            c.PressCheck('2', "72");
            c.PressCheck('P', "72");
            c.PressCheck('O', "0");
            c.PressCheck('G', "0");
        }

        [Fact]
        public void Clear_DoesNotClearSaved()
        {
            var c = Factory.CreateCalculator();
            c.PressCheck('7', "7");
            c.PressCheck('2', "72");
            c.PressCheck('P', "72");
            c.PressCheck('C', "0");
            c.PressCheck('G', "72");
        }

        [Fact]
        public void Error_IgnoreInput()
        {
            var c = Factory.CreateCalculator();
            c.PressCheck('5', "5");
            c.PressCheck('P', "5");
            c.PressCheck('/', "5");
            c.PressCheck('0', "0");
            c.PressCheck('=', "-E-");
            c.PressCheck('+', "-E-");
            c.PressCheck('=', "-E-");
            c.PressCheck('-', "-E-");
            c.PressCheck('*', "-E-");
            c.PressCheck('/', "-E-");
            c.PressCheck('M', "-E-");
            c.PressCheck('S', "-E-");
            c.PressCheck('K', "-E-");
            c.PressCheck('T', "-E-");
            c.PressCheck('Q', "-E-");
            c.PressCheck('R', "-E-");
            c.PressCheck('I', "-E-");
            c.PressCheck('G', "-E-");
            c.PressCheck('P', "-E-");
            c.PressCheck('C', "-E-");
            c.PressCheck('O', "0");
        }

        [Fact]
        public void SmallNumber_NoExponentialFormat()
        {
            var c = Factory.CreateCalculator();
            c.Press('9');
            c.Press('8');
            c.Press('7');
            c.Press('6');
            c.Press('/');
            c.Press('1');
            c.Press('0');
            c.PressCheck('=', "987,6");
            c.PressCheck('=', "98,76");
            c.PressCheck('=', "9,876");
            c.PressCheck('=', "0,9876");
            c.PressCheck('=', "0,09876");
            c.PressCheck('=', "0,009876");
            c.PressCheck('=', "0,0009876");
            c.PressCheck('=', "0,00009876");
            c.PressCheck('=', "0,000009876");
            c.PressCheck('=', "0,000000988");
            c.PressCheck('=', "0,000000099");
            c.PressCheck('=', "0,00000001");
            c.PressCheck('=', "0,000000001");
            c.PressCheck('=', "0");
        }
    }

    public static class Extensions
    {
        public static void PressCheck(this ICalculator calculator, char c, string expected)
        {
            calculator.Press(c);
            Assert.Equal(expected, calculator.GetCurrentDisplayState());
        }
    }
}
