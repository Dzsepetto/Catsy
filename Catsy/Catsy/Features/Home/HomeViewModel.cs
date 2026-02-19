using Catsy;
using System.Collections.ObjectModel;

public sealed class HomeViewModel : BindableObject
{
    public ObservableCollection<SceneModel> Pages { get; } = new()
    {
        new() { Title="City", Background="city.png" },
        new() { Title="Forest", Background="forest.png" }
    };

    int _selectedIndex;
    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            int clamped = Math.Max(0, Math.Min(value, Pages.Count - 1));
            if (_selectedIndex != clamped)
            {
                _selectedIndex = clamped;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentScene));
            }
        }
    }

    public SceneModel CurrentScene => Pages[SelectedIndex];

    public bool CanLeft => SelectedIndex > 0;
    public bool CanRight => SelectedIndex < Pages.Count - 1;

    public void GoLeft() { if (CanLeft) SelectedIndex--; }
    public void GoRight() { if (CanRight) SelectedIndex++; }
}
