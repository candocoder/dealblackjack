using System.Collections.Generic;

namespace BlackjackPayouts
{
    public class Item
    {
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
