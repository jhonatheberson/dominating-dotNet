using BaltaDataAcces.Models;
using Dapper;
using Microsoft.Data.SqlClient;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

const string connectionString = "Server=worldofai.database.windows.net;Database=balta;User ID=jhonatheberson;Password=BLAZEjoao55@#";


using (var connection = new SqlConnection(connectionString))
{
    var students = connection.Query<Student>("SELECT TOP (1000) [Id],[Name],[Email],[Document],[Phone],[Birthdate],[CreateDate]FROM[dbo].[Student]");
    foreach (var student in students)
    {
      Console.WriteLine($"{student.Id} - {student.Name}");
    }
}

