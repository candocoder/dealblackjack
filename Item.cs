using System;
using System.Collections.Generic;

namespace Black
{
    public class Deal
    {
        public ScanTheLayout()
        {
            CheckForBet("Spot 6");
            CheckForBet("Spot 5");
            CheckForBet("Spot 4");
            CheckForBet("Spot 3");
            CheckForBet("Spot 2");
            CheckForBet("Spot 1");
        }

        public DealSpot3()
        {
            PullCardFromShoe()
            DropCardWithLeftHand();
        }

        public DealSpot2()
        {
            PullCardFromShoe()
            DropCardWithLeftHand();
        }

        public DealSpot2()
        {
            PullCardFromShoe()
            DropCardWithLeftHand();
        }

        public DealFirstBase()
        {
            PullCardFromShoe()
            DropCardWithLeftHand();
        }

        public DealFirstCard()
        {
            if (CheckForBet("Spot 1")  == true)
            {
                DealFirstBase();
            }          
            CheckForBet("Spot 5");
            CheckForBet("Spot 4");
            CheckForBet("Spot 3");
            CheckForBet("Spot 2");
        }

        public DealAHand()
        {
            if (TableOpen == false)
            {
                OpenTable();
            }
            if (YellowCardOut == true)
            {
                ShuffleCards();
            }

            ScanTheLayout()
            if()
        }
        public CountByFours()
        {
            Sac
        }

        public OpenTheTable()
        {
        }

        public CloseTheTable()
        {
        }

        public ScanTheLayout()
        {
        }

        public CheckTheTen()
        {
        }

        public OfferInsurance()
        {
        }
    }


    public class Random
    {
        private static System.Random rand;

        private Random()
        {
            rand = new System.Random();
        }

        public static int GetRandomNumber(int min, int max)
        {
            var rval = (int)Math.Floor((decimal)(rand.NextDouble() * (double)(max - min))) + min;

            if (rval < min) rval = min;
            if (rval > max) rval = max;

            return rval;
        }
    }

    public class Cheques
    {
        int fifties;

        public Cheques(Decimal dollars)
        {
            Dollars = dollars;
        }

        public int Fifties
        {
            get { return fifties; }
        }

        public void AddFifties(int number)
        {
            fifties += number;
        }

        public void AddWhites(int number)
        {
            fifties += number * 2;
        }

        public void AddReds(int number)
        {
            fifties += number * 5 * 2;
        }

        public void AddGreens(int number)
        {
            fifties += number * 25 * 2;
        }

        public void AddBlacks(int number)
        {
            fifties += number * 100 * 2;
        }

        public void AddPurples(int number)
        {
            fifties += number * 500 * 2;
        }

        Decimal Dollars
        {
            get
            {
                return Fifties / 2m;
            }
            set
            {
                fifties = (int)(value * 2);
            }
        }
    }

    public class Payout
    {
        int BetAmount;
        int PayoutAmount;

        public override String ToString()
        {
            return PayoutAmount + " to " + BetAmount;
        }

        public Payout(int bet, int payout)
        {
            BetAmount = bet;
            PayoutAmount = payout;
        }

        public Cheques Pay(Cheques bet)
        {
            return new Cheques(((bet.Fifties * PayoutAmount) / BetAmount) / 2m);
        }
    }

    public class BlackJack
    {
        const int Ace = 1;
        const int Face = 10;
        const int NumbersInGame = 10;
        const int SuitsInDeck = 4;
        const int AcesInDeck = SuitsInDeck;
        const int CardsInDeck = 52;
        const int FacesInSuit = 4;

        public enum Suits : short
        {
            Spades = 1,
            Hearts = 2,
            Diamonds = 3,
            Clubs = 4
        }

        public enum MaximumBetInDollars : short
        {
            FiveHundred = 500,
            OneThousand = 1000
        }

        public enum MinimumBetInDollars : byte
        {
            Five = 5,
            Ten = 10,
            TwentyFive = 25,
            OneHundred = 100
        }

        public enum NumberOfDecks : byte
        {
            One = 1,
            Two = 2,
            Six = 6
        }

        public enum Payout : int
        {
            ThreeToTwo = 3,
            SixToFive = 6
        }

        NumberOfDecks numberOfDecksInGame;
        MaximumBetInDollars maximumBetInDollarsInGame;
        MinimumBetInDollars minimumBetInDollarsInGame;

        public override String ToString()
        {
            if (NumberOfDecksInGame == NumberOfDecks.One)
            {
                return "Single Deck";
            }
            else if (NumberOfDecksInGame == NumberOfDecks.Two)
            {
                return "Double Deck";
            }
            if (NumberOfDecksInGame == NumberOfDecks.Six)
            {
                return "Six Deck";
            }

            return "Not Sure";
        }

        NumberOfDecks NumberOfDecksInGame
        {
            get
            {
                return numberOfDecksInGame;
            }
        }

        MaximumBetInDollars MaximumBetInDollarsInGame
        {
            get
            {
                return MaximumBetInDollarsInGame;
            }
        }

        MinimumBetInDollars MinimumBetInDollarsInGame
        {
            get
            {
                return minimumBetInDollarsInGame;
            }
        }

        private int[] numbersUsed;


        public Decimal RandomBet
        {
            get
            {
                int GetRandomNumberofFiftyCentPieces = Random.GetRandomNumber((int)MinimumBetInDollarsInGame * 2, (int)MaximumBetInDollarsInGame * 2);
                return GetRandomNumberofFiftyCentPieces / 0.5m;
            }
        }

        private int MaxAces
        {
            get
            {
                return (int)NumberOfDecksInGame * (int)Suits.Total;
            }
        }

        private int MaxFaces
        {
            get
            {
                return (int)this.NumberOfDecksInGame * (int)Suits.Total * FacesInSuit;
            }
        }

        private int CardsLeft
        {
            get
            {
                return ((int)NumberOfDecksInGame * CardsInDeck) - CardsUsed;
            }
        }

        private int CardsUsed
        {
            get
            {
                int Total = 0;
                foreach (int Used in numbersUsed)
                {
                    Total += Used;
                }

                return Total;
            }
        }

        public int DealCard()
        {
            int Card = 0;
            int Total = 0;
            int NextCard = Random.GetRandomNumber(1, CardsUsed);

            for (int Number = 1; Number <= NumbersInGame; Number++)
            {
                int Max = MaxAces;

                if (Number == Face)
                {
                    Max = MaxFaces;
                }

                int CardsLeftOfNumber = Max - numbersUsed[Number - 1];

                Total += CardsLeftOfNumber;

                if (NextCard < Total)
                {
                    numbersUsed[Number - 1] += 1;
                    Card = Number;
                    break;
                }
            }

            return Card;
        }

        public Payout PayoutForGame;

        public BlackJack(NumberOfDecks numberOfDecks, MinimumBetInDollars? minimumBetInDollars = null)
        {
            numberOfDecksInGame = numberOfDecks;

            if (numberOfDecks == NumberOfDecks.One)
            {
                PayoutForGame = Payout.SixToFive;
            }
            else
            {
                PayoutForGame = Payout.ThreeToTwo;
            }

            if (minimumBetInDollars == null)
            {
                if (numberOfDecksInGame == NumberOfDecks.One)
                {
                    minimumBetInDollarsInGame = MinimumBetInDollars.Five;
                }
                else if (numberOfDecksInGame == NumberOfDecks.Two)
                {
                    maximumBetInDollarsInGame = MaximumBetInDollars.OneThousand;
                }
                else if (numberOfDecksInGame == NumberOfDecks.Six)
                {
                    minimumBetInDollarsInGame = MinimumBetInDollars.Ten;
                }
            }
            else
            {
                minimumBetInDollarsInGame = minimumBetInDollars.Value;
            }

            if (minimumBetInDollarsInGame == MinimumBetInDollars.Five)
            {
                maximumBetInDollarsInGame = MaximumBetInDollars.FiveHundred;
            }
            else
            {
                maximumBetInDollarsInGame = MaximumBetInDollars.OneThousand;
            }
        }
    }

    public class Table
    {
        BlackJack GameAtTable;
        int Number;

        public Table(int number, BlackJack game)
        {
            Number = number;
            GameAtTable = game;
        }
    }

    public class Pit
    {
        public enum PitNumber : byte
        {
            One = 1,
            Two = 2,
            Three = 3
        }

        public Table[] Tables;

        public Pit(PitNumber Number)
        {
        }
    }

    public class Pits
    {
        private Pit[] pits;

        public Pits()
        {
            pits = new Pit[3];
            pits[(int)Pit.PitNumber.One] = new Pit(Pit.PitNumber.One);
            pits[(int)Pit.PitNumber.Two] = new Pit(Pit.PitNumber.Two);
            pits[(int)Pit.PitNumber.Three] = new Pit(Pit.PitNumber.Three);
        }

        public Pit this[Pit.PitNumber pitNumber]
        {
            get
            {
                return pits[(int)pitNumber];
            }
        }

    }
    public class Floor
    {
        Pits PitsOnFloor;

        public Floor()
        {
            PitsOnFloor = new Pits();

            PitsOnFloor[Pit.PitNumber.One].Tables = new Table[12];

            PitsOnFloor[Pit.PitNumber.One].Tables[0] = new Table(112, new Game(Game.NumberOfDecks.One));
            PitsOnFloor[Pit.PitNumber.One].Tables[1] = new Table(14, new Game(Game.NumberOfDecks.Six));
            PitsOnFloor[Pit.PitNumber.One].Tables[2] = new Table(17, new Game(Game.NumberOfDecks.Six));

        }
    }

    public Hand
    {
    }

    public WordsAtTheTable
    {
        Jokes
        Jokes
    }
}
