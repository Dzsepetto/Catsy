using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace Catsy;

public sealed class HomeViewModel : BindableObject
{
    public ObservableCollection<SceneModel> Scenes { get; } = new();

    private int _selectedIndex;
    public int SelectedIndex
    {
        get => _selectedIndex;
        set { _selectedIndex = value; OnPropertyChanged(); }
    }

    public ICommand NextCommand { get; }
    public ICommand PrevCommand { get; }

    public HomeViewModel()
    {
        // sample scenes - replace with API data later
        Scenes.Add(new SceneModel { Title = "Park", Background = "Home/bg_park.png", Cat = "Home/cat_default.png", Building = "Home/building_small.png" });
        Scenes.Add(new SceneModel { Title = "City", Background = "Home/bg_city.png", Cat = "Home/cat_purple.png", Building = "Home/building_tall.png" });
        Scenes.Add(new SceneModel { Title = "Castle", Background = "Home/bg_castle.png", Cat = "Home/cat_gold.png", Building = "Home/building_castle.png" });

        NextCommand = new Command(() => SelectedIndex = Math.Min(Scenes.Count - 1, SelectedIndex + 1));
        PrevCommand = new Command(() => SelectedIndex = Math.Max(0, SelectedIndex - 1));
    }
}