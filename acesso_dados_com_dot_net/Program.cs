using BaltaDataAcces.Models;
using Dapper;
using Microsoft.Data.SqlClient;



namespace BaltaDataAccess
{
  class Program
  {
    static void Main(string[] args)
    {

      const string connectionString = "Server=worldofai.database.windows.net;Database=balta;User ID=jhonatheberson;Password=BLAZEjoao55@#";


      using (var connection = new SqlConnection(connectionString))
      {
        // EVITAR PROCESSAMENTO AQUI DENTRO, PORQUE A CONEXÃO ESTÁ ABERTA

        ListStudents(connection);
        // CreateStudent(connection);


      }

    }
    static void ListStudents(SqlConnection connection)
    {

      var students = connection.Query<Student>("SELECT TOP (1000) [Id],[Name],[Email],[Document],[Phone],[Birthdate],[CreateDate]FROM[dbo].[Student]");
      foreach (var item in students)
      {
        Console.WriteLine($"{item.Id} - {item.Name}");
      }

    }
    static void CreateStudent(SqlConnection connection)
    {
      var student = new Student();
      student.Id = Guid.NewGuid();
      student.Name = "jhonat";
      student.Email = "jhonat@gmail.com";
      student.Phone = "84976479384";
      student.Birthdate = DateTime.Parse("2/16/2008 12:15:12 PM");
      student.CreateDate = DateTime.Now;

      // NÃO VAMOS CONCATENAR STRING EM SQL, PARA NÃO OCORRER SQL INJECTION
      // O "@" FAZ QUE EU POSSA PELAS AS LINHAS
      // O "$" FAZ QUE NÃO POSSA CONCATENAR
      // VAMOS OPTAR POR RECEBER PARÂMETROS E RECEBE PARA METROS PELO "@"
      var insertSql = @"INSERT INTO 
        [Student] 
      VALUES(
        @Id,
        @Nome,
        @Email,
        @Document,
        @Phone,
        @Birthdate,
        @CreateDate)"; //vamos evitar comandos SQL como NEWID(), no C#


      //UTILIZANDO PARAMETROS PARA MONTAR SQL EXECUTE
      var rows = connection.Execute(insertSql, new
      {
        student.Id,
        Nome = student.Name,
        student.Email,
        student.Document,
        student.Phone,
        student.Birthdate,
        student.CreateDate
      });

      Console.WriteLine($"{rows} linhas inderidas");
    }
  }

}

