using System;
using System.Collections.Generic;
using System.Text;

namespace EventPlanner
{
    public enum TypeEvent
    {
        Coffee, Lecture, Concert, StudySession
    }
    public class Event
    {
        public Event() { }
        public Event(string name, TypeEvent eventType, DateTime startTime, DateTime endTime)
        => (Name, EventType, StartTime, EndTime) = (name, eventType, startTime, endTime);
        public string Name { get; set; }
        public TypeEvent EventType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public void ShowDetail() 
        {
            Console.WriteLine($"{Name} --  {EventType} -- {StartTime} -- {EndTime} -- {Program.events[this].Count}");
        }

        public void ShowPersonList() 
        {
            Console.WriteLine("Lista osoba:");
            for (var i = 0; i < Program.events[this].Count; ++i)
            {
                Console.Write(i+1 + ". "); 
                Program.events[this][i].ShowPerson();
            }

        }
    }
}
