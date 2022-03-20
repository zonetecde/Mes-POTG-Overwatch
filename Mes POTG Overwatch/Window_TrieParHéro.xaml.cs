using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Mes_POTG_Overwatch
{
    /// <summary>
    /// Logique d'interaction pour Window_TrieParHéro.xaml
    /// </summary>
    public partial class Window_TrieParHéro : Window
    {
        public Window_TrieParHéro()
        {
            InitializeComponent();

            List<Héro> héros = Enum.GetValues(typeof(Héro)).Cast<Héro>().ToList();

            foreach (Héro héro in héros)
            {
                if (héro != Héro.null_)
                {
                    UserControl_Héro userControl_Héro = new UserControl_Héro(héro);
                    WrapPanel_héro.Children.Add(userControl_Héro);
                }
            }
        }

        /// <summary>
        /// Coche tous les héros
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (UserControl_Héro userControl_Héro in WrapPanel_héro.Children)
            {
                userControl_Héro.checkbox_autoriser.IsChecked = true;
            }
        }

        /// <summary>
        /// Décoche tous les héros
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            foreach (UserControl_Héro userControl_Héro in WrapPanel_héro.Children)
            {
                userControl_Héro.checkbox_autoriser.IsChecked = false;
            }
        }
    }
}
