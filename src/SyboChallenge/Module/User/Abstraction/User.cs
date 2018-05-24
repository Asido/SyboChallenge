using System;

namespace SyboChallenge.Module.User.Abstraction
{
    public class User
    {
        public Guid Key { get; set; }
        public string Name { get; set; }
    }

    public class Friend
    {
        public Guid Key { get; set; }
        public string Name { get; set; }
        public int Highscore { get; set; }
    }

    public class State
    {
        public int GamesPlayed { get; set; }
        public int Score { get; set; }
    }
}
