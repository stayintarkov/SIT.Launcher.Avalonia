using Avalonia.Media;
using FluentAvalonia.UI.Windowing;
using SIT.Manager.ViewModels;

namespace SIT.Manager.Views;
public partial class CrashWindow : AppWindow
{
    public CrashWindow()
    {
        InitializeComponent();
        DataContext = new CrashWindowViewModel();
        TitleBar.BackgroundColor = new Color(0xFF, 0x00, 0x00, 0x00);
        TitleBar.ForegroundColor = new Color(0xFF, 0xFF, 0xFF, 0xFF);
        TitleBar.InactiveBackgroundColor = new Color(0xFF, 0x00, 0x00, 0x00);
        TitleBar.InactiveForegroundColor = new Color(0xFF, 0xFF, 0xFF, 0xFF);
        TitleBar.ButtonBackgroundColor = new Color(0xFF, 0x00, 0x00, 0x00);
        TitleBar.ButtonForegroundColor = new Color(0xFF, 0xFF, 0xFF, 0xFF);
        TitleBar.ButtonHoverBackgroundColor = new Color(0xFF, 0x11, 0x11, 0x11);
        TitleBar.ButtonHoverForegroundColor = new Color(0xFF, 0xFF, 0xFF, 0xFF);
        TitleBar.ButtonPressedBackgroundColor = new Color(0xFF, 0x21, 0x21, 0x21);
        TitleBar.ButtonPressedForegroundColor = new Color(0xFF, 0xFF, 0xFF, 0xFF);
        TitleBar.ButtonInactiveBackgroundColor = new Color(0xFF, 0x00, 0x00, 0x00);
        TitleBar.ButtonInactiveForegroundColor = new Color(0xFF, 0xFF, 0xFF, 0xFF);
    }
}
