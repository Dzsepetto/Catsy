namespace Catsy
{
    public partial class App : Application
    {
        private readonly MainLayoutPage _mainLayoutPage;

        public App(MainLayoutPage mainLayoutPage)
        {
            InitializeComponent();
            _mainLayoutPage = mainLayoutPage;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(_mainLayoutPage);

#if WINDOWS
        window.Width = 420;
        window.Height = 900;

        window.MinimumWidth = 420;
        window.MinimumHeight = 900;

        window.MaximumWidth = 504;
        window.MaximumHeight = 1080;
#endif

            return window;
        }
    }

}