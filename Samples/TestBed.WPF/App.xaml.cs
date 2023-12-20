using System.Windows;
using LanguageExt.Effects;

namespace TestBed.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly MinRT Runtime = new ();
    }
}
