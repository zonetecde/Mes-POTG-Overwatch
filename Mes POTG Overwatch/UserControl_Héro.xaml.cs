using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Mes_POTG_Overwatch
{
    /// <summary>
    /// Logique d'interaction pour UserControl_Héro.xaml
    /// </summary>
    public partial class UserControl_Héro : UserControl
    {
        public UserControl_Héro(Héro héro, bool Séléctionner = false, Window_SéléctionnerHéro window_SélectionnerHéro = null)
        {
            InitializeComponent();

            img_héro.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"/images_hero/" + héro.ToString() + ".png"));

            if (MainWindow.HéroFiltre.Contains(héro))
            {
                checkbox_autoriser.IsChecked = true;
            }
            Héro = héro;
            Window_SélectionnerHéro = window_SélectionnerHéro;
            if (Séléctionner)
            {
                checkbox_autoriser.Visibility = Visibility.Hidden;
            }
        }

        public Héro Héro { get; }
        public Window_SéléctionnerHéro Window_SélectionnerHéro { get; }

        private void checkbox_autoriser_Checked(object sender, RoutedEventArgs e)
        {
            if (!MainWindow.HéroFiltre.Contains(Héro))
                MainWindow.HéroFiltre.Add(Héro);
        }

        private void checkbox_autoriser_Unchecked(object sender, RoutedEventArgs e)
        {
            if (MainWindow.HéroFiltre.Contains(Héro))
                MainWindow.HéroFiltre.Remove(Héro);
        }

        /// <summary>
        /// Sélectionne le héro
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void img_héro_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                if(Window_SélectionnerHéro != null)
                    Window_SélectionnerHéro.HéroSéléctionné(Héro);
            }
        }
    }
}
