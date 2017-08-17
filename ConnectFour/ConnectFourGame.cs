using System;
using System.Collections.Generic;
using System.Linq;

namespace ConnectFour
{
    internal class ConnectFourGame
    {
        private static readonly int MaxPlayers = 6;
        private static readonly int MIN_PLAYERS = 2;
        private static readonly int MAX_NAME_LENGTH = 50;
        private static readonly ConsoleColor[] COLORS = new ConsoleColor[]
        {
            ConsoleColor.Red,
            ConsoleColor.Yellow,
            ConsoleColor.Cyan,
            ConsoleColor.Green,
            ConsoleColor.Magenta,
            ConsoleColor.Blue
        };
        private static readonly char[] SYMBOLS = new [] { 'O', 'X', '$', '#', '@', '%' };
        private static readonly int COL_COUNT = 7;
        private static readonly int ROW_COUNT = 6;
        private static readonly int WIN_LENGTH = 4;
        private static readonly int BOARD_START_X = 2;
        private static readonly int BOARD_START_Y = MaxPlayers + 3;
        private static readonly int BOARD_STEP_HORZ = 4;
        private static readonly int BOARD_STEP_VERT = 2;
        private static readonly int MSG_START_Y = BOARD_START_Y + (ROW_COUNT * BOARD_STEP_VERT) + 2;

        private int[,] _board;
        private List<Player> _players;
        private int _currentPlayer;

        public ConnectFourGame()
        {
            //Initialise players
            Console.Clear();
            _players = new List<Player>();
            string input = "";

            //Repeat for all players
            do
            {
                ConnectFourUtilities.ClearLine(_players.Count + 0);
                ConnectFourUtilities.WriteAt(0, _players.Count + 1, "Name für Spieler " + (_players.Count + 1) + " eingeben oder Enter drücken wenn alle Spieler eingegeben wurden: ", true);
                input = Console.ReadLine(); //Read player name, if only Enter without input is given, no more players are added.

                if (input.Length > MAX_NAME_LENGTH) //Truncate long names
                {
                    input = input.Substring(0, MAX_NAME_LENGTH);

                    //Clean up Terminal
                    for (int i = 0; i < 100; i++)
                    {
                        ConnectFourUtilities.ClearLine(_players.Count + i);
                    }
                    Console.CursorTop = 0;
                }

                //If user tries to abort player creation when not enough players have been added yet, write error message.
                if (input == "" && _players.Count < 2)
                {
                    ConnectFourUtilities.WriteAt(0, _players.Count + 2, "Bitte mindestens " +  MIN_PLAYERS + " Spieler eingeben!", true);
                    continue;
                }
                else if (input != "")
                {
                    //Create new player object and add it to list of players.
                    _players.Add(new Player(input, COLORS[_players.Count], SYMBOLS[_players.Count]));

                    //Add playername to list of players in console.
                    ConnectFourUtilities.WriteAt(0, _players.Count - 1, "Spieler " + _players.Count + ": ", true);
                    Console.ForegroundColor = _players.Last().Color;
                    ConnectFourUtilities.WriteAt(12, _players.Count - 1, _players.Last().Symbol + " " + _players.Last().Name);
                    Console.ResetColor();
                }

            } while (_players.Count < 2 || (input != "" && _players.Count < MaxPlayers)); //Ask for more players as long as less than MIN_PLAYERS have been added and while input was not only an ENTER.

            //Clear input request
            ConnectFourUtilities.ClearLine(_players.Count);
            ConnectFourUtilities.ClearLine(_players.Count + 1);

            //Initialise board
            //0: empty field
            //1: stone of player 1
            //2: stone of player 2
            //etc.
            _board = new int[COL_COUNT,ROW_COUNT];

            //Determine who can begin
            _currentPlayer = new Random().Next(_players.Count);

            //Draw empty board
            Console.SetCursorPosition(0, BOARD_START_Y - 2);
            ConnectFourUtilities.DrawBoard(COL_COUNT, ROW_COUNT);

            //Run game
            while(true)
            {
                //Ask user to enter a column and add stone to board
                PlayerPlacesDisc();

                //Check whether player has won or board is full
                if (HasPlayerWon())
                {
                    ConnectFourUtilities.WriteAt(0, MSG_START_Y, _players[_currentPlayer].Name + " hat gewonnen!", true);
                    ConnectFourUtilities.WriteAt(0, MSG_START_Y + 2, "ENTER drücken um zu beenden...", true);
                    return;
                }
                else if (IsBoardFull())
                {
                    ConnectFourUtilities.WriteAt(0, MSG_START_Y, "Unentschieden!", true);
                    ConnectFourUtilities.WriteAt(0, MSG_START_Y + 2, "ENTER drücken um zu beenden...", true);
                    return;
                }

                //Continue to next player
                _currentPlayer = (_currentPlayer + 1) % _players.Count;
            }
        }

        private void PlayerPlacesDisc()
        {
            Console.ForegroundColor = _players[_currentPlayer].Color;
            ConnectFourUtilities.WriteAt(0, MSG_START_Y, _players[_currentPlayer].Name + " ist an der Reihe!", true);
            Console.ResetColor();

            int selectedColumn = -1;

            while(true)
            {
                selectedColumn = ReadColumn() - 1;
                ConnectFourUtilities.ClearLine(MSG_START_Y + 3); //Clear full column warning.

                //Check whether topmost (index 0) field of selected column is empty
                if (_board[selectedColumn, 0] == 0)
                {
                    break;
                }
                else
                {
                    ConnectFourUtilities.WriteAt(0, MSG_START_Y + 3, "Diese Spalte ist voll, wähle eine andere!", true);
                }

            }

            //Find lowest empty field of selected column and place stone there.
            for (int row = 0; row < ROW_COUNT; row++)
            {
                if (row == ROW_COUNT - 1 || _board[selectedColumn, row + 1] != 0)
                {
                    //Place stone
                    _board[selectedColumn, row] = _currentPlayer + 1;
                    //Add stone to visual board
                    DrawStone(selectedColumn, row);
                    break;
                }
            }
        }

        private int ReadColumn()
        {
            int col = 0;
            while(true)
            {
                ConnectFourUtilities.WriteAt(0, MSG_START_Y + 2, $"Bitte Spaltennummer (1 - {COL_COUNT}) eingeben: ", true);
                string input = Console.ReadLine();
                ConnectFourUtilities.ClearLine(MSG_START_Y + 3); //Clear error message

                if (int.TryParse(input, out col) && col >= 1 && col <= COL_COUNT)
                {
                    break;
                }
                else
                {
                    ConnectFourUtilities.WriteAt(0, MSG_START_Y + 3, string.Format("Ungültige Spaltennummer \"{0}\", bitte versuche es erneut.", input), true);
                }
            }

            return col;
        }

        private bool IsBoardFull()
        {
            foreach (int val in _board)
            {
                if (val == 0)
                {
                    return false;
                }
            }
            return true;
        }

        private void DrawStone(int col, int row)
        {
            Console.ForegroundColor = _players[_currentPlayer].Color;
            ConnectFourUtilities.WriteAt(
                BOARD_START_X + col * BOARD_STEP_HORZ, 
                BOARD_START_Y + row * BOARD_STEP_VERT,
                _players[_currentPlayer].Symbol.ToString());
            Console.ResetColor();
        }

        private bool HasPlayerWon()
        {
            //Iterate through all fields
            for (int col = 0; col < COL_COUNT; col++) //Iterate through columns
            {
                for (int row = 0; row < ROW_COUNT; row++) //Iterate through rows
                {
                    bool wonVertical = true;
                    bool wonHorizontal = true;
                    bool wonDiagTLBR = true;
                    bool wonDiagBLTR = true;

                    for (int i = 0; i < WIN_LENGTH; i++)
                    {
                        //check column starting at (col, row)
                        if (row + i >= ROW_COUNT ||                     //If field does not exist
                            _board[col, row + i] != _currentPlayer + 1) //is empty or contains a foreign stone
                        {
                            wonVertical = false; //Not a winning combination
                        }

                        //check row starting at (col, row)
                        if (col + i >= COL_COUNT ||                     //If field does not exist or 
                            _board[col + i, row] != _currentPlayer + 1) //is empty or contains a foreign stone
                        {
                            wonHorizontal = false; //Not a winning combination
                        }

                        //check diagonal top left to bottom right starting at (col, row)
                        if (col + i >= COL_COUNT || row + i >= ROW_COUNT ||  //If field does not exist or
                            _board[col + i, row + i] != _currentPlayer + 1) //is empty or contains a foreign stone
                        {
                            wonDiagTLBR = false; //Not a winning combination
                        }

                        //check diagonal bottom left to top right starting at (col, row)
                        if (col + i >= COL_COUNT || row - i < 0 ||          //If field does not exist or
                            _board[col + i, row - i] != _currentPlayer + 1) //is empty or contains a foreign stone
                        {
                            wonDiagBLTR = false; //Not a winning combination
                        }
                    }

                    if (wonVertical || wonHorizontal || wonDiagTLBR || wonDiagBLTR) //If a winning combination could be found, exit function
                    {
                        if (wonVertical) MarkWinner(col, row, 0, 1);
                        if (wonHorizontal) MarkWinner(col, row, 1, 0);
                        if (wonDiagTLBR) MarkWinner(col, row, 1, 1); 
                        if (wonDiagBLTR) MarkWinner(col, row, 1, -1);

                        return true; //Player who made last move has won
                    }
                }
            }

            return false;
        }

        private void MarkWinner(int col, int row, int dirX, int dirY)
        {
            for (int i = 0; i < WIN_LENGTH; i++)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                DrawStone(col + (i * dirX), row + (i * dirY));
            }
            Console.ResetColor();
        }
    }
}
