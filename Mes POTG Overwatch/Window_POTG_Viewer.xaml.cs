using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
    /// Logique d'interaction pour Window_POTG_Viewer.xaml
    /// </summary>
    public partial class Window_POTG_Viewer : Window
    {
        Timer Timer;

        public Window_POTG_Viewer(TempsFort tempsFort)
        {
            InitializeComponent();
            TempsFort = tempsFort;

            mePlayer.Source = new Uri(tempsFort.Path);

            this.DataContext = tempsFort;

            Timer = new Timer(13000);
            Timer.Elapsed += new ElapsedEventHandler(RecommencerVidéo);
            Timer.Start();
        }

        /// <summary>
        /// Recommence la vidéo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecommencerVidéo(object sender, ElapsedEventArgs e)
        {
            Timer.Stop();
            Timer = new Timer(13000);
            Timer.Elapsed += new ElapsedEventHandler(RecommencerVidéo);
            Timer.Start();

            Dispatcher.Invoke(() =>
            {
                mePlayer.Stop();
                mePlayer.Position = new TimeSpan(0, 0, 5);
                mePlayer.Play();
            });
        }

        public TempsFort TempsFort { get; }

        /// <summary>
        /// Ajuste la taille de la fenetre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            this.Height -= 10;
            this.Width -= 10;

            // Skip le début
            mePlayer.Position = new TimeSpan(0, 0, 5);
            mePlayer.Play();
        }

        /// <summary>
        /// Quitte
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

    }
}
