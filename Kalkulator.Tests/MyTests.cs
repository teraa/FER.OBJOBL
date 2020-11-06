using PrvaDomacaZadaca_Kalkulator;
using System.Globalization;
using System.Threading;
using Xunit;

namespace Kalkulator.Tests
{
    public class MyTests
    {
        static MyTests()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("hr-HR");
        }

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
            c.PressCheck('+', "0");
            c.PressCheck(',', "0,");
            c.PressCheck('+', "0");
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
