using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CodeCoverageAnalyzer.ViewModel
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new(name));
    }

}