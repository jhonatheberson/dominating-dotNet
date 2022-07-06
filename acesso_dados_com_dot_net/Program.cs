using Microsoft.Data.SqlClient;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

const string connectionString = "Server=worldofai.database.windows.net;Database=balta;User ID=jhonatheberson;Password=BLAZEjoao55@#";

// var connection = new SqlConnection();
// connection.Open();

// connection.Close();
// connection.Dispose(); //ele destroi a conexão,

// var connection = new SqlConnection(); // preciso instanciar novemente a conexão

// forma mais otmizada
using (var connection = new SqlConnection(connectionString))
{
  connection.Open();
  Console.WriteLine("conectado");

  using (var command = new SqlCommand())
  {
    command.Connection = connection;
    command.CommandType = System.Data.CommandType.Text;
    command.CommandText = "SELECT TOP (1000) [Id],[Name],[Email],[Document],[Phone],[Birthdate],[CreateDate]FROM[dbo].[Student]";

    var reader = command.ExecuteReader();
    while (reader.Read())
    {
      Console.WriteLine($"{reader.GetGuid(0)} - {reader.GetString(1)}");
    }
  }
}

