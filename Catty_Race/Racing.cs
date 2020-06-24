using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catty_Race
{
    public enum HoundEnum
    {
        Pink,
        Teal,
        Blue
    }

    public class Racing
    {
        public Greyhound PinkHound = new Greyhound() { name = "Pink", position = 0 };
        public Greyhound TealHound = new Greyhound() { name = "Teal", position = 0 };
        public Greyhound BlueHound = new Greyhound() { name = "Blue", position = 0 };
        public List<Greyhound> Hounds;
        public Punter Joe;
        public Punter Bob;
        public Punter Alice;
        public List<Punter> Punters;
        private Random random = new Random();

        public Racing()
        {
            Hounds = new List<Greyhound>() { PinkHound, TealHound, BlueHound };
            Joe = PunterFactory.ManufacturePunter(PunterEnum.Joe);
            Bob = PunterFactory.ManufacturePunter(PunterEnum.Bob);
            Alice = PunterFactory.ManufacturePunter(PunterEnum.Alice);
            Punters = new List<Punter>() { Joe, Bob, Alice };
        }

        public void Bet(HoundEnum hound, Punter punter, int amount)
        {
            punter.betAmount = amount;
            switch (hound)
            {
                case HoundEnum.Pink:
                    punter.betHound = PinkHound;
                    break;
                case HoundEnum.Teal:
                    punter.betHound = TealHound;
                    break;
                case HoundEnum.Blue:
                    punter.betHound = BlueHound;
                    break;

            }
        }

        public async Task<Greyhound> Race()
        {
            bool noWinner = true;
            int pink = 0;
            int teal = 0;
            int blue = 0;

            int counter = 0;

            while (noWinner)
            {
                counter += 1;
                await Task.Delay(16);

                if (counter % 10 == 0)
                {
                    pink = (int)(6 * (random.NextDouble() * 2.0 - 1.0)) + 6;
                    teal = (int)(3 * (random.NextDouble() * 2.0 - 1.0)) + 6;
                    blue = (int)(1 * (random.NextDouble() * 2.0 - 1.0)) + 6;
                }

                PinkHound.position += pink;
                TealHound.position += teal;
                BlueHound.position += blue;

                noWinner = PinkHound.position < 1000 && TealHound.position < 1000 && BlueHound.position < 1000;
            }

            Greyhound winner = new Greyhound() { name = "dummy", position = -99 };

            foreach (Greyhound hound in Hounds)
            {
                if (hound.position > winner.position) winner = hound;
            }

            foreach (Punter punter in Punters)
            {
                if (winner == punter.betHound)
                {
                    punter.Cash += punter.betAmount;
                }
                else
                {
                    punter.Cash -= punter.betAmount;
                }

                punter.betAmount = 0;
            }
            return winner;
        }

        public void Reset()
        {
            PinkHound.position = 0;
            BlueHound.position = 0;
            TealHound.position = 0;
        }
    }
}
