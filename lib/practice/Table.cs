using System;
using System.Data;
using Npgsql;

// Rooletochka is just like Rooletka, but a little smaller.
//
namespace Rooletochka
{
    // Implements table of database, because writing sql commands
    // is not good idea.
    //
    // The idea of the class is the following. It would be good
    // to have an object-oriented implementation of all this sql-
    // commands. As it known, result of any sql-query is table.
    // It looks evident to implement class Table and some common
    // methods, such as select and insert.
    //
    public class Table
    {
        private string command;
        private NpgsqlConnection connection;
        private NpgsqlDataReader data;

        public Table(string com, NpgsqlConnection con)
        {
            command = com;
            connection = con;
            data = commandToData(command);
        }

        public Table(string com)
        {
            Console.Write("User Id: ");
            string userId = Console.ReadLine();

            Console.Write("Password: ");
            string password = Console.ReadLine();

            string connect = String.Format(@"Server=127.0.0.1;Port=5432;
                User Id={0};Password={1};Database=practice;", userId, password);
            connection = new NpgsqlConnection(connect);

            command = com;
            data = commandToData(command);
        }

        private NpgsqlDataReader commandToData(string command)
        {
            NpgsqlCommand query = new NpgsqlCommand(command);
            return query.ExecuteReader();
        }



        public Table Select(string query)
        {
            string result = String.Format("select {0} from {1}",
                query, command);
            return new Table(result, connection);
        }
    }

    public class Test
    {
        public static void Main()
        {
            Console.Write("User Id: ");
            string userId = Console.ReadLine();

            Console.Write("Password: ");
            string password = Console.ReadLine();

            string command = String.Format(@"Server=127.0.0.1;Port=5432;
                User Id={0};Password={1};Database=practice;", userId, password);
            NpgsqlConnection connect = new NpgsqlConnection(command);
            connect.Open();
            connect.Close();
        }
    }
}
