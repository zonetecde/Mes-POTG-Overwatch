using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mes_POTG_Overwatch
{
    /// <summary>
    /// Logique d'interaction pour UserControl_CompilationItem.xaml
    /// </summary>
    public partial class UserControl_CompilationItem : UserControl
    {
        public UserControl_CompilationItem(TempsFort tempsFort)
        {
            InitializeComponent();
            TempsFort = tempsFort;

            this.DataContext = TempsFort;
            img_hero.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"/images_hero/" + tempsFort.Héro.ToString() + ".png"));
        }

        public TempsFort TempsFort { get; }

        /// <summary>
        /// Décale l'image à gauche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                try
                {
                    int index = MainWindow.MainWindow_.wrapPanel_timeline.Children.IndexOf(this);

                    if (index != 0)
                    {
                        MainWindow.MainWindow_.wrapPanel_timeline.Children.Remove(this);
                        MainWindow.MainWindow_.wrapPanel_timeline.Children.Insert(index - 1, this);
                    }
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// Décale l'image à droite
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rightArrow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                try
                {
                    int index = MainWindow.MainWindow_.wrapPanel_timeline.Children.IndexOf(this);

                    if (index + 1 != MainWindow.MainWindow_.wrapPanel_timeline.Children.Count)
                    {
                        MainWindow.MainWindow_.wrapPanel_timeline.Children.Remove(this);
                        MainWindow.MainWindow_.wrapPanel_timeline.Children.Insert(index + 1, this);
                    }
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// Supprime
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            MainWindow.MainWindow_.wrapPanel_timeline.Children.Remove(this);

            if (MainWindow.MainWindow_.wrapPanel_timeline.Children.Count == 0)
                MainWindow.MainWindow_.label_WarningAucunTF.Visibility = Visibility.Visible;
        }
    }
}
