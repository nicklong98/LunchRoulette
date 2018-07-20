namespace LunchRoulette.Exceptions
{
    public class LunchRouletteException : System.Exception {}

    public class LunchRouletteNotFoundException : LunchRouletteException { }

    namespace LunchSpotExceptions
    {
        public class LunchSpotException : LunchRouletteException { }
        public class LunchSpotNotFoundException : LunchRouletteNotFoundException { }
    }

    namespace CuisineExceptions
    {
        public class CuisineException : LunchRouletteException { }
        public class CuisineNotFoundException : LunchRouletteNotFoundException { }
    }
}
