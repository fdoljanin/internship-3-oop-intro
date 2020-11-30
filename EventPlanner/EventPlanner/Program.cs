using System;
using System.Collections.Generic;

namespace EventPlanner
{
    class Program
    {
        static Dictionary<Event, List<Person>> events;

        static void Main(string[] args)
        {
        }

        static void ColorText(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        static (bool doesQuit, bool isInt, int number) ValidNumber()
        {
            var input = Console.ReadLine().Trim();
            if (input == "") return (false, false, -1);
            int number;
            var success = int.TryParse(input, out number);
            if (success) return (false, true, number);
            else return (false, false, -1);
        }

        static string WriteRead(string message)
        {
            Console.WriteLine(message);
            return Console.ReadLine();
        }

        static void MainMenu()
        {
            var optionsMessage = @"1. Dodajte event
2. Obrišite event
3. Edit event
4. Dodajte osobu na event
5. Uklonite osobu s eventa
6. Ispišite detalje eventa
7. Ugasite aplikaciju";
            Console.WriteLine(optionsMessage);
        }

        static void AddEvent()
        {
            ColorText("NOVI EVENT; enter za povratak", ConsoleColor.DarkMagenta);
            var name = WriteRead("Ime eventa:");
            TypeEvent eventType;
            while (true)
            {
                var eventTypeInput = WriteRead("Vrsta eventa:");
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
                string dateInput = WriteRead("Vrijeme početka i završetka eventa u obliku dd/mm/yy hh:mm:ss, odvojeni zarezom:");
                if (!(dateInput.Contains(",") && DateTime.TryParse(dateInput.Split(",")[0], out start) && DateTime.TryParse(dateInput.Split(",")[1], out end)))
                {
                    ColorText("Unos nije ispravnog formata!\n", ConsoleColor.Yellow);
                }
                else if (start.Ticks > end.Ticks)
                {
                    ColorText("Događaj ne može završiti prije nego što je počeo!\n", ConsoleColor.Red);
                }
                else break;
            }
        }

    }
}
