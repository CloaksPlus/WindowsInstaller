using System.Threading.Tasks;
using System.Windows;

namespace Cloaks
{
    public partial class CloaksLoad : Window
    {
        readonly MainWindow Main = new MainWindow();
        readonly Animator animator = new Animator();

        public CloaksLoad()
        {
            InitializeComponent();
        }

        private async void Cloaks_Loaded(object sender, RoutedEventArgs e)
        {
            animator.Fade(CloaksBG);
            animator.Fade(CloaksText);
            animator.ObjectShift(CloaksBG, CloaksBG.Margin, new Thickness(0, 0, 0, 0));
            animator.ObjectShift(CloaksText, CloaksText.Margin, new Thickness(0, 0, 0, 0));
            await Task.Delay(1500);
            animator.FadeOut(CloaksBG);
            animator.FadeOut(CloaksText);
            animator.ObjectShift(CloaksBG, CloaksBG.Margin, new Thickness(472, 0, 0, 0));
            animator.ObjectShift(CloaksText, CloaksText.Margin, new Thickness(-472, 0, 0, 0));
            await Task.Delay(1000);
            this.Hide();
            Main.Show();
        }
    }
}