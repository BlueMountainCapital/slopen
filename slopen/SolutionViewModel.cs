using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SlnOpener
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        //basic ViewModelBase
        internal void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class SolutionViewModel : ViewModelBase
    {
        public SolutionViewModel(string path)
        {
            _path = path;
            _name = System.IO.Path.GetFileName(_path);
            _searchableName = _name.ToLower();
        }

        private readonly string _path;
        private readonly string _name;
        private readonly string _searchableName;

        public string Name { get { return _name; } }
        public string SearchableName { get { return _searchableName; } }
        public string Path { get { return _path; } }
    }

    internal class GenericCommand : ICommand
    {
        private readonly Action<object> _a;

        public GenericCommand(Action<object> a)
        {
            _a = a;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _a(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }

    public class SolutionsViewModel : ViewModelBase
    {
        private const string InitialPath = @"c:\src";

        private readonly ObservableCollection<SolutionViewModel> _solutions;
        private readonly List<SolutionViewModel> _allSolutions;

        public SolutionsViewModel()
        {
            _solutions = new ObservableCollection<SolutionViewModel>();

            _allSolutions = Directory.GetFiles(InitialPath, "*.sln", SearchOption.AllDirectories)
                .Select(f => new SolutionViewModel(f))
                .OrderBy(s => s.Name)
                .ToList();

            foreach (var svm in _allSolutions)
                _solutions.Add(svm);

            SearchString = "[search]";

            _openCommand = new GenericCommand(param =>
            {
                if (Solutions.Count > 0)
                {
                    var path = Solutions[SelectedSolution].Path;
                    var result = MessageBox.Show("Opening " + path, "Boom", MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                        System.Diagnostics.Process.Start(path);
                }
            });

            _focusCommand = new GenericCommand(param =>
            {
                (param as UIElement).Focus();
            });

            _clearSearchCommand = new GenericCommand(param =>
            {
                SearchString = string.Empty;
            });
        }

        public ObservableCollection<SolutionViewModel> Solutions { get { return _solutions; } }

        private int _selectedSolution;
        public int SelectedSolution
        {
            get { return _selectedSolution; }
            set { RaisePropertyChanged("SelectedSolution"); _selectedSolution = value; }
        }

        private string _searchString;

        public string SearchString
        {
            get { return _searchString; }
            set
            {
                RaisePropertyChanged("SearchString");
                _searchString = value;
                FilterSolutions();
            }
        }

        private void FilterSolutions()
        {
            var searchString = _searchString.ToLower();
            if (_searchString.StartsWith("[") && _searchString.EndsWith("]"))
                searchString = string.Empty;

            _solutions.Clear();
            foreach (var svm in _allSolutions.Where(s => s.SearchableName.Contains(searchString)))
                _solutions.Add(svm);

            SelectedSolution = 0;
        }

        private readonly ICommand _openCommand;
        public ICommand OpenCommand { get { return _openCommand; } }

        private readonly ICommand _focusCommand;
        public ICommand FocusCommand { get { return _focusCommand; } }

        private readonly ICommand _clearSearchCommand;
        public ICommand ClearSearchCommand { get { return _clearSearchCommand; } }
    }
}
