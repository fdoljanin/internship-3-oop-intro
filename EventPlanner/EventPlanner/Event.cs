using System;
using System.Collections.Generic;
using System.Text;

namespace EventPlanner
{
    enum TypeEvent
    {
        Coffee, Lecture, Concert, StudySession
    }
    class Event
    {
        public Event() { }
        public Event(string name, TypeEvent eventType, DateTime startTime, DateTime endTime)
        => (Name, EventType, StartTime, EndTime) = (name, eventType, startTime, endTime);
        public string Name { get; set; }
        public TypeEvent EventType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
