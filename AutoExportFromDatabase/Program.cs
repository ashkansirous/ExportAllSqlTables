// See https://aka.ms/new-console-template for more information

using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;

string connectionString = "Server=xxx.database.windows.net;Database=xxx;User=xxx;Password=xxx;";
SqlConnection connection = new SqlConnection(connectionString);

try
{
    connection.Open();
    DataTable tables = connection.GetSchema("Tables");
    string outputPath = @"C:\OutputFolder\"; // Change this to your desired output folder

    if (!Directory.Exists(outputPath))
    {
        Directory.CreateDirectory(outputPath);
    }

    foreach (DataRow table in tables.Rows)
    {
        string? name = table["TABLE_NAME"].ToString();
        string? schema = table["TABLE_SCHEMA"].ToString();
        var tableName = $"{schema}.{name}";
        if (table["TABLE_TYPE"].ToString()?.ToLower() == "view")
        {
            continue;
        }
        Console.WriteLine($"Exporting data from table:{tableName}");

        // Create a SQL command to select all data from the table
        string selectQuery = $"SELECT * FROM {tableName}";
        SqlDataAdapter dataAdapter = new SqlDataAdapter(selectQuery, connection);
        DataTable dataTable = new DataTable();

        // Fill the data into a DataTable
        dataAdapter.Fill(dataTable);

        // Convert DataTable to JSON
        string json = JsonConvert.SerializeObject(dataTable);

        // Save JSON to a file
        string jsonFilePath = Path.Combine(outputPath, tableName + ".json");
        File.WriteAllText(jsonFilePath, json);

        Console.WriteLine($"Data from table {tableName} saved to {jsonFilePath}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}
finally
{
    connection.Close();
}
