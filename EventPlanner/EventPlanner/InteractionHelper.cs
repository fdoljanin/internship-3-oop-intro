using System;
using System.Collections.Generic;
using System.Text;

namespace EventPlanner
{
    public static class InteractionHelper
    {
        public static (string userInput, bool doesKeep) WriteRead(string message, Action baseFunction)
        {
            Console.WriteLine(message);
            var output = Console.ReadLine().Trim();
            if (output == "")
            {
                Console.Clear();
                baseFunction();
                return (output, false);
            }
            return (output,true);
        }
        public static void ColorText(string message, ConsoleColor color) 
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static (bool doesKeep, int number) GetNumberOrQuit(string message, Action baseFunction) 
        {
            var input = WriteRead(message, baseFunction);
            if (!input.doesKeep) return (false, -1);
            int number;
            var success = int.TryParse(input.userInput, out number);
            if (success) return (true, number);
            else
            {
                ColorText("Unos nije broj!", ConsoleColor.Yellow);
                return GetNumberOrQuit(message, baseFunction);
            }

        }

        public static void SuccessMessage(string message, Action baseFunction) 
        {
            ColorText(message, ConsoleColor.Green);
            ColorText("Pritisnite enter za povratak...", ConsoleColor.Gray);
            Console.ReadLine();
            Console.Clear();
            baseFunction();
        }

        public static bool ConfirmChange(string message)
        {
            ColorText(message + " (da/ne)", ConsoleColor.Red);
            while (true)
            {
                string choice = Console.ReadLine().ToLower().Trim();
                if (choice == "da") return true;
                if (choice == "ne") return false;
                ColorText("Odgovor nije prepoznat, upišite ponovno. (da/ne)", ConsoleColor.Yellow);
            }
        }

    }
}
