namespace Catty_Race
{
    public enum PunterEnum
    {
        Joe,
        Bob,
        Alice
    }
    public abstract class Punter
    {
        public int Cash = 50;
        public int betAmount;
        public Greyhound betHound;
        public string name;
    }

    public static class PunterFactory
    {
        public static Punter ManufacturePunter(PunterEnum name)
        {
            switch (name)
            {
                case PunterEnum.Joe:
                    return new Joe();
                case PunterEnum.Bob:
                    return new Bob();
                case PunterEnum.Alice:
                    return new Alice();
            }

            return null;
        }
    }

    class Joe : Punter
    {
        public Joe()
        {
            name = "Joe Sieve";
        }
    }

    class Bob : Punter
    {
        public Bob()
        {
            name = "Bob Bobson";
        }
    }

    class Alice : Punter
    {
        public Alice()
        {
            name = "Alice Inoderlun";
        }
    }
}
