namespace Afrimine.Shared.Helpers
{
    public class GenerateRandomNumbers
    {
        private static readonly Random _random = new();

        public static int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }
    }
}
