using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using Size = OpenCvSharp.Size;

namespace Mes_POTG_Overwatch
{
    class OCR
    {
        private List<LocalBitmap> images_ref_1080p = new List<LocalBitmap>();

        /// <summary>
        /// Initialize toutes les images de ref 
        /// </summary>
        internal void InitializeOCR()
        {
            List<string> imgs_ref = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"\images_ref").ToList();
            foreach (string img in imgs_ref)
            {
                images_ref_1080p.Add(new LocalBitmap(img));
            }
        }

        private static Color[] ColorArray = new Color[]
        {
            Color.White,
            Color.Black,
            Color.Orange,
            Color.Blue,
        };

        /// <summary>
        /// Retourne les données de l'image en temps fort
        /// </summary>
        /// <param name="img_tempsfort"></param>
        /// <returns></returns>
        public TempsFort GetTempsFort(Mat img_tempsfort, string filename)
        {
            bool isPotg = false;

            // is POTG
            
            img_tempsfort.ConvertTo(img_tempsfort, MatType.CV_8U);
            Color PixelColor = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(img_tempsfort).GetPixel(248, 39);

            // 39 248

            if (PixelColor.B < 150)
            {
                isPotg = true;
            }
            

            // Qu'elle héro c'est
            // 1. Récupérer que le nom du héro

            img_tempsfort = new Mat(img_tempsfort, new OpenCvSharp.Rect(0, 70, 250, 30));


            // 2. Mettre l'image en pixel noir et blanc
            img_tempsfort = img_tempsfort.Threshold(180, 255, ThresholdTypes.Binary);

            img_tempsfort.SaveImage(@"C:\Users\Zonedetec\AppData\Roaming\Zonedetec\Mes Temps Forts Overwatch\t.png");

            // OpenCvSharp.Extensions.BitmapConverter.ToBitmap(img_tempsfort).Save(@"C:\Users\rayane staszewski\Documents\repos\vid\testt.png");

            // 3. Enfin comparer avec les images connus
            List<HeroRessemblance> Ressemblances = new List<HeroRessemblance>();
            int nbPixelBlanc = 0;

            foreach (LocalBitmap img_ref in images_ref_1080p)
            {
                int nbPixelMêmeCouleur = 0;

                Mat result = new Mat();
                Mat src2 = new Mat();
                src2 = BitmapConverter.ToMat(img_ref.Bitmap);

                Cv2.Absdiff(BitmapConverter.ToMat(BitmapConverter.ToBitmap(img_tempsfort)), BitmapConverter.ToMat(BitmapConverter.ToBitmap(src2)), result);

                result = result.CvtColor(ColorConversionCodes.BGR2GRAY);

                nbPixelMêmeCouleur = result.Width * result.Height - result.CountNonZero();
                nbPixelBlanc = result.CountNonZero();

                // Convertit en pourcentage
                int nbPixelTotal = img_tempsfort.Height * img_tempsfort.Width;
                decimal pourcentage = Math.Round(((decimal)nbPixelMêmeCouleur / (decimal)nbPixelTotal) * 100, 2);
                Ressemblances.Add(new HeroRessemblance(img_ref.Name, pourcentage));
            }


            // 4. Enfin on regarde celui qui a le plus pourcentage le plus élevé
            decimal PlusHautPourcentage = Ressemblances.Select(s => s.Pourcentage).ToList().Max();
            Héro HéroJoué = Héro.null_;

            if (nbPixelBlanc > 5)
            {
                foreach (HeroRessemblance heroRessemblance in Ressemblances)
                {
                    if (heroRessemblance.Pourcentage == PlusHautPourcentage)
                    {
                        HéroJoué = (Héro)Enum.Parse(typeof(Héro), heroRessemblance.HeroName.Replace(' ', '_'));
                    }
                }
            }
            else
            {
                HéroJoué = Héro.null_;
            }

            return new TempsFort
            {
                Héro = HéroJoué,
                IsPOTG = isPotg,
            };
        }

        class LocalBitmap
        {
            public Bitmap Bitmap { get; set; }
            public string Name { get; set; }

            public LocalBitmap(string path)
            {
                Name = Path.GetFileNameWithoutExtension(path);
                Bitmap = new Bitmap(path);
            }
        }

        class HeroRessemblance
        {
            public decimal Pourcentage { get; set; }
            public string HeroName { get; set; }

            public HeroRessemblance(string heroName, decimal pourcentage)
            {
                HeroName = heroName;
                Pourcentage = pourcentage;
            }
        }
    }
}
