using System.Collections.Generic;

namespace BlackjackPayouts
{
    public class Item
    {
        public static decimal[] Tests = { 5, 95, 875, 37, 650, 8, 425, 15, 47, 29, 60, 19, 250, 13, 100, 38, 75, 1000, 23, 525, 42, 600, 30, 150, 6, 300, 34, 78, 67, 21, 88, 900, 500,
                                          41, 90, 66, 7, 700, 66, 18, 825, 125, 35, 44, 14, 48, 31, 39, 850, 45, 450, 85, 9, 350, 16, 40, 28, 65, 22, 225, 32, 725, 46, 925, 4, 24,
                                           9, 325, 550, 400, 10, 55, 800, 26, 275, 625, 20, 43, 33, 1, 70, 37.5m, 25, 17, 950, 49, 80, 11, 750, 92, 36, 111, 87, 3, 200, 50, 27, 375, 12 };

        const string BET = "Bet";
        const string PAYOUT = "Payout";

        public decimal Bet { get; set; }
        public decimal Payout { get; set; }

        public Item( decimal _bet, decimal _ratio )
        {
            Bet = _bet;
            
            // Convert to 50 cent pieces and back.
            // This rounds down to the nearest 50 cents
            // (quarters aren't used)

            Payout = (int)(_bet * 2 * _ratio) / 2m;
        }

        public static string GetFormatedText (Item item)
        {
            string text = BET +": " + item.Bet.ToString() + "\n";
            text += PAYOUT + ": " +  item.Payout.ToString() + "\n";

            return text;
        }
    }
}
