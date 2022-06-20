var currencyConverter = Singleton.Instance;
currencyConverter.UpdateConfiguration(new Tuple<string, string, double>("usd", "cad", 1.34));
currencyConverter.UpdateConfiguration(new Tuple<string, string, double>("cad", "gbp", 0.58));
currencyConverter.UpdateConfiguration(new Tuple<string, string, double>("usd", "eur", 0.86));
var result = currencyConverter.Convert("cad", "eur", 10);
Console.WriteLine($"10 cad to eur : {result}");
Console.ReadKey();

public sealed class Singleton
{
    private Singleton()
    {
    }

    public static ICurrencyConverter Instance { get { return Nested.instance; } }

    private class Nested
    {
        static Nested()
        {
        }

        internal static readonly ICurrencyConverter instance = new CurrencyConverter();
    }
}

public interface ICurrencyConverter
{
    /// <summary>
    /// Clears any prior configuration.
    /// </summary>
    void ClearConfiguration();
    /// <summary>
    /// Updates the configuration. Rates are inserted or replaced internally.
    /// </summary>
    void UpdateConfiguration(IEnumerable<Tuple<string, string, double>> conversionRates);
    void UpdateConfiguration(Tuple<string, string, double> conversionRate);
    /// <summary>
    /// Converts the specified amount to the desired currency.
    /// </summary>
    double Convert(string fromCurrency, string toCurrency, double amount);
}
public class CurrencyConverter : ICurrencyConverter
{
    private List<string> _vertices;
    private List<Tuple<string, string, double>> _edges;
    private Dictionary<string, CurrencyFlow> _baseCurrencyConversionPath;
    private Graph<string> _graph;
    private Algorithms _algorithms;
    public CurrencyConverter()
    {
        ClearConfiguration();
    }
    public void ClearConfiguration()
    {
        _vertices = new List<string>();
        _edges = new List<Tuple<string, string, double>>();
        _baseCurrencyConversionPath = new Dictionary<string, CurrencyFlow>();
        _graph = new Graph<string>();
        _algorithms = new Algorithms();
    }

    public double Convert(string fromCurrency, string toCurrency, double amount)
    {
        var shortestPath = _algorithms.ShortestPathFunction(_graph, fromCurrency);
        var path = string.Join("-", shortestPath(toCurrency));
        // jahate namayesh nazdiktarin masir 
        Console.WriteLine($"Path: {path}");
        var splitedNode = path.Split("-");
        var currencyExprNode = new Dictionary<string, CurrencyFlow>();
        var finalExpr = amount;
        var exprAsString = amount.ToString();
        for (int i = 0; i < splitedNode.Length - 1; i++)
        {
            var baseConversion = _baseCurrencyConversionPath.FirstOrDefault(x => x.Key == splitedNode[i] + "-" + splitedNode[i + 1]);
            finalExpr = MiniMath.Calc(finalExpr, baseConversion.Value.OP, baseConversion.Value.Value);
            exprAsString += $"{baseConversion.Value.OP} {baseConversion.Value.Value.ToString().Replace("/",".")}";
        }
        // jahate namayesh ebarate mohasebe shode dar tabdile arzha
        Console.WriteLine($"Expr: {exprAsString}");
        return finalExpr;
    }


    public void UpdateConfiguration(Tuple<string, string, double> conversionRate)
    {
        if (!_vertices.Contains(conversionRate.Item1))
            _vertices.Add(conversionRate.Item1);
        if (!_vertices.Contains(conversionRate.Item2))
            _vertices.Add(conversionRate.Item2);
        _edges.Add(conversionRate);
        _graph = new Graph<string>(_vertices, _edges);
        _baseCurrencyConversionPath.Add($"{conversionRate.Item1}-{conversionRate.Item2}", new CurrencyFlow('*', conversionRate.Item3));
        _baseCurrencyConversionPath.Add($"{conversionRate.Item2}-{conversionRate.Item1}", new CurrencyFlow('/', conversionRate.Item3));
    }


    public void UpdateConfiguration(IEnumerable<Tuple<string, string, double>> conversionRates)
    {
        foreach (var item in conversionRates)
        {
            UpdateConfiguration(item);
        }
    }
}
public struct CurrencyFlow
{
    public CurrencyFlow(char op, double value)
    {
        OP = op;
        Value = value;
    }
    public char OP { get; set; }
    public double Value { get; set; }
}
public class Graph<T>
{
    public Graph() { }
    public Graph(IEnumerable<string> vertices, IEnumerable<Tuple<string, string, double>> edges)
    {
        foreach (var vertex in vertices)
            AddVertex(vertex);

        foreach (var edge in edges)
            AddEdge(edge);
    }
    public Dictionary<string, HashSet<string>> AdjacencyList { get; } = new Dictionary<string, HashSet<string>>();
    public void AddVertex(string vertex)
    {
        AdjacencyList[vertex] = new HashSet<string>();
    }
    public void AddEdge(Tuple<string, string, double> edge)
    {
        if (AdjacencyList.ContainsKey(edge.Item1) && AdjacencyList.ContainsKey(edge.Item2))
        {
            AdjacencyList[edge.Item1].Add(edge.Item2);
            AdjacencyList[edge.Item2].Add(edge.Item1);
        }
    }
}
public class Algorithms
{
    public Func<string, IEnumerable<string>> ShortestPathFunction(Graph<string> graph, string start)
    {
        var previous = new Dictionary<string, string>();

        var queue = new Queue<string>();

        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var vertex = queue.Dequeue();
            foreach (var neighbor in graph.AdjacencyList[vertex])
            {
                if (previous.ContainsKey(neighbor))
                    continue;

                previous[neighbor] = vertex;
                queue.Enqueue(neighbor);
            }
        }

        Func<string, IEnumerable<string>> shortestPath = v =>
        {
            var path = new List<string> { };

            var current = v;
            while (!current.Equals(start))
            {
                path.Add(current);
                current = previous[current];
            };

            path.Add(start);
            path.Reverse();

            return path;
        };

        return shortestPath;
    }
}
public static class MiniMath
{
    public static double Calc(double baseValue, char op, double operand)
    {
        return op == '*' ? baseValue * operand : baseValue / operand;
    }
}