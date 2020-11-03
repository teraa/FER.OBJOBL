namespace PrvaDomacaZadaca_Kalkulator
{
    public interface ICalculator
    {
        void Press(char inPressedDigit);   // preko ovoga se Kalkulatoru zadaje koja je tipka pritisnuta
        string GetCurrentDisplayState();   // vraća trenutno stanje displeya Kalkulatora
    }
}
