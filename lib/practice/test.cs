using System;
using Rooletochka;
using Npgsql;
using System.Data;

class Test
{
    public static void Main()
    {
        string connect = "Server=127.0.0.1;Port=5432;User Id=s;Database=practice;";
        var connection = new NpgsqlConnection(connect);
        connection.Open();
        Table users = new Table("users", connection);
        
        NpgsqlCommand npgSqlCommand = new NpgsqlCommand("SELECT * FROM users", connection);
        NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader();

        users.Insert("'mala', 'fia'", "name, password");
        //users.Select("*").All();
        Console.WriteLine("16th string");
        connection.Close();
    }
}
