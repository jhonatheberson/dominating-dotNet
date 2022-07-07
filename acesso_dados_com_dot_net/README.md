# Preparando ambiente de desenvolvimento
  - instalação do [dotNet](https://dotnet.microsoft.com/en-us/download)
  - criação de arquivo [docker-compose.yml](https://github.com/jhonatheberson/dominating-dotNet/blob/master/docker-compose.yml)
  - instalação de ferramenta acesso banco [Azure Data Studio](https://docs.microsoft.com/pt-br/sql/azure-data-studio/download-azure-data-studio?view=sql-server-ver16)


# Conexão com banco
   - ADO.NET 


  ### Vamos criar uma do tipo console
   ~~~bash
    dotnet new console
   ~~~

  perceba quel ele criou a seguinte estrutura:

  - nome_da_pasta
    - obj/
    - nome_da_pasta.csproj 
    - Program.cs

  para isso vamos usar Nuget **Microsoft.Data.SqlClient**

  usar pacotes de fontes com segurança

  para dicionar o pacote utilizamos o seguinte comando:

  ~~~bash
    dotnet add package <nome do pacote>
  ~~~

  em nosso caso 

  ~~~bash
  dotnet add package Microsoft.data.sqlclient
  ~~~

  se você quer setar uma versão especifica apenas faça essa pequena mudança:

  ~~~bash
  dotnet add package Microsoft.data.sqlclient --version 2.1.3
  ~~~

  e se você quer remover o pacote execute o comando a seguir:

  ~~~bash
  dotnet remove package Microsoft.data.sqlclient
  ~~~

  perceba que dentro de **acesso_dados_com_dot_net.csproj** ele incluio ReferencePackage

  ~~~cs
  <ItemGroup>
  <PackageReference Include="Microsoft.data.sqlclient" Version="4.1.0" />
  </ItemGroup>
  ~~~

  Agora para usar essa package utilizamos **using**:

  ~~~cs
  using Microsoft.Data.SqlClient;
  ~~~

  o SQL Server, e varios banco utiliza **pull de conexões**, logo sempre que **abrimos** uma conexão com banco e realizamos todas as operações, **fechamos** a conexão


~~~cs
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
~~~

# Dapper
  é uma extenção do Microsftware Data Client, ele faz o mapeamento.

para isso vamos instalar o [Dapper](https://github.com/DapperLib/Dapper) usando o seguinte commando:

~~~bash
dotnet add package Dapper
~~~

perceba novamente que podemos abrir o arquivo **acesso_dados_com_dot_net.csproj** e verificar qual package foi adicionado.


agora que criamos instalamaos o pacote, vamos criar nosso modelo.


## Models
 - geralmente para cada tabela do banco relacional, teremos um model associado a ela, porém isso não é uma verdade absoluta, apenas boas práticas

 logo iremos criar uma pasta **./Models/** e dentro dela nosso primeiro arquivo referente a uma tabela do banco em nosso caso é **Student.cs**

 logo nossa entrutura de projeto está assim:

  - Models/
    - Student.cs
  - bin/
  - obj/

  se olharmos dentro do model Student.cs:

  ~~~cs
  using System;

  namespace BaltaDataAcces.Models
  {
    public class Student
    {
      public Guid Id { get; set; }
      public String Name { get; set; }
      public String Email { get; set; }
      public String Document { get; set; }
      public String Phone { get; set; }
      public DateTime Birthdate { get; set; }
      public DateTime CreateDate { get; set; }
    }

  }
  ~~~

  perceba que esse arquivo tem as colunas da tabela Student, essencialmente. e com isso o **Dapper** nos permite o uso da metodo **Query**


  logo para realizar a mesma operação, porém de forma mais auto nivel, utilizando o **Dapper** fica desta forma:

  ~~~cs
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

  ~~~

  **perceba que com uso do Model, ganhamos um poder importante que é intelissense, assim evitando falha no desenvolvimento.**

  é importante notar que o nome das colunas da tabela **Student** no banco é o mesmo do Model **Student.cs**, isso é necessario, porque senão o **Dapper** não conseguirá diferenciar a não ser que utilize **AS**  para apelidar.



  ~~~cs
  using (var connection = new SqlConnection(connectionString))
  {
      var students = connection.Query<Student>("SELECT TOP (1000) [Id] AS Codigo,[Name] AS Nome FROM [dbo].[Student]");
      foreach (var student in students)
      {
        Console.WriteLine($"{student.Id} - {student.Name}");
      }
  }
  ~~~

  **precisamos entender o que está acontecendo em baixo nível, mas isso ficará mais claro nas proximos tutoriais** 


## INSERT

  - agora vamos realizar uma inserção no banco utilizando o **Dapper**


  para isso inicialmente entender que **NUNCA** vamos deixar string de inserção relizar concatenação, para que não ocorra **SQL INJECTION** para isso vamos utilizar parametros para poder passar as infoemações necessarias para realização do **INSERT**, esse parametrôs são identificados com "@" na frente da variavel da seguinte forma:


  ~~~cs
  using BaltaDataAcces.Models;
  using Dapper;
  using Microsoft.Data.SqlClient;

  // See https://aka.ms/new-console-template for more information
  Console.WriteLine("Hello, World!");

  const string connectionString = "Server=worldofai.database.windows.net;Database=balta;User ID=jhonatheberson;Password=BLAZEjoao55@#";

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

  using (var connection = new SqlConnection(connectionString))
  {
      // EVITAR PROCESSAMENTO AQUI DENTRO, PORQUE A CONEXÃO ESTÁ ABERTA

      //UTILIZANDO PARAMETROS PARA MONTAR SQL EXECUTE
      var rows = connection.Execute(insertSql, new {
        student.Id,
        Nome = student.Name,
        student.Email,
        student.Document,
        student.Phone,
        student.Birthdate,
        student.CreateDate
      });

      Console.WriteLine($"{rows} linhas inderidas");

      var students = connection.Query<Student>("SELECT TOP (1000) [Id],[Name],[Email],[Document],[Phone],[Birthdate],[CreateDate]FROM[dbo].[Student]");
      foreach (var item in students)
      {
        Console.WriteLine($"{item.Id} - {item.Name}");
      }
  }

  ~~~

  O metodo **Execute**  vai ser utilizado tanto para **INSERT**, **UPDATE** e **DELETE**


## UPDATE

  - antes de iniciar o update, vamos refatorar o codigo, deixando mais modular e limpando de forma mais clara, então ficamos assim:


  ~~~cs
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
  ~~~


  com essa mudança podemos criar nosso metodo **UpdateStudent**

  ~~~cs
  static void UpdateStudent(SqlConnection connection)
  {
    var updateQuery = "UPDATE [Student] SET [Email]=@Email WHERE [Id]=@Id";

    var rows = connection.Execute(updateQuery, new {
      Id = new Guid("32a8c9d6-cced-478a-89b7-2adb6d5c33fa"),
      Email = "updatefuncionou@gmail.com"
    });

    Console.WriteLine($"{rows} registros atualizados");
  }
  ~~~


  agora chamamos esse metodo na função **main** da seguinte maneira:
  
  ~~~cs
  static void Main(string[] args)
    {

      const string connectionString = "Server=worldofai.database.windows.net;Database=balta;User ID=jhonatheberson;Password=BLAZEjoao55@#";


      using (var connection = new SqlConnection(connectionString))
      {
        // EVITAR PROCESSAMENTO AQUI DENTRO, PORQUE A CONEXÃO ESTÁ ABERTA
        UpdateStudent(connection);
        ListStudents(connection);
        // CreateStudent(connection);


      }

    }
  ~~~
  
  e pronto, basta executar o prejeto:


  ~~~bash
  dotnet run
  ~~~

# Imersão

  -  agora vamos utilizar arquitetura algumas SQL mais dethalhados

  ## Execute Many

  - inicialmente vamos cirar o metodo **DELETE** para finalizar nossa **CRUD** nosso metodo DELETE ficou desta forma:


    ~~~cs
    static void DeleteStudent(SqlConnection connection)
    {
      var updateQuery = "DELETE FROM [Student] WHERE [Id]=@Id";

      var rows = connection.Execute(updateQuery, new {
        Id = new Guid("32a8c9d6-cced-478a-89b7-2adb6d5c33fa")
      });

      Console.WriteLine($"{rows} registros deletado");
    }
    ~~~

    agora com CRUD criado, vamos utilizar para criar Varios **Student** ou seja **Many**:

    ~~~cs
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
    ~~~

    perceba que as principais diferencias que agora estou passando uma lista no **Execute**, essa forma também serve para **UpdateMany** como **DeleteMany**.

## Executando procedures

- agora vamos mostrar como executar procedures dentro do banco, utilizando C#, mas para isso precisamos criar a procedure no banco, assim executando este script:


  ~~~sql
  CREATE OR ALTER PROCEDURE spDeleteStudent (
      @StudentId UNIQUEIDENTIFIER
  )
  AS
      BEGIN TRANSACTION
          DELETE FROM 
              [StudentCourse] 
          WHERE 
              [StudentId] = @StudentId

          DELETE FROM 
              [Student] 
          WHERE 
              [Id] = @StudentId
      COMMIT
  ~~~

  essencialmente essa procedure executa **TRANSACTION** deletando sdudent

  nosso metodo que executa proceddure ficou desa forma:

  ~~~cs
  static void ExecuteProcedure(SqlConnection connection)
  {
    var procedure = "[spDeleteStudent]";
    var pars = new { StudentId = "3140e182-f7ff-4392-b09f-bf1e3f0461b4" };
    var affectedRows = connection.Execute(procedure, pars, commandType: CommandType.StoredProcedure);

    Console.WriteLine($"{affectedRows} linhas afetadas");
  }
  ~~~


  algumas coisa são importante observar nesse metodo, primeiramente que precisamos passar **commandType** a qual para execução de procedure é **System.Data.CommandType.StoredProcedure**. 

  também perceba que a procedure recebe parametros, uma boa prática é utilizar o mesmo nome do parâmetros da procedura também para executação em que nosso caso é **StudentId**

## Lendo Procedures

- aqui a diferença que queremos pegar o retorno da procedure