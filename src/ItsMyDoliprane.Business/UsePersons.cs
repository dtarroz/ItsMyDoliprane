using ItsMyDoliprane.Repository;
using ItsMyDoliprane.Repository.Models;

namespace ItsMyDoliprane.Business;

public class UsePersons
{
    private readonly PersonRepository _personRepository;

    public UsePersons(PersonRepository personRepository) {
        _personRepository = personRepository;
    }

    public List<Person> GetPersons() {
        return _personRepository.GetPersons();
    }
}
