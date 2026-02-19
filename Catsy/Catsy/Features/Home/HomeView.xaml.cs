namespace Catsy;

public partial class HomeView : ContentView
{
    HomeViewModel VM => (HomeViewModel)BindingContext;

    View? _current;
    int _lastIndex;
    double PageWidth =>
    Width > 0 ? Width :
    PagerHost.Width > 0 ? PagerHost.Width :
    0;

    public HomeView()
    {
        InitializeComponent();
        BindingContext = new HomeViewModel();

        ShowScene(animated: false);
        _lastIndex = VM.SelectedIndex;

        VM.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(HomeViewModel.SelectedIndex))
                ShowScene(animated: true);
        };

        PagerHost.GestureRecognizers.Add(new SwipeGestureRecognizer
        {
            Direction = SwipeDirection.Left,
            Command = new Command(() => VM.GoRight())
        });

        PagerHost.GestureRecognizers.Add(new SwipeGestureRecognizer
        {
            Direction = SwipeDirection.Right,
            Command = new Command(() => VM.GoLeft())
        });
    }

    async void ShowScene(bool animated)
    {
        if (PageWidth <= 0)
            return;

        int direction = VM.SelectedIndex > _lastIndex ? 1 : -1;
        _lastIndex = VM.SelectedIndex;

        var next = new SceneView
        {
            BindingContext = VM.CurrentScene,
            TranslationX = animated ? direction * PageWidth : 0
        };

        PagerHost.Children.Add(next);

        if (_current != null && animated)
        {
            await Task.WhenAll(
                _current.TranslateTo(-direction * PageWidth, 0, 250, Easing.CubicOut),
                next.TranslateTo(0, 0, 250, Easing.CubicOut)
            );

            PagerHost.Children.Remove(_current);
            _current.BindingContext = null;
        }

        _current = next;
    }
}
