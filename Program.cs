using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NumericalTicTacToe
{
    class TicTacToeProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Numerical Tic-Tac-Toe!");
            Console.WriteLine("1. New Game");
            Console.WriteLine("2. Load Game");
            string? choice = Console.ReadLine()?.Trim();

            if (choice == "1")
            {
                Console.Write("Enter board size (>= 3): ");
                string? sizeInput = Console.ReadLine();
                int boardSize;
                while (sizeInput == null || !int.TryParse(sizeInput, out boardSize) || boardSize < 3)
                {
                    Console.Write("Invalid size. Enter a number (>= 3): ");
                    sizeInput = Console.ReadLine();
                }

                Console.WriteLine("Select mode: 1. Human vs Human, 2. Human vs Computer");
                string? modeChoice = Console.ReadLine();
                PlayMode mode = (modeChoice != null && modeChoice.Trim() == "2") ? PlayMode.HumanVsComputer : PlayMode.HumanVsHuman;

                Game game = new Game(mode, boardSize, true);
                game.Start();
            }
            else if (choice == "2")
            {
                Console.Write("Enter filename to load: ");
                string? filename = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(filename))
                {
                    Console.WriteLine("Invalid filename.");
                    return;
                }
                Game loadedGame = new Game(PlayMode.HumanVsHuman, 3, true);
                loadedGame.LoadGame(filename);
                loadedGame.Start();
            }
            else
            {
                Console.WriteLine("Invalid choice.");
                return;
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }

    // Board manages the game board， check win and check tie
    public class Board
    {   public int Size { get; private set; }
        public int[,] Cells { get; private set; }
        public int WinSum { get; private set; }

        public Board(int size)
        {   Size = size;
            Cells = new int[size, size];
            WinSum = size * (size * size + 1) / 2;
        }

        public void Display()
        {   Console.WriteLine("Current game board:");
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    string value = Cells[i, j] == 0 ? " " : Cells[i, j].ToString();
                    Console.Write($" {value,-2} ");
                    if (j < Size - 1)
                        Console.Write("|");
                }
                Console.WriteLine();
                if (i < Size - 1)
                    Console.WriteLine(new string('-', Size * 5 - 1));
            }
        }

        public bool IsEmpty(int row, int col)
        {   return Cells[row, col] == 0; }

        public bool PlaceMove(int row, int col, int number)
        {   if (row < 0 || row >= Size || col < 0 || col >= Size)
                return false;
            
            if (!IsEmpty(row, col))
                return false;
            Cells[row, col] = number;
            return true;
        }

        public void RemoveMove(int row, int col)
        {   Cells[row, col] = 0;}

        public bool CheckWin()
        {   
            for (int i = 0; i < Size; i++)
            {
                int[] rowLine = GetRow(i);
                if (IsLineWinning(rowLine))
                    return true;
            }

            for (int j = 0; j < Size; j++)
            {
                int[] colLine = GetColumn(j);
                if (IsLineWinning(colLine))
                    return true;
            }

            int[] diag1 = GetDiagonal1();
            if (IsLineWinning(diag1))
                return true;

            int[] diag2 = GetDiagonal2();
            if (IsLineWinning(diag2))
                return true;
            return false;
        }

        public bool CheckTie()
        {
            if (CheckWin())
                return false;
            // Check every cell，then if any is empty, not a tie.
            for (int i = 0; i < Size; i++)
            {   for (int j = 0; j < Size; j++)
                {   if (Cells[i, j] == 0)
                        return false;
                }
            }
            return true;
        }

        private int[] GetRow(int row)
        {   int[] line = new int[Size];
            for (int j = 0; j < Size; j++)
                line[j] = Cells[row, j];
            return line;
        }

        private int[] GetColumn(int col)
        {   int[] line = new int[Size];
            for (int i = 0; i < Size; i++)
                line[i] = Cells[i, col];
            return line;
        }

        private int[] GetDiagonal1()
        {   int[] line = new int[Size];
            for (int i = 0; i < Size; i++)
                line[i] = Cells[i, i];
            return line;
        }

        private int[] GetDiagonal2()
        {   int[] line = new int[Size];
            for (int i = 0; i < Size; i++)
                line[i] = Cells[i, Size - 1 - i];
            return line;
        }

        private bool IsLineWinning(int[] line)
        {   for (int i = 0; i < line.Length; i++)
            {   if (line[i] == 0)
                    return false;
            }

            int sum = 0;
            for (int i = 0; i < line.Length; i++)
            {   sum += line[i];}
            return sum == WinSum;
        }
    }

    // Abstract Player class hold player name and remaining numbers.
    public abstract class Player
    {   public string Name { get; set; }
        public List<int> RemainingNumbers { get; protected set; }

        public Player(string name, bool isOdd, int maxNumber)
        {   Name = name;
            RemainingNumbers = new List<int>();
            int start = isOdd ? 1 : 2;
            for (int i = start; i <= maxNumber; i += 2)
                RemainingNumbers.Add(i);
        }

        public abstract void MakeMove(Board board);
    }

    public enum PlayMode
    {   HumanVsHuman, HumanVsComputer}

    // Human Player: read input from user
    public class HumanPlayer : Player
    {
        public HumanPlayer(string name, bool isOdd, int maxNumber) : base(name, isOdd, maxNumber){}}

        public override void MakeMove(Board board)
        {
            while (true)
            {   Console.WriteLine($"{Name}, your remaining numbers: {string.Join(", ", RemainingNumbers)}");
                Console.Write($"Enter row (1 to {board.Size}): ");
                string? rowInput = Console.ReadLine();
                if (rowInput == null || !int.TryParse(rowInput, out int row))
                {
                    Console.WriteLine("Invalid row input.");
                    continue;
                }
                row -= 1; // Convert to 0-index

                Console.Write($"Enter column (1 to {board.Size}): ");
                string? colInput = Console.ReadLine();
                if (colInput == null || !int.TryParse(colInput, out int col))
                {   Console.WriteLine("Invalid column input.");
                    continue;
                }
                col -= 1; 

                if (row < 0 || row >= board.Size || col < 0 || col >= board.Size)
                {   Console.WriteLine($"Please enter values between 1 and {board.Size}.");
                    continue;
                }

                Console.Write("Enter number to place: ");
                string? numberInput = Console.ReadLine();
                if (numberInput == null || !int.TryParse(numberInput, out int number))
                {   Console.WriteLine("Invalid number input.");
                    continue;
                }

                if (!RemainingNumbers.Contains(number))
                {   Console.WriteLine("This number is used.");
                    continue;
                }
                if (!board.IsEmpty(row, col))
                {   Console.WriteLine("This cell is used.");
                    continue;
                }
                if (board.PlaceMove(row, col, number))
                {   RemainingNumbers.Remove(number);
                    break;
                }
            }
        }
    }

    // Computer automatically makes a move
    public class ComputerPlayer : Player
    {
        private Random rand = new Random();

        public ComputerPlayer(string name, bool isOdd, int maxNumber): base(name, isOdd, maxNumber){}

        public override void MakeMove(Board board)
        {   Console.WriteLine($"{Name} is thinking...");
            // Try each empty cell with available number to see if it wins.
            for (int i = 0; i < board.Size; i++)
            {
                for (int j = 0; j < board.Size; j++)
                {   if (!board.IsEmpty(i, j))
                        continue;
                    foreach (int number in RemainingNumbers.ToArray())
                    {   board.PlaceMove(i, j, number);
                        if (board.CheckWin())
                        {   Console.WriteLine($"{Name} plays {number} at ({i + 1}, {j + 1}).");
                            RemainingNumbers.Remove(number);
                            return;
                        }
                        board.RemoveMove(i, j);
                    }
                }
            }
            // If no winning move, pick random empty cell & number
            List<(int, int)> emptyCells = new List<(int, int)>();
            for (int i = 0; i < board.Size; i++)
            {   for (int j = 0; j < board.Size; j++)
                {   if (board.IsEmpty(i, j))
                        emptyCells.Add((i, j));
                }
            }
            if (emptyCells.Count > 0 && RemainingNumbers.Count > 0)
            {   var (row, col) = emptyCells[rand.Next(emptyCells.Count)];
                int number = RemainingNumbers[rand.Next(RemainingNumbers.Count)];
                board.PlaceMove(row, col, number);
                Console.WriteLine($"{Name} plays {number} at ({row + 1}, {col + 1}).");
                RemainingNumbers.Remove(number);
            }
        }
    }

    // Game class is to control game flow, saving, and loading.
    public class Game
    {
        public Board Board { get; private set; }
        public Player FirstPlayer { get; private set; }
        public Player SecondPlayer { get; private set; }
        public PlayMode Mode { get; private set; }
        public int CurrentTurn { get; private set; } 

        public Game(PlayMode mode, int boardSize, bool humanIsFirstPlayer)
        {   Mode = mode;
            Board = new Board(boardSize);
            int maxNumber = boardSize * boardSize;
            if (humanIsFirstPlayer)
            {   FirstPlayer = new HumanPlayer("Player 1", true, maxNumber);
                SecondPlayer = (mode == PlayMode.HumanVsComputer)
                    ? (Player)new ComputerPlayer("Computer", false, maxNumber)
                    : new HumanPlayer("Player 2", false, maxNumber);
            }
            else
            {   FirstPlayer = new ComputerPlayer("Computer", true, maxNumber);
                SecondPlayer = new HumanPlayer("Player 2", false, maxNumber);
            }
            CurrentTurn = 1;
        }

        public void Start()
        {   while (true)
            {   Board.Display();
                Console.WriteLine($"Win sum: {Board.WinSum}");
                Console.WriteLine($"Current turn: {(CurrentTurn == 1 ? FirstPlayer.Name : SecondPlayer.Name)}");

                // If current player is a computer, make an auto move.
                if ((CurrentTurn == 1 && FirstPlayer is ComputerPlayer) ||
                    (CurrentTurn == 2 && SecondPlayer is ComputerPlayer))
                {   if (CurrentTurn == 1)
                        FirstPlayer.MakeMove(Board);
                    else
                        SecondPlayer.MakeMove(Board);
                }
                else
                {   Console.WriteLine("Enter 1 for move, 2 for save, 3 for help:");
                    string? command = Console.ReadLine()?.Trim();
                    if (command == "2")
                    {   Console.Write("Enter filename to save: ");
                        string? filename = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(filename))
                            continue;
                        SaveGame(filename);
                        Console.WriteLine("Game saved. Press any key.");
                        Console.ReadKey();
                        continue;
                    }
                    else if (command == "3")
                    {   ShowHelp();
                        continue;
                    }
                    else if (command == "1")
                    {   if (CurrentTurn == 1)
                            FirstPlayer.MakeMove(Board);
                        else
                            SecondPlayer.MakeMove(Board);
                    }
                    else
                    {continue;}
                }

                if (Board.CheckWin())
                {   Board.Display();
                    Console.WriteLine($"{(CurrentTurn == 1 ? FirstPlayer.Name : SecondPlayer.Name)} wins!");
                    break;
                }
                if (Board.CheckTie())
                {   Board.Display();
                    Console.WriteLine("This game is a tie.");
                    break;
                }
                CurrentTurn = (CurrentTurn == 1) ? 2 : 1;
            }
        }

        private void ShowHelp()
        {   Console.WriteLine("Commands:");
            Console.WriteLine("1. Move: Enter row & column (1-indexed) and number to place.");
            Console.WriteLine("2. Save: Save the current game.");
            Console.WriteLine("3. Help: Show this menu.");
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }

        public void SaveGame(string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {   writer.WriteLine(Board.Size);
                writer.WriteLine(CurrentTurn);
                for (int i = 0; i < Board.Size; i++)
                {   string[] rowValues = new string[Board.Size];
                    for (int j = 0; j < Board.Size; j++)
                        rowValues[j] = Board.Cells[i, j].ToString();
                    writer.WriteLine(string.Join(",", rowValues));
                }
                writer.WriteLine(string.Join(",", FirstPlayer.RemainingNumbers));
                writer.WriteLine(string.Join(",", SecondPlayer.RemainingNumbers));
                writer.WriteLine(Mode.ToString());
            }
        }

        public void LoadGame(string filename)
        {   if (!File.Exists(filename))
            {   Console.WriteLine("File not found.");
                return;
            }
            using (StreamReader reader = new StreamReader(filename))
            {   int boardSize = int.Parse(reader.ReadLine() ?? "3");
                Board = new Board(boardSize);
                CurrentTurn = int.Parse(reader.ReadLine() ?? "1");
                for (int i = 0; i < boardSize; i++)
                {   string? line = reader.ReadLine();
                    if (line == null) break;
                    string[] nums = line.Split(',');
                    for (int j = 0; j < boardSize; j++)
                        Board.Cells[i, j] = int.Parse(nums[j]);
                }
                int maxNumber = boardSize * boardSize;
                FirstPlayer = new HumanPlayer("Player 1", true, maxNumber);
                SecondPlayer = new HumanPlayer("Player 2", false, maxNumber);
                string? avail1 = reader.ReadLine();
                if (!string.IsNullOrWhiteSpace(avail1))
                {   string[] nums = avail1.Split(',');
                    FirstPlayer.RemainingNumbers.Clear();
                    foreach (string num in nums)
                        FirstPlayer.RemainingNumbers.Add(int.Parse(num));
                }
                string? avail2 = reader.ReadLine();
                if (!string.IsNullOrWhiteSpace(avail2))
                {   string[] nums = avail2.Split(',');
                    SecondPlayer.RemainingNumbers.Clear();
                    foreach (string num in nums)
                        SecondPlayer.RemainingNumbers.Add(int.Parse(num));
                }
                string? modeStr = reader.ReadLine();
                if (!string.IsNullOrWhiteSpace(modeStr))
                    Mode = (PlayMode)Enum.Parse(typeof(PlayMode), modeStr);
            }
            Console.WriteLine("Game loaded successfully.");
        }
    }
}