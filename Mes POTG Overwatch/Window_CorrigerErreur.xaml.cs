using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using Window = System.Windows.Window;

namespace Mes_POTG_Overwatch
{
    /// <summary>
    /// Logique d'interaction pour Window_CorrigerErreur.xaml
    /// </summary>
    public partial class Window_CorrigerErreur : Window
    {
        private VideoCapture videoCapture;
        private Mat mat = new Mat();

        public Window_CorrigerErreur(List<TempsFort> tfWithErreur)
        {
            InitializeComponent();

            label_tfNuméro.Content = "Temps fort 1/" + tfWithErreur.Count;

            //for (int i = 0; i < tfWithErreur.Count; i++)
            //{
            TraiterTempsFort(tfWithErreur[0]);
            //}
        }

        private void TraiterTempsFort(TempsFort tempsFort)
        {
            videoCapture = new VideoCapture(tempsFort.Path);

            videoCapture.Read(mat);
            videoCapture.Set(CaptureProperty.PosFrames, 200);
            videoCapture.Read(mat);

            Bitmap screen = mat.ToBitmap(); // Bitmap de la frame numéro x

            System.Drawing.Rectangle cloneRect_screen = new System.Drawing.Rectangle(0, 0, 250, 100);
            System.Drawing.Imaging.PixelFormat format_screen =
                screen.PixelFormat;
            screen = screen.Clone(cloneRect_screen, format_screen);

            img_tf.Source = BitmapToImageSource(screen);

        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
