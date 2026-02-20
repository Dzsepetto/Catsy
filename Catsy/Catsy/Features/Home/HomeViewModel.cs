using System.Collections.ObjectModel;

namespace Catsy;

public sealed class HomeViewModel : BindableObject
{
    public ObservableCollection<HomePageItem> Pages { get; } = new()
    {
        new HomePageItem { Title = "Welcome" },
        new HomePageItem { Title = "Catsy Shop" },
        new HomePageItem { Title = "Mini Games" }
    };

    int _selectedIndex;
    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            if (_selectedIndex != value)
            {
                _selectedIndex = value;
                OnPropertyChanged();
            }
        }
    }
}

public class HomePageItem
{
    public string Title { get; set; } = string.Empty;
}
