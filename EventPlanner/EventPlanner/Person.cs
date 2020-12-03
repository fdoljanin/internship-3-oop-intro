using System;
using System.Collections.Generic;
using System.Text;

namespace EventPlanner
{
    class Person
    {
        public Person() { }
        public Person(string firstName, string lastName, int identificator, int phone)
        => (FirstName, LastName, Identificator, Phone) = (firstName, lastName, identificator, phone);
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Identificator { get; set; } //name for OIB
        public int Phone { get; set; }
    }
}
