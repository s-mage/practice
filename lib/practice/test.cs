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
        if(connection.State == ConnectionState.Open) { Console.WriteLine("zbs"); }

        Table users = new Table("users", connection);

        // Insert works fine.
        //
        users.Insert("'mala', 'fia'", "name, password");

        // Select() is fine too.
        //
        users = users.Select("*").Where("id > 3").All();

        // Print all usernames.
        //
        while(users.data.Read()) {
            Console.WriteLine(users.data.GetString(1));
        }

        connection.Close();
    }
}
