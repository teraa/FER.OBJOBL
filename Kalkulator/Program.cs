using System;

namespace PrvaDomacaZadaca_Kalkulator
{
    public class Program
    {
    static void Main(string[] args)
    {
        var calc = Factory.CreateCalculator();
        ConsoleKeyInfo key;
        while (true)
        {
            Console.WriteLine(calc.GetCurrentDisplayState());
            key = Console.ReadKey(true);
            calc.Press(key.KeyChar);
        }
    }
    }
}
