using System;
using System.Text.RegularExpressions;
using System.IO;

using Npgsql;
using System.Data;

using Rooletochka;

class Test
{
    public static string GenRandomString(int size) {
        string result = "";
        while (result.Length < size) {
            result += Path.GetRandomFileName();
        }
        return result.Remove(size - 1);
    }
    public static void Main() {
        string connect = "Server=127.0.0.1;Port=5432;User Id=s;Database=practice;";
        var connection = new NpgsqlConnection(connect);
        connection.Open();
        if(connection.State == ConnectionState.Open) { Console.WriteLine("zbs"); }

        Table users = new Table("users", connection);

        // Insert works fine.
        //
        // users.Insert("'mala', 'fia'", "name, password");

        // Select() is fine too.
        //
        users = users.Select("*").Where("id > 3").All();

        // Print all usernames.
        //
        while(users.data.Read()) {
            Console.WriteLine(users.data.GetString(1));
        }

        // Regex at switch? Easy.
        // It can't be implemented, because C# wants constant value in case.
        //
        string rabbit = "1488";
        if (Regex.IsMatch(rabbit, "^14")) { Console.WriteLine("zbs"); }
        if (Regex.IsMatch(rabbit, "88$")) { Console.WriteLine("also zbs"); }
        if (Regex.IsMatch(rabbit, "^1.*8$")) { Console.WriteLine("perfect"); }
        if (Regex.IsMatch(rabbit, "228")) { Console.WriteLine("wut?"); }

        Console.WriteLine(GenRandomString(26));

        connection.Close();
    }
}