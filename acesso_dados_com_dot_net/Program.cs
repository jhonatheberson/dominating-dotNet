using System.Data;
using BaltaDataAcces.Models;
using Dapper;
using Microsoft.Data.SqlClient;



namespace BaltaDataAccess
{
  class Program
  {
    static void Main(string[] args)
    {

      const string connectionString = "Server=balta.chrbdzefu2yy.us-east-1.rds.amazonaws.com;Database=balta;User ID=jhonatheberson;Password=BLAZEjoao55#";


      using (var connection = new SqlConnection(connectionString))
      {
        // EVITAR PROCESSAMENTO AQUI DENTRO, PORQUE A CONEXÃO ESTÁ ABERTA
        // UpdateStudent(connection);
        // DeleteStudent(connection);
        // ListStudents(connection);
        // CreateManyStudent(connection);
        // ListStudents(connection);
        // CreateStudent(connection);
        // ExecuteReadProcedure(connection);
        // ExecuteProcedure(connection);
        // ListStudents(connection);

        Console.WriteLine("conectou");
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

    static void UpdateStudent(SqlConnection connection)
    {
      var updateQuery = "UPDATE [Student] SET [Email]=@Email WHERE [Id]=@Id";

      var rows = connection.Execute(updateQuery, new
      {
        Id = new Guid("32a8c9d6-cced-478a-89b7-2adb6d5c33fa"),
        Email = "updatefuncionou@gmail.com"
      });

      Console.WriteLine($"{rows} registros atualizados");
    }

    static void DeleteStudent(SqlConnection connection)
    {
      var updateQuery = "DELETE FROM [Student] WHERE [Id]=@Id";

      var rows = connection.Execute(updateQuery, new
      {
        Id = new Guid("32a8c9d6-cced-478a-89b7-2adb6d5c33fa")
      });

      Console.WriteLine($"{rows} registros deletado");
    }

    static void CreateManyStudent(SqlConnection connection)
    {
      var student = new Student();
      student.Id = Guid.NewGuid();
      student.Name = "jhonat";
      student.Email = "jhonat@gmail.com";
      student.Phone = "84976479384";
      student.Birthdate = DateTime.Parse("2/16/2008 12:15:12 PM");
      student.CreateDate = DateTime.Now;

      var student2 = new Student();
      student2.Id = Guid.NewGuid();
      student2.Name = "jhonatnovo";
      student2.Email = "jhonatnovo@gmail.com";
      student2.Phone = "84976479384";
      student2.Birthdate = DateTime.Parse("4/28/1997 12:15:12 PM");
      student2.CreateDate = DateTime.Now;

      // NÃO VAMOS CONCATENAR STRING EM SQL, PARA NÃO OCORRER SQL INJECTION
      // O "@" FAZ QUE EU POSSA PELAS AS LINHAS
      // O "$" FAZ QUE NÃO POSSA CONCATENAR
      // VAMOS OPTAR POR RECEBER PARÂMETROS E RECEBE PARA METROS PELO "@"
      var insertSql = @"INSERT INTO 
        [Student] 
      VALUES(
        @Id,
        @Name,
        @Email,
        @Document,
        @Phone,
        @Birthdate,
        @CreateDate)"; //vamos evitar comandos SQL como NEWID(), no C#


      //UTILIZANDO PARAMETROS PARA MONTAR SQL EXECUTE
      var rows = connection.Execute(insertSql, new[]{
      new
      {
        student.Id,
        student.Name,
        student.Email,
        student.Document,
        student.Phone,
        student.Birthdate,
        student.CreateDate
      },
      new
      {
        student2.Id,
        student2.Name,
        student2.Email,
        student2.Document,
        student2.Phone,
        student2.Birthdate,
        student2.CreateDate
      }
      });

      Console.WriteLine($"{rows} linhas inderidas");
    }

    static void ExecuteProcedure(SqlConnection connection)
    {
      var procedure = "[spDeleteStudent]";
      var pars = new { StudentId = "3140e182-f7ff-4392-b09f-bf1e3f0461b4" };
      var affectedRows = connection.Execute(procedure, pars, commandType: CommandType.StoredProcedure);

      Console.WriteLine($"{affectedRows} linhas afetadas");
    }
    static void ExecuteReadProcedure(SqlConnection connection)
    {
      var procedure = "[spGetStudent]";
      var pars = new { StudentId = "c55390d4-71dd-4f3c-b978-d1582f51a327" };
      var students = connection.Query(procedure, pars, commandType: CommandType.StoredProcedure);

      foreach (var item in students)
      {
        Console.WriteLine(item.Name); // aqui a tipos não é disponivel, porque é variavel dinamica
      }
    }

  }

}

