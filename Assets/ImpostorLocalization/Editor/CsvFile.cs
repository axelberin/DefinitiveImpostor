using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

internal sealed class CsvFile
{
    private readonly Dictionary<string, int> columnIndexes;

    public IReadOnlyList<string> Headers { get; }
    public IReadOnlyList<CsvRow> Rows { get; }

    private CsvFile(List<string> headers, List<CsvRow> rows)
    {
        Headers = headers;
        Rows = rows;
        columnIndexes = new Dictionary<string, int>(StringComparer.Ordinal);

        for (int i = 0; i < headers.Count; i++)
        {
            string header = headers[i].TrimStart('\uFEFF');
            if (!columnIndexes.TryAdd(header, i))
                throw new InvalidDataException($"Duplicate CSV column: {header}");
        }

        foreach (CsvRow row in rows)
            row.SetColumnIndexes(columnIndexes);
    }

    public void RequireColumns(params string[] names)
    {
        foreach (string name in names)
        {
            if (!columnIndexes.ContainsKey(name))
                throw new InvalidDataException($"Missing required CSV column: {name}");
        }
    }

    public static CsvFile Read(string assetPath)
    {
        if (!File.Exists(assetPath))
            throw new FileNotFoundException("CSV source was not found.", assetPath);

        string text = File.ReadAllText(assetPath, new UTF8Encoding(true));
        List<List<string>> records = Parse(text);
        if (records.Count == 0)
            throw new InvalidDataException($"CSV is empty: {assetPath}");

        List<string> headers = records[0];
        List<CsvRow> rows = new();
        for (int i = 1; i < records.Count; i++)
        {
            if (records[i].Count == 1 && string.IsNullOrWhiteSpace(records[i][0]))
                continue;

            while (records[i].Count < headers.Count)
                records[i].Add(string.Empty);

            if (records[i].Count != headers.Count)
                throw new InvalidDataException($"CSV row {i + 1} has {records[i].Count} columns; expected {headers.Count}.");

            rows.Add(new CsvRow(assetPath, i + 1, records[i]));
        }

        return new CsvFile(headers, rows);
    }

    private static List<List<string>> Parse(string text)
    {
        List<List<string>> rows = new();
        List<string> row = new();
        StringBuilder field = new();
        bool quoted = false;

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (quoted)
            {
                if (c == '"')
                {
                    if (i + 1 < text.Length && text[i + 1] == '"')
                    {
                        field.Append('"');
                        i++;
                    }
                    else
                    {
                        quoted = false;
                    }
                }
                else
                {
                    field.Append(c);
                }

                continue;
            }

            switch (c)
            {
                case '"' when field.Length == 0:
                    quoted = true;
                    break;
                case ',':
                    row.Add(field.ToString());
                    field.Clear();
                    break;
                case '\r':
                    if (i + 1 < text.Length && text[i + 1] == '\n')
                        i++;
                    row.Add(field.ToString());
                    field.Clear();
                    rows.Add(row);
                    row = new List<string>();
                    break;
                case '\n':
                    row.Add(field.ToString());
                    field.Clear();
                    rows.Add(row);
                    row = new List<string>();
                    break;
                default:
                    field.Append(c);
                    break;
            }
        }

        if (quoted)
            throw new InvalidDataException("CSV contains an unterminated quoted field.");

        if (field.Length > 0 || row.Count > 0)
        {
            row.Add(field.ToString());
            rows.Add(row);
        }

        return rows;
    }
}

internal sealed class CsvRow
{
    private readonly string file;
    private readonly int line;
    private readonly List<string> values;
    private Dictionary<string, int> columnIndexes;

    public CsvRow(string file, int line, List<string> values)
    {
        this.file = file;
        this.line = line;
        this.values = values;
    }

    public string this[string column]
    {
        get
        {
            if (columnIndexes == null || !columnIndexes.TryGetValue(column, out int index))
                throw new InvalidDataException($"Unknown CSV column '{column}' in {file}.");

            return values[index].Trim();
        }
    }

    public int Line => line;
    public string File => file;

    public void SetColumnIndexes(Dictionary<string, int> indexes)
    {
        columnIndexes = indexes;
    }

    public string Require(string column)
    {
        string value = this[column];
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidDataException($"{file}:{line} requires a value in '{column}'.");
        return value;
    }
}
