using System;
using System.Collections.Generic;
using Lab_6.Events;
using Lab_6.Models;
namespace Lab_6.Services{
    public class DeanOfficeNotifier {
        private readonly HashSet<Guid> _warnedStudents = new();
        public event EventHandler<string>? WarningCreated;
        public void Subscribe(Student student){
            student.MarksChanged -= OnMarksChanged;
            student.MarksChanged += OnMarksChanged;
        }
        public void Unsubscribe(Student student){
            student.MarksChanged -= OnMarksChanged;
        }
        private void OnMarksChanged( object? sender, MarksChangedEventArgs e) {
            if (sender is not Student student)
                return;
            if (!student.HasMarks)
                return;
            if (e.NewAverage > 3){
                _warnedStudents.Remove(e.StudentId);
                return;
            }
            if (e.NewAverage < 3 && _warnedStudents.Add(e.StudentId)){
                string message =
                    $"ПОПЕРЕДЖЕННЯ ДЕКАНАТУ: студент {student.Name} " +
                    $"має середній бал {e.NewAverage:F2}, що нижче 3.";
                WarningCreated?.Invoke(this, message);
            }
        }
    }
}
