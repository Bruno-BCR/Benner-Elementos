using System;
using System.Collections.Generic;

public class Network
{
    private readonly int _size; // Total number of elements
    private readonly Dictionary<int, HashSet<int>> _connections; // Stores the connections

    public Network(int size)
    {
        if (size <= 0)
            throw new ArgumentException("Size must be a positive integer."); // Validates positive size (discarts negatives)

        _size = size;
        _connections = new Dictionary<int, HashSet<int>>();

        // Initializes each element
        for (int i = 1; i <= size; i++)
        {
            _connections[i] = new HashSet<int>();
        }
    }

    // Connects two elements
    public void Connect(int a, int b)
    {
        ValidateElement(a);
        ValidateElement(b);

        if (a == b)
            throw new ArgumentException("Cannot connect an element to itself."); // Prevent self-connection

        _connections[a].Add(b); // Add b to a's connection list (lista do a = a's list)
        _connections[b].Add(a); // Add a to b's connection list (lista do b = b's list)
    }

    // Disconnects two elements
    public void Disconnect(int a, int b)
    {
        ValidateElement(a);
        ValidateElement(b);

        if (!_connections[a].Contains(b))
            throw new InvalidOperationException("Elements are not connected."); // Only disconnect if they are connected 
        _connections[a].Remove(b);
        _connections[b].Remove(a);
    }

    // Checks whether two elements are connected directly or indirectly
    public bool Query(int a, int b)
    {
        ValidateElement(a);
        ValidateElement(b);
        return BFS(a, b) != -1; // Use BFS (Breadth-first search)
    }

    // Returns the level of connection between two elements (0 if not connected)
    public int LevelConnection(int a, int b)
    {
        ValidateElement(a);
        ValidateElement(b);

        if (a == b)
            return 0;

        int level = BFS(a, b); // Perform BFS to find the shortest path
        return level == -1 ? 0 : level;
    }

    // Breadth-first search to find the shortest connection path between elements
    private int BFS(int start, int target)
    {
        if (start == target)
            return 0;

        var visited = new HashSet<int>();
        var queue = new Queue<(int node, int level)>();
        queue.Enqueue((start, 0));
        visited.Add(start);

        while (queue.Count > 0)
        {
            var (current, level) = queue.Dequeue();

            foreach (var neighbor in _connections[current])
            {
                if (neighbor == target)
                    return level + 1;

                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue((neighbor, level + 1));
                }
            }
        }

        return -1; // Not found
    }

    // Validates if an element is within the valid range
    private void ValidateElement(int element)
    {
        if (element < 1 || element > _size)
            throw new ArgumentOutOfRangeException($"Element {element} is out of valid range (1 to {_size}).");

        if (!_connections.ContainsKey(element))
            throw new ArgumentException($"Element {element} is not initialized.");
    }
}

// Main program: user interaction
class Program
{
    static void Main()
    {
        Console.Write("How many elements? (press Enter for default 6): ");
        string? input = Console.ReadLine();
        int total;

        // Defaults to 6 if user provides no input
        if (string.IsNullOrWhiteSpace(input))
        {
            total = 6;
            Console.WriteLine("Defaulting to 6 elements.");
        }
        else
        {
            // Keep asking until a valid positive number is given
            while (!int.TryParse(input, out total) || total <= 0)
            {
                Console.Write("Type a positive integer: ");
                input = Console.ReadLine();
            }
        }

        Network network = new Network(total); // Initialize the network

        Console.WriteLine($"\nAvailable integers: 1 - {total}");
        Console.WriteLine("\nType the integers you wish to connect (example: 1 2). Type 'end' to finish this step.");
        int connectionsMade = 0;

        // Connection step
        while (true)
        {
            Console.Write("Connect: ");
            var entrada = Console.ReadLine();

            if (entrada.Trim().ToLower() == "end")
                break;

            var parts = entrada.Split();

            if (parts.Length != 2 || !int.TryParse(parts[0], out int a) || !int.TryParse(parts[1], out int b))
            {
                Console.WriteLine("Invalid entry. Try: <int> <int>");
                continue;
            }

            try
            {
                network.Connect(a, b);
                connectionsMade++;
                Console.WriteLine($"Successfully connected: {a} and {b}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        Console.WriteLine($"\nTotal connections made: {connectionsMade}");

        // Disconnection step
        Console.WriteLine("\nDisconnect connections (example: 1 2). Type 'end' to finish.");
        while (true)
        {
            Console.Write("Disconnect: ");
            var entrada = Console.ReadLine();

            if (entrada.Trim().ToLower() == "end")
                break;

            var parts = entrada.Split();

            if (parts.Length != 2 || !int.TryParse(parts[0], out int a) || !int.TryParse(parts[1], out int b))
            {
                Console.WriteLine("Invalid entry. Try: <int> <int>");
                continue;
            }

            try
            {
                network.Disconnect(a, b);
                Console.WriteLine($"Disconnected: {a} and {b}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // Query step
        Console.WriteLine("\nConsult connections (example: 1 4). Type 'end' to finish this step.");
        while (true)
        {
            Console.Write("Consult: ");
            var entrada = Console.ReadLine();

            if (entrada.Trim().ToLower() == "end")
                break;

            var parts = entrada.Split();

            if (parts.Length != 2 || !int.TryParse(parts[0], out int a) || !int.TryParse(parts[1], out int b))
            {
                Console.WriteLine("Invalid entry. Try: <int> <int>");
                continue;
            }

            try
            {
                bool connected = network.Query(a, b);
                int level = network.LevelConnection(a, b);

                if (connected)
                {
                    if (level == 1)
                        Console.WriteLine($"The elements {a} and {b} are directly connected (level {level}).");
                    else
                        Console.WriteLine($"The elements {a} and {b} are indirectly connected (level {level}).");
                }
                else
                {
                    Console.WriteLine($"The elements {a} and {b} are not connected.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        Console.WriteLine("\nProgram ended."); // End of the program
    }
}
