using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public class KeyGenerator
{
    public static byte[] GenerateKey(int length)
    {
        using (var randomNumberGenerator = RandomNumberGenerator.Create())
        {
            byte[] key = new byte[length / 8];
            randomNumberGenerator.GetBytes(key);
            return key;
        }
    }
}

public class HmacCalculator
{
    public static string CalculateHmac(string message, byte[] key)
    {
        using (var hmac = new HMACSHA256(key))
        {
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}

public class MoveTableGenerator
{
    public static void DisplayMoveTable(string[] moves)
    {
        int length = moves.Length;
        Console.Write($"| {"v PC\\User >",-8}");
        for (int i = 0; i < length; i++)
        {
            Console.Write($"| {moves[i],-8}");
        }
        Console.WriteLine("|");

        Console.WriteLine("+" + new string('-', (length * 11) + 16) + "+");

        for (int i = 0; i < length; i++)
        {
            Console.Write($"| {moves[i],-11}");

            for (int j = 0; j < length; j++)
            {
                Console.Write($"| {DetermineWinner(i, j, length),-8}");
            }
            Console.WriteLine("|");

            Console.WriteLine("+" + new string('-', (length * 11) + 16) + "+");
        }
    }

    private static string DetermineWinner(int userMove, int computerMove, int length)
    {
        int half = length / 2;

        if (userMove == computerMove)
        {
            return "Draw";
        }
        else if ((userMove + half) % length == computerMove)
        {
            return "Win";
        }
        else
        {
            return "Lose";
        }
    }
}

public class RockPaperScissorsGame
{
    private readonly string[] moves;
    private readonly byte[] key;

    public RockPaperScissorsGame(string[] moves, byte[] key)
    {
        this.moves = moves;
        this.key = key;
    }

    public void PlayGame()
    {
        int computerMove = GetRandomMove();

        string hmac = HmacCalculator.CalculateHmac(moves[computerMove], key);

        Console.WriteLine($"HMAC: {hmac}");

        MoveTableGenerator.DisplayMoveTable(moves);

        int userMove = GetUserMove();

        Console.WriteLine($"Computer's move: {moves[computerMove]}");

        DisplayResult(userMove, computerMove);

        Console.WriteLine($"Original Key: {BitConverter.ToString(key).Replace("-", "").ToLower()}");
    }

    private int GetRandomMove()
    {
        Random random = new Random();
        return random.Next(moves.Length);
    }

    private int GetUserMove()
    {
        int userMove;

        do
        {
            Console.Write($"Enter your move (1 - {moves.Length}, 0 - Exit): ");
        } while (!int.TryParse(Console.ReadLine(), out userMove) || userMove < 0 || userMove > moves.Length);

        return userMove - 1;
    }

    private void DisplayResult(int userMove, int computerMove)
    {
        int length = moves.Length;
        int half = length / 2;

        Console.WriteLine($"Your move: {moves[userMove]}");

        if (userMove == computerMove)
        {
            Console.WriteLine("Result: Draw");
        }
        else if ((userMove + half) % length == computerMove)
        {
            Console.WriteLine("Result: You win!");
        }
        else
        {
            Console.WriteLine("Result: Computer wins!");
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length < 3 || args.Length % 2 == 0 || args.Distinct().Count() != args.Length)
        {
            Console.WriteLine("Error: Invalid input. Please provide an odd number (>=3) of non-repeating strings.");
            Console.WriteLine("Example: STONE PAPER SCISSORS or STONE SPOCK PAPER LIZARD SCISSORS");
            return;
        }

        byte[] key = KeyGenerator.GenerateKey(256);
        RockPaperScissorsGame game = new RockPaperScissorsGame(args, key);
        game.PlayGame();
    }
}

/*
 
 1) a link to a video demonstrating launch with 3 and 7 parameters, launch with incorrect parameters (repeated or even number, one or no), help table generation (on 5 parameters), choice of the user move, output of results;
 2) source code link to github.

*/