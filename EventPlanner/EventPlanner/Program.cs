using System;
using System.Collections.Generic;

namespace EventPlanner
{
    public class Program
    {
        public static class Atendees //default persons
        {
            public static Person Ivo = new Person("Ivo", "Puljek", 465, 385154);
            public static Person Ana = new Person("Ana", "Sataraš", 293, 385123);
            public static Person Caroline = new Person("Caroline", "Vision", 554, 1118);
            public static Person Hussein = new Person("Hussein", "Care", 745, 1224);
            public static Person Arna = new Person("Arna", "Butira", 661, 1993);
            public static Person Tonko = new Person("Tonko", "Milan", 191, 385665);
        }


        public static Dictionary<Event, List<Person>> events = new Dictionary<Event, List<Person>>() //default events
        {
            {
                new Event("Eurovision", TypeEvent.Concert, DateTime.Parse("18/5/2021"), DateTime.Parse("22/5/2021")), new List<Person>(){Atendees.Ivo, Atendees.Arna }
            },
            {
                new Event("School", TypeEvent.StudySession, DateTime.Parse("7/9/2020"), DateTime.Parse("23/12/2020")), new List<Person>(){ Atendees.Ana, Atendees.Hussein, Atendees.Tonko}
            },
            {
                new Event("Programming class", TypeEvent.Lecture, DateTime.Parse("7/2/2021 17:00"), DateTime.Parse("7/2/2021 20:00")), new List<Person>(){Atendees.Ivo, Atendees.Tonko }
            },
            {
                new Event("Hangout", TypeEvent.Coffee, DateTime.Parse("6/2/2021 16:45"), DateTime.Parse("6/2/2021 22:12")), new List<Person>(){Atendees.Ana, Atendees.Caroline}
            }
        };

        public static void Empty() { //used as empty base function

        }

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            MainMenu();
        }

        

        static (bool doesKeep, string name) FindNameOrQuit(string message, bool needsUnique, Action baseFunction) 
        {
            var nameInput = InteractionHelper.WriteRead(message, baseFunction);
            var isUniqueName = true;
            if (!nameInput.doesKeep) return (false, "");
            foreach (var eventIterator in events.Keys)
            {
                if (eventIterator.Name.ToLower() == nameInput.userInput.ToLower()) isUniqueName = false;
            }
            if (needsUnique == isUniqueName) return (true, nameInput.userInput);
            var alert = needsUnique ? "Ime već postoji!" : "Taj event ne postoji!";
            InteractionHelper.ColorText(alert, ConsoleColor.Yellow);
            return FindNameOrQuit(message, needsUnique, baseFunction);
        }
                

        static void EventList()
        {
            Console.WriteLine("Popis evenata:");
            foreach (var eventPrint in events.Keys) Console.WriteLine(eventPrint.Name);
        }

        static KeyValuePair<Event, List<Person>> ReturnEventCopy(string name)
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


        static void MainMenu()
        {
            var optionsString = @"1. Dodajte event
2. Obrišite event
3. Edit event
4. Dodajte osobu na event
5. Uklonite osobu s eventa
6. Ispišite detalje eventa
7. Ugasite aplikaciju";
            var options = new List<Action>() { () => AddOrEditEvent(), EventDeleteOption, EventEdit, AddPerson, RemovePerson, SubMenu, ()=>InteractionHelper.ColorText("Gašenje...",ConsoleColor.Blue)};
            Console.WriteLine(optionsString);
            var optionInput = InteractionHelper.GetNumberOrQuit("Unesite opciju:", MainMenu);
            if (optionInput.doesKeep == false) return;
            Console.Clear();
            if (optionInput.number > 7 || optionInput.number < 1)
            {
                InteractionHelper.ColorText("Unos izvan granica!", ConsoleColor.Yellow);
                MainMenu();
                return;
            }
            options[optionInput.number-1]();
        }


        static bool AddOrEditEvent(bool isEdit = false, List<Person> persons = null)
        {
            if (!isEdit) InteractionHelper.ColorText("NOVI EVENT; enter za povratak", ConsoleColor.DarkMagenta);
            Action  baseFunction = MainMenu;
            if (isEdit) baseFunction = Empty;
            var nameInput = FindNameOrQuit("Unesite novo ime:", true, baseFunction);
            if (!nameInput.doesKeep) return false;
            TypeEvent eventType;
            while (true)
            {
                var eventTypeInput = InteractionHelper.WriteRead("Tip eventa (Coffee, Lecture, Concert, StudySession):", baseFunction);
                if (!eventTypeInput.doesKeep) return false;
                if (!Enum.TryParse(typeof(TypeEvent), eventTypeInput.userInput, out object eventTypeOut))
                {
                    InteractionHelper.ColorText("Unos nije ispravan!", ConsoleColor.Yellow);
                    continue;
                }
                eventType = (TypeEvent) eventTypeOut;
                break;
            }
            DateTime start = new DateTime(), end = new DateTime();
            while (true)
            {
                var dateInput = InteractionHelper.WriteRead("Vrijeme početka i završetka eventa u obliku dd/mm/yyyy hh:mm, odvojeni zarezom:", baseFunction);
                if (!dateInput.doesKeep) return false;
                if (!(dateInput.userInput.Contains(",") && DateTime.TryParse(dateInput.userInput.Split(",")[0], out start) && DateTime.TryParse(dateInput.userInput.Split(",")[1], out end)))
                {
                    InteractionHelper.ColorText("Unos nije ispravnog formata!", ConsoleColor.Yellow);
                    continue;
                }
                if (start > end)
                {
                    InteractionHelper.ColorText("Događaj ne može završiti prije nego što je počeo!", ConsoleColor.Yellow);
                    continue;
                }
                Func<DateTime, DateTime, DateTime, bool> between = (intervalStart, intervalEnd, time) => (time >= intervalStart && time <= intervalEnd);
                var isBusy = false;
                foreach (var eventKey in events)
                {
                    if (between(eventKey.Key.StartTime, eventKey.Key.EndTime, start)
                        || between(eventKey.Key.StartTime, eventKey.Key.EndTime, end)
                        || between(start, end, eventKey.Key.StartTime)
                        || between(start, end, eventKey.Key.EndTime))
                    {
                        InteractionHelper.ColorText("Dio intervala je već zauzet!", ConsoleColor.Yellow);
                        isBusy = true;
                        break;
                    }
                }
                if (isBusy) continue;
                break;
            }
            var newEvent = new Event(nameInput.name, eventType, start, end);
            if (!isEdit) events.Add(newEvent, new List<Person>()); //to avoid null reference
            else events.Add(newEvent, persons);
            if (!isEdit) InteractionHelper.SuccessMessage("Event dodan!", MainMenu);
            return true;
        }


        static void EventDeleteOption() 
        {
            InteractionHelper.ColorText("BRIŠETE EVENT; enter za povratak", ConsoleColor.Red);
            EventList();
            var nameInput = FindNameOrQuit("Unesite ime eventa kojeg želite obrisati:", false, MainMenu);
            if (!nameInput.doesKeep) return;
            if (InteractionHelper.ConfirmChange("Želite li obrisati event?"))
            {
                events.Remove(ReturnEventCopy(nameInput.name).Key);
                InteractionHelper.SuccessMessage("Event obrisan!", MainMenu);
            }
            else InteractionHelper.SuccessMessage("Akcija poništena.", MainMenu);
        }

        static void EventEdit() 
        {
            InteractionHelper.ColorText("UREĐIVANJE EVENTA; enter za povratak:", ConsoleColor.DarkMagenta);
            EventList();
            var nameInput = FindNameOrQuit("Unesite ime eventa kojeg želite urediti:", false, MainMenu);
            if (!nameInput.doesKeep) return;
            var editEventCopy = ReturnEventCopy(nameInput.name);
            InteractionHelper.ColorText("Staro ime: " + editEventCopy.Key.Name, ConsoleColor.DarkGray);
            InteractionHelper.ColorText("Stari tip: " + editEventCopy.Key.EventType, ConsoleColor.DarkGray);
            InteractionHelper.ColorText("Stari početak: " + editEventCopy.Key.StartTime + " stari kraj: " + editEventCopy.Key.EndTime, ConsoleColor.DarkGray);
            events.Remove(editEventCopy.Key);
            if (!AddOrEditEvent(true, editEventCopy.Value)) 
            {
                events.Add(editEventCopy.Key, editEventCopy.Value);
                MainMenu();
            }
            else InteractionHelper.SuccessMessage("Event uređen!", MainMenu);

        }


        static (bool doesKeep, int identificator, bool isExisting, Person outPerson) FindIdentificatorOrQuit(string message, bool needsUnique, string eventName) 
        {
            var personCopy = new Person();
            var identificatorInput = InteractionHelper.GetNumberOrQuit("Unesite OIB:", MainMenu);
            if (!identificatorInput.doesKeep) return (false, -1, false, personCopy);
            var inThisEvent = false;
            var inDictionary = false;
            foreach (var eventCheck in events.Keys)
            {
                foreach (var person in events[eventCheck])
                {
                    if (person.Identificator == identificatorInput.number)
                    {
                        if (eventCheck.Name.ToLower() == eventName.ToLower()) inThisEvent = true;
                        else inDictionary = true;
                        personCopy = person;
                        break;
                    }

                }
            }


            if (needsUnique && inThisEvent)
            {
                InteractionHelper.ColorText("Osoba ovog OIB-a već je u eventu.", ConsoleColor.Yellow);
                return FindIdentificatorOrQuit(message, needsUnique, eventName);
            }
            else if (!needsUnique && !inThisEvent)
            {
                InteractionHelper.ColorText("Osoba ovog OIB-a nije u eventu.", ConsoleColor.Yellow);
                return FindIdentificatorOrQuit(message, needsUnique, eventName);
            }

            else return (true, identificatorInput.number, inDictionary, personCopy);

        }


        static void AddPerson()
        {
            InteractionHelper.ColorText("DODAVANJE OSOBA NA EVENT; esc za povratak", ConsoleColor.Cyan);
            EventList();
            var eventNameInput = FindNameOrQuit("Unesite ime eventa kojem želite dodati osobu:", false, MainMenu);
            if (!eventNameInput.doesKeep) return;
            var identificationInput = FindIdentificatorOrQuit("Unesite OIB osobe:", true, eventNameInput.name);
            if (!identificationInput.doesKeep) return;
            if (identificationInput.isExisting) //if person with same identification exist in dictionary
            {
                events[ReturnEventCopy(eventNameInput.name).Key].Add(identificationInput.outPerson);
                InteractionHelper.SuccessMessage($"Osoba {identificationInput.outPerson.FirstName} dodana!", MainMenu);
                return;
            }
            var personFirstName = InteractionHelper.WriteRead("Unesite ime osobe:", MainMenu);
            if (!personFirstName.doesKeep) return;
            var personLastName = InteractionHelper.WriteRead("Unesite prezime osobe:", MainMenu);
            if (!personLastName.doesKeep) return;
            var phoneInput = InteractionHelper.GetNumberOrQuit("Unesite broj telefona osobe, bez razmaka:", MainMenu);
            if (!phoneInput.doesKeep) return;
            var newPerson = new Person(personFirstName.userInput, personLastName.userInput, identificationInput.identificator, phoneInput.number);
            events[ReturnEventCopy(eventNameInput.name).Key].Add(newPerson);
            InteractionHelper.SuccessMessage("Osoba dodana!", MainMenu);
        }


        static void RemovePerson() 
        {
            InteractionHelper.ColorText("BRISANJE OSOBA S EVENTA; esc za povratak", ConsoleColor.Red);
            EventList();
            var eventNameInput = FindNameOrQuit("Unesite ime eventa kojem želite obrisati osobu:", false, MainMenu);
            if (!eventNameInput.doesKeep) return;
            var eventCopy = ReturnEventCopy(eventNameInput.name).Key;
            eventCopy.ShowPersonList();
            var identificationInput = FindIdentificatorOrQuit("Unesite OIB osobe koju želite obrisati:", false, eventNameInput.name);
            if (!identificationInput.doesKeep) return;

            if (InteractionHelper.ConfirmChange("Želite li uistinu obrisati osobu?"))
            {
                events[eventCopy].Remove(identificationInput.outPerson);
                InteractionHelper.SuccessMessage("Osoba izbrisana!", MainMenu);
            } else InteractionHelper.SuccessMessage("Akcija poništena.", MainMenu);

        }


        static void SubMenu()
        {
            var optionsString = @"Submenu:
1. Ispiši detalje eventa
2. Ispiši sve sudionike
3. Ispiši sve detalje
4. Povratak
            ";
            Console.WriteLine(optionsString);
            var optionInput = InteractionHelper.GetNumberOrQuit("Odaberite broj:", MainMenu);
            if (!optionInput.doesKeep) return;
            Console.Clear();
            if (optionInput.number > 4 || optionInput.number < 1)
            {
                InteractionHelper.ColorText("Unos izvan granica!", ConsoleColor.Yellow);
                SubMenu();
                return;
            }
            if (optionInput.number == 4)
            {
                MainMenu();
                return;
            }
            EventList();
            EventDetailOption(optionInput.number);
        }


        static void EventDetailOption(int optionChosen) 
        {
            var message = "";
            if (optionChosen == 1) message = "Unesite ime eventa čije detalje želite; enter za return:";
            else if (optionChosen == 2) message = "Unesite ime eventa čije članove želite; enter za return:";
            else if (optionChosen == 3) message = "Unesite ime eventa čije sve detalje želite; enter za return:";
            var nameInput = FindNameOrQuit(message, false, SubMenu);
            if (!nameInput.doesKeep) return;
            var eventToShow = ReturnEventCopy(nameInput.name);
            if (optionChosen == 1 || optionChosen == 3) eventToShow.Key.ShowDetail();
            if (optionChosen == 2 || optionChosen == 3) eventToShow.Key.ShowPersonList();
            EventDetailOption(optionChosen);
        }
    }
}
