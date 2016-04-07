using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
  public class Person
  {
    public long PersonID { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public DateTime DateCreated { get; set; }

    public static List<Person> GetPersonList()
    {
      return new List<Person>()
      {
        new Person() { PersonID = 1, DateCreated = DateTime.Now, Name = "", Surname = "The Builder" },
        new Person() { PersonID = 2, DateCreated = DateTime.Now, Name = "Rob", Surname = "The Robber" },
        new Person() { PersonID = 2, DateCreated = DateTime.Now, Name = "Gob", Surname = "The Gobbler" }
      };
    }
  }
}
