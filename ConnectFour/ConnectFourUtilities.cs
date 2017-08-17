using System;

namespace ConnectFour
{
    internal class ConnectFourUtilities
    {
        public static void DrawBoard(int colCount, int rowCount)
        {
            DrawColNums(colCount);

            for (int row = 0; row < rowCount; row++) //Iterate through rows
            {
                if (row == 0) //Upper border
                {
                    Console.Write("╔"); //Left lower corner
                    for (int col = 0; col < colCount; col++)
                    {
                        if (col != colCount - 1)
                        {
                            Console.Write("═══╦");
                        }
                        else
                        {
                            Console.WriteLine("═══╗"); //Right border and new line
                        }
                    }
                }

                Console.Write("║"); //Left border of board
                for (int col = 0; col < colCount; col++) //Iterate through columns
                {
                    Console.Write("   ");

                    if (col != colCount - 1)
                    {
                        Console.Write("│"); //Col divider
                    }
                    else
                    {
                        Console.WriteLine("║"); //Right border and new line
                    }
                }

                if (row != rowCount - 1)
                { //Horizontal divider
                    Console.Write("╠"); //Left border
                    for (int col = 0; col < colCount; col++)
                    {
                        if (col != colCount - 1)
                        {
                            Console.Write("───┼");
                        }
                        else
                        {
                            Console.WriteLine("───╣"); //Right border and new line
                        }
                    }
                }
                else //Lower border
                {
                    Console.Write("╚"); //Left lower corner
                    for (int col = 0; col < colCount; col++)
                    {
                        if (col != colCount - 1)
                        {
                            Console.Write("═══╩");
                        }
                        else
                        {
                            Console.WriteLine("═══╝"); //Right border and new line
                        }
                    }
                }
            }

            DrawColNums(colCount);
        }

        private static void DrawColNums(int colCount)
        {
            Console.Write(" "); //Padding for border left
            for (int col = 0; col < colCount; col++)
            {
                switch ((col + 1).ToString().Length)
                {
                    case 1:
                        Console.Write(" " + (col + 1) + "  ");
                        break;
                    case 2:
                        Console.Write((col + 1) + "  ");
                        break;
                    case 3:
                        Console.Write(col + 1);
                        break;
                    default:
                        Console.Write("###");
                        break;
                }
                
                if (col == colCount - 1) //Newline
                {
                    Console.WriteLine("");
                }
            }
        }

        public static void WriteAt(int left, int top, string text)
        {
            Console.SetCursorPosition(left, top);
            Console.Write(text);
        }

        public static void WriteAt(int left, int top, string text, bool clearLine)
        {
            if (clearLine)
            {
                Console.SetCursorPosition(left, top);
                Console.Write(new String(' ', Console.BufferWidth));
            }

            WriteAt(left, top, text);
        }

        public static void ClearLine(int top)
        {
            WriteAt(0, top, "", true);
        }

    }
}
