using System;
using System.Collections.Generic;

namespace EventPlanner
{
    class Program
    {
        static Dictionary<Event, List<Person>> events = new Dictionary<Event, List<Person>>();

        static void Main(string[] args)
        {
            /*AddOrEditEvent();
            EventList();
            EventDeleteOption();
            EventList();*/
            MainMenu();


        }

        static void ColorText(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        static (bool doesKeep, int number) ValidNumber(string message)
        {
            var input = WriteRead(message).Trim();
            if (input == "") return (false, -1);
            int number;
            var success = int.TryParse(input, out number);
            if (success) return (true, number);
            else {
                ColorText("Unesite broj!\n", ConsoleColor.Yellow);
                return ValidNumber(message);
            }

        }

        static string WriteRead(string message, bool isSubmenu = false)
        {
            Console.WriteLine(message);
            var output = Console.ReadLine().Trim();
            if (!isSubmenu && output == "")
            { 
                Console.Clear();
                MainMenu();
            }
            else if (isSubmenu && output == "")
            {
                Console.Clear();
                SubMenu();
            }
            return output;
        }

        static void SuccessMessage(string message)
        {
            ColorText(message, ConsoleColor.Green);
            ColorText("Pritisnite enter za povratak", ConsoleColor.Gray);
            Console.ReadLine();
            Console.Clear();
            MainMenu();
        }


        static (bool doesKeep, string name) FindNameOrQuit(string message, bool needsUnique, bool isSubmenu=false)
        {
            var name = WriteRead(message, isSubmenu).Trim();
            var isUniqueName = true;
            if (name == "") return (false, "");
            foreach (var eventIterator in events.Keys)
            {
                if (eventIterator.Name.ToLower() == name.ToLower()) isUniqueName = false;
            }
            if (needsUnique == isUniqueName) return (true, name);
            var alert = needsUnique && !isUniqueName ? "Ime već postoji!" : "Taj event ne postoji!";
            ColorText(alert, ConsoleColor.Yellow);
            return FindNameOrQuit(message, needsUnique);
        }
                

        static void EventList()
        {
            Console.WriteLine("Popis evenata:");
            foreach (var eventPrint in events.Keys) Console.WriteLine(eventPrint.Name);
        }

        static void EventDetail()
        {
            var nameInput = FindNameOrQuit("Unesite ime eventa čije detalje želite; enter za return:", false, true);
            if (!nameInput.doesKeep) return;
            var eventToShow = ReturnEventCopy(nameInput.name).Key;
            Console.WriteLine($"{eventToShow.Name} --  {eventToShow.EventType} -- {eventToShow.StartTime} -- {eventToShow.EndTime} -- {events[eventToShow].Count}");
            EventDetail();
        }

        static void PersonList()
        {
            var nameInput = FindNameOrQuit("Unesite ime eventa čije članove želite; enter za return:", false, true);
            if (!nameInput.doesKeep) return;
            var personsToShow = ReturnEventCopy(nameInput.name).Value;
            Console.WriteLine("Lista osoba:");
            for (int i = 0; i < personsToShow.Count; ++i)
            {
                Console.WriteLine($"{i}. {personsToShow[i].FirstName} -- {personsToShow[i].LastName} -- {personsToShow[i].Phone}");
            }
     
            PersonList();
        }

        static void AllDetail()
        {
            var nameInput = FindNameOrQuit("Unesite ime eventa čije detalje želite; enter za return:", false, true);
            if (!nameInput.doesKeep) return;
            var eventToShow = ReturnEventCopy(nameInput.name);
            Console.WriteLine($"{eventToShow.Key.Name} --  {eventToShow.Key.EventType} -- {eventToShow.Key.StartTime} -- {eventToShow.Key.EndTime} -- {eventToShow.Value.Count}");
            Console.WriteLine("Lista osoba:");
            for (int i = 0; i < eventToShow.Value.Count; ++i)
            {
                Console.WriteLine($"{i}. {eventToShow.Value[i].FirstName} -- {eventToShow.Value[i].LastName} -- {eventToShow.Value[i].Phone}");
            }
        }

        static void MainMenu()
        {
            var optionsString = @"1. Dodajte event
2. Obrišite event
3. Edit event
4. Dodajte osobu na event
5. Uklonite osobu s eventa
6. Ispišite detalje eventa
7. Ugasite aplikaciju";
            var options = new List<Action>() { () => AddOrEditEvent(), EventDeleteOption, EventEdit, AddPerson, RemovePerson, SubMenu, ()=>ColorText("Gašenje...",ConsoleColor.Blue)};
            var optionInput = ValidNumber(optionsString);
            if (optionInput.number > 7 || optionInput.number < 1)
            {
                ColorText("Unos izvan granica!\n", ConsoleColor.Yellow);
                Console.Clear();
                MainMenu();
                return;
            }
            Console.Clear();
            options[optionInput.number-1]();
        }


        static bool AddOrEditEvent(bool isEdit = false, List<Person> persons = null)
        {
            if (!isEdit) ColorText("NOVI EVENT; enter za povratak", ConsoleColor.DarkMagenta);
            var nameInput = FindNameOrQuit("Unesite novo ime:",true);
            if (!nameInput.doesKeep) return false;
            TypeEvent eventType;
            while (true)
            {
                var eventTypeInput = WriteRead("Tip eventa:");
                if (eventTypeInput == "") return false;
                if (!Enum.TryParse(typeof(TypeEvent), eventTypeInput, out object eventTypeOut))
                {
                    ColorText("Unos nije ispravan! \n", ConsoleColor.Yellow);
                    continue;
                }
                eventType = (TypeEvent) eventTypeOut;
                break;
            }
            DateTime start = new DateTime(), end = new DateTime();
            while (true)
            {
                var dateInput = WriteRead("Vrijeme početka i završetka eventa u obliku dd/mm/yyyy hh:mm:ss, odvojeni zarezom:");
                if (dateInput == "") return false;
                if (!(dateInput.Contains(",") && DateTime.TryParse(dateInput.Split(",")[0], out start) && DateTime.TryParse(dateInput.Split(",")[1], out end)))
                {
                    ColorText("Unos nije ispravnog formata!\n", ConsoleColor.Yellow);
                    continue;
                }
                if (start > end)
                {
                    ColorText("Događaj ne može završiti prije nego što je počeo!\n", ConsoleColor.Red);
                    continue;
                }
                Func<DateTime, DateTime, DateTime, bool> between = (intervalStart, intervalEnd, time) => (time >= intervalStart && time <= intervalEnd);
                bool isBusy = false;
                foreach (var eventKey in events)
                {
                    if (between(eventKey.Key.StartTime, eventKey.Key.EndTime, start)
                        || between(eventKey.Key.StartTime, eventKey.Key.EndTime, end)
                        || between(start, end, eventKey.Key.StartTime)
                        || between(start, end, eventKey.Key.EndTime))
                    {
                        ColorText("Dio intervala je već zauzet!", ConsoleColor.Yellow);
                        isBusy = true;
                    }
                }
                if (isBusy) continue;
                break;
            }
            var newEvent = new Event(nameInput.name, eventType, start, end);
            if (!isEdit) events.Add(newEvent, new List<Person>()); //to avoid null reference
            else events.Add(newEvent, persons);
            if (!isEdit) SuccessMessage("Event dodan!");
            return true;
        }


        static void EventDeleteOption()
        {
            ColorText("BRIŠETE EVENT; enter za povratak", ConsoleColor.Red);
            EventList();
            var nameInput = FindNameOrQuit("Unesite ime eventa kojeg želite obrisati:", false);
            if (!nameInput.doesKeep) return;
            else events.Remove(ReturnEventCopy(nameInput.name).Key);
            ColorText("Event obrisan.", ConsoleColor.Green);
        }

        static KeyValuePair<Event, List<Person>> ReturnEventCopy (string name)
        {
            foreach (var eventToCopy in events)
            {
                if (name.ToLower() == eventToCopy.Key.Name.ToLower())
                {
                    return eventToCopy;
                }
            }
            return new KeyValuePair<Event, List<Person>>(); //just that compiler isn't angry
        }
        static void EventEdit()
        {
            ColorText("UREĐIVANJE EVENTA; enter za povratak:", ConsoleColor.DarkMagenta);
            EventList();
            var nameInput = FindNameOrQuit("Unesite ime eventa kojeg želite urediti:", false);
            if (!nameInput.doesKeep) return;
            var editEventCopy = ReturnEventCopy(nameInput.name);
            ColorText("Staro ime: " + editEventCopy.Key.Name, ConsoleColor.DarkGray);
            ColorText("Stari tip: " + editEventCopy.Key.EventType, ConsoleColor.DarkGray);
            ColorText("Stari početak: " + editEventCopy.Key.StartTime + " stari kraj: " + editEventCopy.Key.EndTime, ConsoleColor.DarkGray);
            events.Remove(editEventCopy.Key);
            if (!AddOrEditEvent(true, editEventCopy.Value)) //if we quit edit
            {
                events.Add(editEventCopy.Key, editEventCopy.Value);
            }
            else SuccessMessage("Even uređen!");

        }

        static (bool doesKeep, int identificator, bool isExisting, Person outPerson) FindIdentificatornOrQuit(string message, bool needsUnique, string eventName)
        {
            var personCopy = new Person();
            var identificatorInput = ValidNumber("Unesite OIB:");
            if (!identificatorInput.doesKeep) return (false, -1, false, personCopy);
            var inThisEvent = false;
            var inDictionary = false;

            foreach (var eventCheck in events.Keys)
            {
                foreach (var person in events[eventCheck])
                {
                    if (person.Identificator == identificatorInput.number)
                    {
                        if (eventCheck.Name == eventName) inThisEvent = true;
                        else inDictionary = true;
                        personCopy = person;
                        break;
                    }

                }
            }

            if (needsUnique && inThisEvent)
            {
                ColorText("Osoba ovog OIB-a već je u eventu.", ConsoleColor.Yellow);
                return FindIdentificatornOrQuit(message, needsUnique, eventName);
            }
            else if (!needsUnique && inThisEvent)
            {
                ColorText("Osoba ovog OIB-a nije u eventu.", ConsoleColor.Yellow);
                return FindIdentificatornOrQuit(message, needsUnique, eventName);
            }
            else return (true, identificatorInput.number, inDictionary, personCopy);

        }
        static void AddPerson()
        {
            ColorText("DODAVANJE OSOBA NA EVENT; esc za povratak", ConsoleColor.Cyan);
            var eventNameInput = FindNameOrQuit("Unesite ime eventa kojem želite dodati osobu:", false);
            if (!eventNameInput.doesKeep) return;
            var identificationInput = FindIdentificatornOrQuit("Unesite OIB osobe:", true, eventNameInput.name);
            if (!identificationInput.doesKeep) return;
            if (identificationInput.isExisting) //if person with same identification exist in dictionary
            {
                events[ReturnEventCopy(eventNameInput.name).Key].Add(identificationInput.outPerson);
                ColorText($"Osoba {identificationInput.outPerson.FirstName} dodana.", ConsoleColor.Green);
                AddPerson();
                return;
            }
            var personFirstName = WriteRead("Unesite ime osobe:");
            if (personFirstName == "") return;
            var personLastName = WriteRead("Unesite prezime osobe:");
            if (personLastName == "") return;
            var phoneInput = ValidNumber("Unesite broj telefona osobe:");
            if (!phoneInput.doesKeep) return;
            var newPerson = new Person(personFirstName, personLastName, identificationInput.identificator, phoneInput.number);
            events[ReturnEventCopy(eventNameInput.name).Key].Add(newPerson);
            SuccessMessage("Osoba dodana!");
        }

        static void RemovePerson()
        {
            ColorText("BRISANJE OSOBA S EVENTA; esc za povratak", ConsoleColor.Red);
            var eventNameInput = FindNameOrQuit("Unesite ime eventa kojem želite dodati osobu:", false);
            if (!eventNameInput.doesKeep) return;
            var identificationInput = FindIdentificatornOrQuit("Unesite OIB osobe koju želite obrisati:", false, eventNameInput.name);
            if (!identificationInput.doesKeep) return;
            events[ReturnEventCopy(eventNameInput.name).Key].Remove(identificationInput.outPerson);
            SuccessMessage("Osoba izbrisana!");
            RemovePerson();

        }

        static void SubMenu()
        {
            var optionsString = @"
Submenu:
1. Ispiši detalje eventa
2. Ispiši sve sudionike
3. Ispiši sve detalje
4. Povratak
            ";
            var options = new List<Action>() { EventDetail, PersonList, AllDetail, MainMenu };
            var optionInput = ValidNumber(optionsString);
            if (!optionInput.doesKeep) return;
            if (optionInput.number > 4 || optionInput.number < 1)
            {
                ColorText("Unos izvan granica!", ConsoleColor.Yellow);
                SubMenu();
                return;
            }
            options[optionInput.number]();
        }
    }
}
