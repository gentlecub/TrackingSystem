namespace TrackingSystem.App.UI;

/// <summary>
/// Generic class for printing formatted tables to console
/// </summary>
public class TablePrinter
{
    private readonly List<TableColumn> _columns = new();
    private readonly List<List<TableCell>> _rows = new();
    private string _title = "";

    /// <summary>
    /// Sets the table title
    /// </summary>
    public TablePrinter WithTitle(string title)
    {
        _title = title;
        return this;
    }

    /// <summary>
    /// Adds a column to the table
    /// </summary>
    public TablePrinter AddColumn(string header, int width, TextAlignment alignment = TextAlignment.Left)
    {
        _columns.Add(new TableColumn(header, width, alignment));
        return this;
    }

    /// <summary>
    /// Adds a data row to the table
    /// </summary>
    public TablePrinter AddRow(params object[] values)
    {
        var row = new List<TableCell>();
        for (int i = 0; i < values.Length && i < _columns.Count; i++)
        {
            var value = values[i]?.ToString() ?? "";
            row.Add(new TableCell(value));
        }
        _rows.Add(row);
        return this;
    }

    /// <summary>
    /// Adds a row with custom cells (including colors)
    /// </summary>
    public TablePrinter AddRow(params TableCell[] cells)
    {
        _rows.Add(cells.ToList());
        return this;
    }

    /// <summary>
    /// Prints the table to console
    /// </summary>
    public void Print()
    {
        if (_columns.Count == 0) return;

        int totalWidth = _columns.Sum(c => c.Width) + (_columns.Count * 3) + 1;

        // Title
        if (!string.IsNullOrEmpty(_title))
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(_title);
            Console.ResetColor();
        }

        // Top line
        Console.WriteLine(BuildHorizontalLine('┌', '┬', '┐'));

        // Headers
        Console.Write("│");
        foreach (var col in _columns)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($" {FormatCell(col.Header, col.Width, col.Alignment)} ");
            Console.ResetColor();
            Console.Write("│");
        }
        Console.WriteLine();

        // Separator line
        Console.WriteLine(BuildHorizontalLine('├', '┼', '┤'));

        // Data rows
        foreach (var row in _rows)
        {
            Console.Write("│");
            for (int i = 0; i < _columns.Count; i++)
            {
                var col = _columns[i];
                var cell = i < row.Count ? row[i] : new TableCell("");

                if (cell.Color.HasValue)
                {
                    Console.ForegroundColor = cell.Color.Value;
                }

                Console.Write($" {FormatCell(cell.Value, col.Width, col.Alignment)} ");

                if (cell.Color.HasValue)
                {
                    Console.ResetColor();
                }
                Console.Write("│");
            }
            Console.WriteLine();
        }

        // Bottom line
        Console.WriteLine(BuildHorizontalLine('└', '┴', '┘'));
    }

    /// <summary>
    /// Clears table rows (keeps columns)
    /// </summary>
    public TablePrinter ClearRows()
    {
        _rows.Clear();
        return this;
    }

    /// <summary>
    /// Completely clears the table
    /// </summary>
    public TablePrinter Clear()
    {
        _columns.Clear();
        _rows.Clear();
        _title = "";
        return this;
    }

    private string BuildHorizontalLine(char left, char middle, char right)
    {
        var parts = _columns.Select(c => new string('─', c.Width + 2));
        return left + string.Join(middle.ToString(), parts) + right;
    }

    private static string FormatCell(string value, int width, TextAlignment alignment)
    {
        if (string.IsNullOrEmpty(value))
            value = "";

        // Truncate if necessary
        if (value.Length > width)
        {
            value = value.Substring(0, width - 2) + "..";
        }

        return alignment switch
        {
            TextAlignment.Right => value.PadLeft(width),
            TextAlignment.Center => value.PadLeft((width + value.Length) / 2).PadRight(width),
            _ => value.PadRight(width)
        };
    }

    /// <summary>
    /// Creates a table from a collection of objects
    /// </summary>
    public static TablePrinter FromCollection<T>(
        IEnumerable<T> items,
        params (string Header, int Width, Func<T, object> Selector, Func<T, ConsoleColor?>? ColorSelector)[] columns)
    {
        var table = new TablePrinter();

        foreach (var col in columns)
        {
            table.AddColumn(col.Header, col.Width);
        }

        foreach (var item in items)
        {
            var cells = columns.Select(col =>
            {
                var value = col.Selector(item)?.ToString() ?? "";
                var color = col.ColorSelector?.Invoke(item);
                return new TableCell(value, color);
            }).ToArray();

            table.AddRow(cells);
        }

        return table;
    }
}

/// <summary>
/// Represents a table column
/// </summary>
public class TableColumn
{
    public string Header { get; }
    public int Width { get; }
    public TextAlignment Alignment { get; }

    public TableColumn(string header, int width, TextAlignment alignment = TextAlignment.Left)
    {
        Header = header;
        Width = width;
        Alignment = alignment;
    }
}

/// <summary>
/// Represents a table cell
/// </summary>
public class TableCell
{
    public string Value { get; }
    public ConsoleColor? Color { get; }

    public TableCell(string value, ConsoleColor? color = null)
    {
        Value = value;
        Color = color;
    }

    // Implicit conversion from string
    public static implicit operator TableCell(string value) => new TableCell(value);
}

/// <summary>
/// Text alignment in the cell
/// </summary>
public enum TextAlignment
{
    Left,
    Center,
    Right
}
