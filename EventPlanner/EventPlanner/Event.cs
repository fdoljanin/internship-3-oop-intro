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
        public string Name { get; set; }
        public TypeEvent EventType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
