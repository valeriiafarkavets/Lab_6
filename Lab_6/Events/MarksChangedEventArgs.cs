using System;
namespace Lab_6.Events{
    public class MarksChangedEventArgs : EventArgs{
        public Guid StudentId { get; }
        public double OldAverage { get; }
        public double NewAverage { get; }
        public DateTime EventTime { get; }
        public MarksChangedEventArgs( Guid studentId, double oldAverage, double newAverage) {
            StudentId = studentId;
            OldAverage = oldAverage;
            NewAverage = newAverage;
            EventTime = DateTime.Now;
        }
    }
}
