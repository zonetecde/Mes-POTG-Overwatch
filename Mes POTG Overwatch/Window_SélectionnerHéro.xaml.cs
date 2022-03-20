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
using System.Windows.Shapes;

namespace Mes_POTG_Overwatch
{
    /// <summary>
    /// Logique d'interaction pour Window_SélectionnerHéro.xaml
    /// </summary>
    public partial class Window_SéléctionnerHéro : Window
    {
        public Window_SéléctionnerHéro(TempsFort tempsFort)
        {
            InitializeComponent();

            List<Héro> héros = Enum.GetValues(typeof(Héro)).Cast<Héro>().ToList();

            foreach (Héro héro in héros)
            {
                if (héro != Héro.null_)
                {
                    UserControl_Héro userControl_Héro = new UserControl_Héro(héro, true, this);
                    WrapPanel_héro.Children.Add(userControl_Héro);
                }
            }
            TempsFort = tempsFort;
        }

        public TempsFort TempsFort { get; }

        /// <summary>
        /// Séléctionne le héro
        /// </summary>
        /// <param name="héro"></param>
        internal void HéroSéléctionné(Héro héro)
        {
            MainWindow.Utilities.resource.TempsForts[MainWindow.Utilities.resource.TempsForts.IndexOf(TempsFort)].Héro = héro;
            MainWindow.Utilities.SaveResources();

            foreach(UserControl_TempsFort userControl_TempsFort in MainWindow.MainWindow_.WrapPanel_tf.Children)
            {
                if(userControl_TempsFort.TempsFort == TempsFort)
                    userControl_TempsFort.img_hero.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"/images_hero/" + héro.ToString() + ".png"));
            }

            this.Close();
        }
    }
}
