using System;
using System.Collections.Generic;

// Classe que representa uma rede de elementos conectados
public class Rede
{
    private readonly int _tamanho; // Número total de elementos
    private readonly Dictionary<int, HashSet<int>> _conexoes; // Armazena as conexões

    public Rede(int tamanho)
    {
        if (tamanho <= 0)
            throw new ArgumentException("O tamanho deve ser um número inteiro positivo."); // Valida que o tamanho seja positivo

        _tamanho = tamanho;
        _conexoes = new Dictionary<int, HashSet<int>>();

        // Inicializa cada elemento com um conjunto vazio de conexões
        for (int i = 1; i <= tamanho; i++)
        {
            _conexoes[i] = new HashSet<int>();
        }
    }

    // Conecta dois elementos de forma bidirecional
    public void Conectar(int a, int b)
    {
        ValidarElemento(a);
        ValidarElemento(b);

        if (a == b)
            throw new ArgumentException("Não é possível conectar um elemento a si mesmo."); // Impede auto-conexão

        _conexoes[a].Add(b); // Adiciona b à lista de conexões de a
        _conexoes[b].Add(a); // Adiciona a à lista de conexões de b
    }

    // Desconecta dois elementos
    public void Desconectar(int a, int b)
    {
        ValidarElemento(a);
        ValidarElemento(b);

        if (!_conexoes[a].Contains(b))
            throw new InvalidOperationException("Os elementos não estão conectados."); // Só desconecta se estiverem conectados

        _conexoes[a].Remove(b);
        _conexoes[b].Remove(a);
    }

    // Verifica se dois elementos estão conectados direta ou indiretamente
    public bool Consultar(int a, int b)
    {
        ValidarElemento(a);
        ValidarElemento(b);
        return BFS(a, b) != -1; // Usa busca em largura (BFS)
    }

    // Retorna o nível de conexão entre dois elementos (0 se não estiverem conectados)
    public int NivelConexao(int a, int b)
    {
        ValidarElemento(a);
        ValidarElemento(b);

        if (a == b)
            return 0;

        int nivel = BFS(a, b); // Realiza BFS para encontrar o menor caminho
        return nivel == -1 ? 0 : nivel;
    }

    // Busca em largura para encontrar o caminho mais curto entre elementos
    private int BFS(int inicio, int alvo)
    {
        if (inicio == alvo)
            return 0;

        var visitados = new HashSet<int>(); // Rastreia os nós visitados
        var fila = new Queue<(int no, int nivel)>(); // Fila com tuplas (nó, nível)
        fila.Enqueue((inicio, 0));
        visitados.Add(inicio);

        while (fila.Count > 0)
        {
            var (atual, nivel) = fila.Dequeue();

            foreach (var vizinho in _conexoes[atual])
            {
                if (vizinho == alvo)
                    return nivel + 1;

                if (!visitados.Contains(vizinho))
                {
                    visitados.Add(vizinho);
                    fila.Enqueue((vizinho, nivel + 1));
                }
            }
        }

        return -1; // Alvo não encontrado
    }

    // Valida se um elemento está dentro do intervalo permitido
    private void ValidarElemento(int elemento)
    {
        if (elemento < 1 || elemento > _tamanho)
            throw new ArgumentOutOfRangeException($"Elemento {elemento} está fora do intervalo válido (1 a {_tamanho}).");

        if (!_conexoes.ContainsKey(elemento))
            throw new ArgumentException($"Elemento {elemento} não está inicializado.");
    }
}

// Programa principal: interação com o usuário
class Programa
{
    static void Main()
    {
        Console.Write("Quantos elementos? (pressione Enter para o padrão 6): ");
        string? entrada = Console.ReadLine();
        int total;

        // Define 6 como padrão se o usuário não digitar nada
        if (string.IsNullOrWhiteSpace(entrada))
        {
            total = 6;
            Console.WriteLine("Usando o valor padrão de 6 elementos.");
        }
        else
        {
            // Continua pedindo até que seja fornecido um número positivo válido
            while (!int.TryParse(entrada, out total) || total <= 0)
            {
                Console.Write("Digite um número inteiro positivo: ");
                entrada = Console.ReadLine();
            }
        }

        Rede rede = new Rede(total); // Inicializa a rede

        Console.WriteLine($"\nNúmeros disponíveis: 1 - {total}");
        Console.WriteLine("\nDigite os elementos que deseja conectar (exemplo: 1 2). Digite 'fim' para encerrar esta etapa.");
        int totalConexoes = 0;

        // Etapa de conexão
        while (true)
        {
            Console.Write("Conectar: ");
            var linha = Console.ReadLine();

            if (linha.Trim().ToLower() == "fim")
                break;

            var partes = linha.Split();

            if (partes.Length != 2 || !int.TryParse(partes[0], out int a) || !int.TryParse(partes[1], out int b))
            {
                Console.WriteLine("Entrada inválida. Tente: <int> <int>");
                continue;
            }

            try
            {
                rede.Conectar(a, b);
                totalConexoes++;
                Console.WriteLine($"Conectado com sucesso: {a} e {b}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
        }

        Console.WriteLine($"\nTotal de conexões feitas: {totalConexoes}");

        // Etapa de desconexão
        Console.WriteLine("\nDesconectar conexões (exemplo: 1 2). Digite 'fim' para encerrar.");
        while (true)
        {
            Console.Write("Desconectar: ");
            var linha = Console.ReadLine();

            if (linha.Trim().ToLower() == "fim")
                break;

            var partes = linha.Split();

            if (partes.Length != 2 || !int.TryParse(partes[0], out int a) || !int.TryParse(partes[1], out int b))
            {
                Console.WriteLine("Entrada inválida. Tente: <int> <int>");
                continue;
            }

            try
            {
                rede.Desconectar(a, b);
                Console.WriteLine($"Desconectado: {a} e {b}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
        }

        // Etapa de consulta
        Console.WriteLine("\nConsultar conexões (exemplo: 1 4). Digite 'fim' para encerrar esta etapa.");
        while (true)
        {
            Console.Write("Consultar: ");
            var linha = Console.ReadLine();

            if (linha.Trim().ToLower() == "fim")
                break;

            var partes = linha.Split();

            if (partes.Length != 2 || !int.TryParse(partes[0], out int a) || !int.TryParse(partes[1], out int b))
            {
                Console.WriteLine("Entrada inválida. Tente: <int> <int>");
                continue;
            }

            try
            {
                bool conectados = rede.Consultar(a, b);
                int nivel = rede.NivelConexao(a, b);

                if (conectados)
                {
                    if (nivel == 1)
                        Console.WriteLine($"Os elementos {a} e {b} estão diretamente conectados (nível {nivel}).");
                    else
                        Console.WriteLine($"Os elementos {a} e {b} estão indiretamente conectados (nível {nivel}).");
                }
                else
                {
                    Console.WriteLine($"Os elementos {a} e {b} não estão conectados.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
        }

        Console.WriteLine("\nPrograma finalizado."); // Fim do programa
    }
}
