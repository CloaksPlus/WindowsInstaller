using System.Windows;
using System.Windows.Media;

namespace Cloaks
{
    public partial class App : Application
    {
        static App()
        {
            TextOptions.TextFormattingModeProperty.OverrideMetadata(typeof(Window),
                new FrameworkPropertyMetadata(TextFormattingMode.Display, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));
        }
    }
}
