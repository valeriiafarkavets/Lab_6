using System;
using System.Collections.ObjectModel;
using Lab_6.Events;
using Lab_6.Models;
namespace Lab_6.Services{
    public class Logger {
        public ObservableCollection<string> Entries { get; } = new();
        public void Subscribe(Student student){
            student.MarksChanged -= OnMarksChanged;
            student.MarksChanged += OnMarksChanged;
        }
        public void Unsubscribe(Student student){
            student.MarksChanged -= OnMarksChanged;
        }
        private void OnMarksChanged(object? sender, MarksChangedEventArgs e) {
            if (sender is not Student student)
                return;
            string message =
                $"[{e.EventTime:HH:mm:ss}] " +
                $"Студент: {student.Name} | " +
                $"Середній бал: {e.OldAverage:F2} → {e.NewAverage:F2}";

            Entries.Add(message);
        }
        public void Clear() {
            Entries.Clear();
        }
    }
}
