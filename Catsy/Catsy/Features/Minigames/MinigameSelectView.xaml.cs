namespace Catsy.Features.Minigames;

public partial class MinigameSelectView : ContentView
{
	public MinigameSelectView(MinigameSelectViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}