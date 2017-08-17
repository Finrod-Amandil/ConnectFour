using System;

namespace ConnectFour
{
    class Player
    {
        public ConsoleColor Color { get; private set; }
        public char Symbol { get; private set; }
        public string Name { get; private set; }

        public Player(string name, ConsoleColor color, char symbol)
        {
            Color = color;
            Symbol = symbol;
            Name = name;
        }
    }
}
