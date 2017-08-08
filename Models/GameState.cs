namespace DojoDachi
{
    public class GameState
    {
        public int Fullness { get; set; }

        public int Happiness { get; set; }

        public int Meals { get; set; }

        public int Energy { get; set; }

        public int Level { get; set;}

        public GameState()
        {
            Fullness = 20;
            Happiness = 20;
            Meals = 3;
            Energy = 50;
            Level = 1;
        }
    }
}