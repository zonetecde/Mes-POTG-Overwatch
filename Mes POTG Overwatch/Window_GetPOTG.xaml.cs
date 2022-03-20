using Microsoft.WindowsAPICodePack.Dialogs;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using Path = System.IO.Path;
using Rectangle = System.Drawing.Rectangle;

namespace Mes_POTG_Overwatch
{
    /// <summary>
    /// Logique d'interaction pour Window_GetPOTG.xaml
    /// </summary>
    public partial class Window_GetPOTG : System.Windows.Window
    {
        private string POTG_Folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Overwatch\videos\overwatch";
        BackgroundWorker BackgroundWorker = new BackgroundWorker();
        private int VidéosTaitées;
        private int VidéosTotale;

        public MainWindow MainWindow { get; }

        public Window_GetPOTG(MainWindow mainWindow)
        {
            InitializeComponent();

            textBox_POTG_path.Text = POTG_Folder;

            if(MainWindow.Utilities.resource.POTG_Path != null)
                textBox_POTG_path.Text = MainWindow.Utilities.resource.POTG_Path;


            BackgroundWorker.DoWork += new DoWorkEventHandler(AnalyserLesTempsForts);
            BackgroundWorker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
            BackgroundWorker.WorkerReportsProgress = true;
            MainWindow = mainWindow;
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            label_counter.Content = VidéosTaitées + "/" + VidéosTotale;
            progressBar.Value++;

            if (VidéosTaitées == VidéosTotale)
            {
                MainWindow.Utilities.resource.TempsForts = MainWindow.Utilities.resource.TempsForts.OrderBy(x => x.Date).ToList();
                MainWindow.Utilities.resource.TempsForts.Reverse();

                MainWindow.Utilities.SaveResources();

                forcedClose = false;

                BackgroundWorker.Dispose();
                VidéosTaitées++;
                this.Close();
            }
        }

        public Bitmap ResizeBitmap(Bitmap bmp, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(bmp, 0, 0, width, height);
            }

            return result;
        }


        /// <summary>
        /// Analyse les temps forts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnalyserLesTempsForts(object sender, DoWorkEventArgs e)
        {
            List<string> filePaths = Directory.GetFiles(POTG_Folder).ToList();

            MainWindow.Utilities.resource.POTG_Path = POTG_Folder;

            // Compte le nombre de vidéo qu'il y a dans le dossier
            FiltrerLesVidéosDuDossier(filePaths);

            CorrigerLeNomDesFichiers(filePaths);

            SupprimerLesTempsFortsQuiNexistePlus();

            VidéosTaitées = 0;
            VidéosTotale = filePaths.Count;

            Dispatcher.Invoke(() =>
            {
                label_counter.Content = VidéosTaitées + "/" + VidéosTotale;
                progressBar.Maximum = filePaths.Count;
            });

            VideoCapture videoCapture;
            OCR oCR = new OCR();

            oCR.InitializeOCR();

            Mat mat = new Mat();

            foreach (string file in filePaths)
            {
                bool existeDéjà = MainWindow.Utilities.resource.TempsForts.Any(item => item.Path == file); // Si le tf a déjà été importé
                TempsFort tf;
                string tempFile = file;
                
                if (!existeDéjà)
                {

                    try
                    {
                        videoCapture = new VideoCapture(file);

                        videoCapture.Read(mat);
                        videoCapture.Set(CaptureProperty.PosFrames, 750);
                        videoCapture.Read(mat);

                        Bitmap screen = mat.ToBitmap(); // Bitmap de la frame numéro x

                        screen = ResizeBitmap(screen, 1920, 1080);

                        tf = CreatePOTG(videoCapture, oCR, mat, file, tempFile, ref screen);

                        MainWindow.Utilities.resource.TempsForts.Add(tf);

                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            videoCapture = new VideoCapture(file);

                            videoCapture.Read(mat);
                            videoCapture.Set(CaptureProperty.PosFrames, 200);
                            videoCapture.Read(mat);

                            Bitmap screen = mat.ToBitmap(); // Bitmap de la frame numéro x

                            screen = ResizeBitmap(screen, 1920, 1080);

                            tf = CreatePOTG(videoCapture, oCR, mat, file, tempFile, ref screen);

                            MainWindow.Utilities.resource.TempsForts.Add(tf);
                        }
                        catch
                        {
                            File.Delete(file);
                        }
                    }

                }


                BackgroundWorker.ReportProgress(Convert.ToInt32((VidéosTaitées / VidéosTotale) * 100));
                VidéosTaitées++;
            }
        }

        private static void FiltrerLesVidéosDuDossier(List<string> filePaths)
        {
            for (int i = filePaths.Count - 1; i >= 0; i--)
            {
                if (!System.IO.Path.GetExtension(filePaths[i]).Equals(".mp4"))
                {
                    filePaths.RemoveAt(i);
                }
            }
        }

        private static void CorrigerLeNomDesFichiers(List<string> filePaths)
        {
            for (int i = 0; i < filePaths.Count; i++)
            {
                if (Path.GetFileName(filePaths[i]).Contains("♥"))
                {
                    File.Move(filePaths[i], filePaths[i].Replace('♥', ' '));
                    filePaths[i].Replace('♥', ' ');
                }
            }
        }

        private static void SupprimerLesTempsFortsQuiNexistePlus()
        {
            for (int i = MainWindow.Utilities.resource.TempsForts.Count - 1; i >= 0; i--)
            {
                if (!File.Exists(MainWindow.Utilities.resource.TempsForts[i].Path))
                    MainWindow.Utilities.resource.TempsForts.RemoveAt(i);
            }
        }

        private static void SupprimerTempsFortAvecHéroNull()
        {
            for (int i = MainWindow.Utilities.resource.TempsForts.Count - 1; i >= 0; i--)
            {
                if (MainWindow.Utilities.resource.TempsForts[i].Héro == Héro.null_)
                    MainWindow.Utilities.resource.TempsForts.RemoveAt(i);
            }
        }

        /// <summary>
        /// Créer le potg
        /// </summary>
        /// <param name="videoCapture"></param>
        /// <param name="oCR"></param>
        /// <param name="mat"></param>
        /// <param name="file"></param>
        /// <param name="tempFile"></param>
        /// <param name="screen"></param>
        /// <returns></returns>
        private static TempsFort CreatePOTG(VideoCapture videoCapture, OCR oCR, Mat mat, string file, string tempFile, ref Bitmap screen)
        {
            TempsFort tf;
            Mat mat1 = new Mat(mat, new OpenCvSharp.Rect(0, 0, 250, 100));


            // OCR
            tf = oCR.GetTempsFort(mat1, Path.GetFileNameWithoutExtension(tempFile));

            screen.Dispose();

            tf.Date = File.GetCreationTime(file);
            tf.Titre = Path.GetFileNameWithoutExtension(file).Remove(Path.GetFileNameWithoutExtension(file).Length - 18, 18); ; // 18 
            tf.Path = file;

            if (tf.Héro == Héro.null_)
            {
                videoCapture.Set(CaptureProperty.PosFrames, 50);

                Mat mat2 = new Mat(mat, new OpenCvSharp.Rect(mat.Width - (mat.Width - 100), mat.Height - 100, 100, 100));

                tf.ImageOfHero = "";
            }

            try
            {
                videoCapture.Dispose();
                File.Move(file.Replace('♥', ' '), tempFile);
            }
            catch (Exception ex) { }

            return tf;
        }

        /// <summary>
        /// Ouvre le répértoire pour séléctionner le dossier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                textBox_POTG_path.Text = dialog.FileName;
                POTG_Folder = dialog.FileName;

                this.Focus();
            }
        }

        /// <summary>
        /// Récupère les infos des temps forts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(POTG_Folder))
            {
                (sender as UIElement).Visibility = Visibility.Hidden;

                label_counter.Visibility = Visibility.Visible;
                progressBar.Visibility = Visibility.Visible;

                BackgroundWorker.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("Le dossier n'existe pas.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }


        bool forcedClose = true;
        private void Window_Closed(object sender, EventArgs e)
        {
            if (forcedClose)
                MainWindow.Close();
        }

        /// <summary>
        /// Continue sans analyser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Label_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                forcedClose = false;
                this.Close();
            }
        }
    }
}
