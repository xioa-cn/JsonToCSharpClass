using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace JsonToCSharpClass.Views;

public partial class About : Window
{
    public About()
    {
        InitializeComponent();
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        // 打开默认浏览器到指定的 URL
        Process.Start(new ProcessStartInfo
        {
            FileName = e.Uri.AbsoluteUri,
            UseShellExecute = true
        });
        e.Handled = true;
    }
}