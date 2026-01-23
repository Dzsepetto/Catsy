namespace Catsy
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Shell.SetTabBarBackgroundColor(this, Colors.Black);
            Shell.SetTabBarForegroundColor(this, Colors.White);
            Shell.SetTabBarTitleColor(this, Colors.White);
            Shell.SetTabBarUnselectedColor(this, Colors.Gray);
        }
    }
}
