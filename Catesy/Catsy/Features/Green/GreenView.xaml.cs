using System.Diagnostics;

namespace Catsy;

public partial class GreenView : ContentView
{
    private bool _running;
    private double _x;            // current X position (px within playfield)
    private double _dir = 1;      // +1 right, -1 left

    // "x gyorsaság": px/sec
    private const double Speed = 280;

    // loop timing
    private DateTime _lastTick;

    public GreenView()
    {
        InitializeComponent();

        // induláskor középre rakjuk, ha már megvan a méret
        Playfield.SizeChanged += (_, __) =>
        {
            if (Playfield.Width > 0)
            {
                _x = 0;
                Ball.TranslationX = _x;
            }
        };
    }

    private void Start_Clicked(object sender, EventArgs e)
    {
        if (_running)
            return;

        _running = true;
        _lastTick = DateTime.UtcNow;

        // background loop
        _ = RunLoopAsync();
    }

    private void Stop_Clicked(object sender, EventArgs e)
    {
        _running = false;
    }

    private async Task RunLoopAsync()
    {
        // amíg nincs rendes méret, várunk
        while (_running && (Playfield.Width <= 0 || Ball.Width <= 0))
            await Task.Delay(16);

        while (_running)
        {
            var now = DateTime.UtcNow;
            var dt = (now - _lastTick).TotalSeconds; // seconds since last frame
            _lastTick = now;

            // mennyi a max X, hogy pont ne lógjon ki
            var maxX = Math.Max(0, Playfield.Width - Ball.Width);

            // pozíció frissítése
            _x += _dir * Speed * dt;

            // fal ütközés + irányváltás
            if (_x <= 0)
            {
                _x = 0;
                _dir = 1;
            }
            else if (_x >= maxX)
            {
                _x = maxX;
                _dir = -1;
            }

            // UI frissítés
            Ball.TranslationX = _x;

            // kb. 60 fps
            await Task.Delay(16);
        }
    }
}
