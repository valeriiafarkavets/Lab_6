using System;
using System.Collections.Generic;
using System.Linq;
using Lab_6.Events;
namespace Lab_6.Models{
    public class Student{
        private readonly List<int> _marks = new();
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public IReadOnlyList<int> Marks => _marks.AsReadOnly();
        public double AverageMark =>
            _marks.Count == 0 ? 0 : _marks.Average();
        public bool HasMarks => _marks.Count > 0;
        public event EventHandler<MarksChangedEventArgs>? MarksChanged;
        public void AddMark(int mark){
            if (mark < 1 || mark > 5)
                throw new ArgumentOutOfRangeException(nameof(mark), "Оцінка повинна бути від 1 до 5.");
                double oldAverage = AverageMark;
                 _marks.Add(mark);
               OnMarksChanged(oldAverage, AverageMark);
        }
        public void UpdateMark(int index, int newMark){
            if (index < 0 || index >= _marks.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (newMark < 1 || newMark > 5)
                throw new ArgumentOutOfRangeException(nameof(newMark), "Оцінка повинна бути від 1 до 5.");
            if (_marks[index] == newMark)
                return;
            double oldAverage = AverageMark;
            _marks[index] = newMark;
            OnMarksChanged(oldAverage, AverageMark);
        }
        protected virtual void OnMarksChanged( double oldAverage, double newAverage){
            MarksChanged?.Invoke(this, new MarksChangedEventArgs( Id, oldAverage,newAverage));
        }
    }
}
