using ItsMyDoliprane.Repository.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace ItsMyDoliprane.Repository;

public class PersonRepository : AbstractRepository
{
    public PersonRepository(IConfiguration configuration) : base(configuration) { }

    public List<Person> GetPersons() {
        using SqliteConnection connection = CreateConnectionAndOpen();
        using SqliteCommand command = connection.CreateCommand();
        List<Person> persons = new List<Person>();
        command.CommandText = "SELECT PKEY, NAME FROM PERSON";
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
            persons.Add(new Person {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
        return persons;
    }
}
