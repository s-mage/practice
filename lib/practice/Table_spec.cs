using NSpec;
using System;
using Rooletochka;
using Npgsql;
using System.Data;

class describe_table : nspec
{
    Table users;
    string name;
    string password;

    void Initialize()
    {
        string connect = "Server=127.0.0.1;Port=5432;User Id=s;Database=practice;";
        var connection = new NpgsqlConnection(connect);
        if(connection.State == ConnectionState.Closed) { connection.Open(); }
        users = new Table("users", connection);
    }

    void BeforeInsert()
    {
        users.Insert("'mala', 'fia'", "name, password");
        while(users.data.Read()) {
            name = users.data.GetString(1);
            password = users.data.GetString(2);
        }
    }

    void before_each()
    {
        Initialize();
    }

    void insert_should_work()
    {
        before = () => BeforeInsert();
        it["When I insert something, it should be on last position"] = () =>
            name.should_be("mala");
        it["When I insert something, it should be on last position"] = () =>
            password.should_be("fia");
    }
}
