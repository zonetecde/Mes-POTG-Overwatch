using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Mes_POTG_Overwatch
{
    /// <summary>
    /// Logique d'interaction pour UserControl_TempsFort.xaml
    /// </summary>
    public partial class UserControl_TempsFort : UserControl
    {
        public UserControl_TempsFort(TempsFort tempsFort)
        {
            InitializeComponent();
            TempsFort = tempsFort;
            img_hero.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"/images_hero/" + tempsFort.Héro.ToString() + ".png"));

            label_ap.Visibility = tempsFort.IsPOTG ? Visibility.Visible : Visibility.Hidden;

            this.DataContext = tempsFort;

            if (tempsFort.IsPOTG)
                isPOTG.IsChecked = true;

            if (MainWindow.MainWindow_.scrollViewer_compilation.Visibility == Visibility.Visible)
                AjouterALaCompilation.IsEnabled = true;

            label_date.Content = tempsFort.Date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }

        public TempsFort TempsFort { get; }

        /// <summary>
        /// Supprime le tf
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.MainWindow_.WrapPanel_tf.Children.Remove(this);
            File.Delete(TempsFort.Path);
            MainWindow.Utilities.resource.TempsForts.Remove(TempsFort);
            MainWindow.Utilities.SaveResources();
        }

        /// <summary>
        /// Ouvre le tf
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Window_POTG_Viewer window_POTG_Viewer = new Window_POTG_Viewer(TempsFort);
                window_POTG_Viewer.ShowDialog();
            }

            // Le séléctionne
            // Enlève les bordures de tous les uc
            foreach (UserControl_TempsFort userControl_TempsFort in MainWindow.MainWindow_.WrapPanel_tf.Children)
            {
                userControl_TempsFort.BorderBrush = System.Windows.Media.Brushes.White;
                userControl_TempsFort.BorderThickness = new Thickness(1);
            }

            this.BorderBrush = System.Windows.Media.Brushes.Red;
            this.BorderThickness = new Thickness(2);
        }

        /// <summary>
        /// Change le héro
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Window_SéléctionnerHéro window_SéléctionnerHéro = new Window_SéléctionnerHéro(TempsFort);
            window_SéléctionnerHéro.ShowDialog();
        }

        private void isPOTG_Checked(object sender, RoutedEventArgs e)
        {
            label_ap.Visibility = Visibility.Visible;
            MainWindow.Utilities.resource.TempsForts[MainWindow.Utilities.resource.TempsForts.IndexOf(TempsFort)].IsPOTG = true;
            MainWindow.Utilities.SaveResources();
        }

        private void isPOTG_Unchecked(object sender, RoutedEventArgs e)
        {
            label_ap.Visibility = Visibility.Hidden;
            MainWindow.Utilities.resource.TempsForts[MainWindow.Utilities.resource.TempsForts.IndexOf(TempsFort)].IsPOTG = false;
            MainWindow.Utilities.SaveResources();
        }

        /// <summary>
        /// Active l'option "Ajouter à la compilation"
        /// </summary>
        /// <param name="v"></param>
        internal void EnabledOrDisableCompilationOption(bool activerOuDésactiver)
        {
            AjouterALaCompilation.IsEnabled = activerOuDésactiver;
        }

        /// <summary>
        /// Ajoute la vidéo à la compilation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AjouterALaCompilation_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.MainWindow_.AddHighlightToCompilation(TempsFort);
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            
            if (System.IO.File.Exists(TempsFort.Path))
            {
                //Clean up file path so it can be navigated OK
                TempsFort.Path = System.IO.Path.GetFullPath(TempsFort.Path);
                System.Diagnostics.Process.Start("explorer.exe", string.Format("/select,\"{0}\"", TempsFort.Path));
            }

            
        }
    }
}
