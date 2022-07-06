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

  ***perceba que com uso do Model, ganhamos um poder importante que é intelissense, assim evitando falha no desenvolvimento.***