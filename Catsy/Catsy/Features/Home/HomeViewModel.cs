using System.Collections.ObjectModel;

public sealed class HomeViewModel : BindableObject
{
    public ObservableCollection<string> Pages { get; } = new()
    {
        "Page 1",
        "Page 2",
        "Page 3",
        "Page 4"
    };

    private int _selectedIndex;
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
                OnPropertyChanged(nameof(CanSwipeLeft));
                OnPropertyChanged(nameof(CanSwipeRight));
            }
        }
    }

    public bool CanSwipeLeft => SelectedIndex > 0;
    public bool CanSwipeRight => SelectedIndex < Pages.Count - 1;
}
