using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Lab_6.Models;
using Lab_6.Services;
namespace Lab_6.ViewModels{
    public class MainViewModel : INotifyPropertyChanged{
        private readonly DeanOfficeNotifier _deanOffice = new();
        private readonly Logger _logger = new();
        private Student? _selectedStudent;
        private string _studentName = "";
        private string _markInput = "5";
        private string _markIndexInput = "1";
        private string _statusMessage = "Програма готова до роботи";
        private bool _deanSubscribed = true;
        private bool _loggerSubscribed = true;
        public ObservableCollection<Student> Students { get; } = new();
        public ObservableCollection<string> LogEntries => _logger.Entries;
        public ObservableCollection<string> Warnings { get; } = new();
        public Student? SelectedStudent{
            get => _selectedStudent;
            set{
                if (SetProperty(ref _selectedStudent, value)){
                    OnPropertyChanged(nameof(SelectedStudentInfo));
                }
            }
        }
        public string SelectedStudentInfo =>
            SelectedStudent == null
                ? "Студента не вибрано"
                : $"{SelectedStudent.Name} | Оцінки: " +
                  $"{string.Join(", ", SelectedStudent.Marks)} | " +
                  $"Середній бал: {SelectedStudent.AverageMark:F2}";
        public string StudentName{
            get => _studentName;
            set => SetProperty(ref _studentName, value);
        }
        public string MarkInput{
            get => _markInput;
            set => SetProperty(ref _markInput, value);
        }
        public string MarkIndexInput{
            get => _markIndexInput;
            set => SetProperty(ref _markIndexInput, value);
        }
        public string StatusMessage{
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }
        public bool DeanSubscribed{
            get => _deanSubscribed;
            set{
                if (!SetProperty(ref _deanSubscribed, value))
                    return;
                foreach (Student student in Students){
                    if (value)
                        _deanOffice.Subscribe(student);
                    else
                        _deanOffice.Unsubscribe(student);
                }
                StatusMessage = value
                    ? "Деканат підписано на події"
                    : "Деканат відписано від подій";
            }
        }
        public bool LoggerSubscribed{
            get => _loggerSubscribed;
            set{
                if (!SetProperty(ref _loggerSubscribed, value))
                    return;
                foreach (Student student in Students){
                    if (value)
                        _logger.Subscribe(student);
                    else
                        _logger.Unsubscribe(student);
                }
                StatusMessage = value
                    ? "Журнал підписано на події"
                    : "Журнал відписано від подій";
            }
        }
        public ICommand AddStudentCommand { get; }
        public ICommand AddMarkCommand { get; }
        public ICommand UpdateMarkCommand { get; }
        public ICommand ClearLogsCommand { get; }
        public ICommand ClearWarningsCommand { get; }
        public event PropertyChangedEventHandler? PropertyChanged;
        public MainViewModel(){
            _deanOffice.WarningCreated += OnWarningCreated;
            AddStudentCommand = new ActionCommand(AddStudent);
            AddMarkCommand = new ActionCommand(AddMark);
            UpdateMarkCommand = new ActionCommand(UpdateMark);
            ClearLogsCommand = new ActionCommand(() => _logger.Clear());
            ClearWarningsCommand = new ActionCommand(() => Warnings.Clear());
        }
        private void OnWarningCreated(object? sender, string message){
            Warnings.Add(message);
            StatusMessage = message;
        }
        private void AddStudent(){
            string name = StudentName.Trim();
            if (string.IsNullOrWhiteSpace(name)){
                StatusMessage = "Введіть ім'я студента";
                return;
            }
            var student = new Student {
                Name = name
            };
            if (DeanSubscribed)
                _deanOffice.Subscribe(student);

            if (LoggerSubscribed)
                _logger.Subscribe(student);

            Students.Add(student);
            SelectedStudent = student;
            StudentName = "";

            StatusMessage = $"Додано студента: {name}";
        }
        private void AddMark(){
            if (SelectedStudent == null) {
                StatusMessage = "Спочатку виберіть студента";
                return;
            }
            if (!int.TryParse(MarkInput, out int mark) ||
                mark < 1 || mark > 5){
                StatusMessage = "Оцінка повинна бути від 1 до 5";
                return;
            }
            SelectedStudent.AddMark(mark);
            OnPropertyChanged(nameof(SelectedStudentInfo));
            if (SelectedStudent.AverageMark >= 3)
                StatusMessage = $"Оцінку {mark} додано";
        }
        private void UpdateMark(){
            if (SelectedStudent == null){
                StatusMessage = "Спочатку виберіть студента";
                return;
            }
            if (!int.TryParse(MarkIndexInput, out int index) ||
                index < 1 || index > SelectedStudent.Marks.Count) {
                StatusMessage = "Введіть правильний номер оцінки";
                return;
            }
            if (!int.TryParse(MarkInput, out int mark) ||
                mark < 1 || mark > 5){
                StatusMessage = "Оцінка повинна бути від 1 до 5";
                return;
            }
            SelectedStudent.UpdateMark(index - 1, mark);
            OnPropertyChanged(nameof(SelectedStudentInfo));
            if (SelectedStudent.AverageMark >= 3)
                StatusMessage = $"Оцінку №{index} змінено на {mark}";
        }
        private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null){
            if (Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        private void OnPropertyChanged(string? propertyName){
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs(propertyName));
        }
        private class ActionCommand : ICommand{
            private readonly Action _execute;
            public ActionCommand(Action execute){
                _execute = execute;
            }
            public bool CanExecute(object? parameter) => true;
            public void Execute(object? parameter) => _execute();
            public event EventHandler? CanExecuteChanged{
                add { }
                remove { }
            }
        }
    }
}
