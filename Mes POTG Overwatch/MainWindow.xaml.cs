using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Mes_POTG_Overwatch
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Utilities Utilities = new Utilities();
        private int nbTFAffichés = 0;
        private List<TempsFort> A_Afficher = new List<TempsFort>();
        public static List<Héro> HéroFiltre = new List<Héro>();
        private int ModeAffichage = 0; // 0 = tous 1 = POTG 2 = tf
        private string Search = string.Empty;
        List<TempsFort> tfWithErreur = new List<TempsFort>();

        public static MainWindow MainWindow_ { get; internal set; }

        public MainWindow()
        {
            InitializeComponent();

            Utilities.InitializeResourcesPath();
            Utilities.LoadResources();

            MainWindow_ = this;
        }


        /// <summary>
        /// Est-ce que c'est le premier lancement ? Si oui récupérer tous les POTG
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            if (Utilities.IsFirstLaunch())
            {
                HéroFiltre = Enum.GetValues(typeof(Héro)).Cast<Héro>().ToList();

                // Ouvre la fenêtre de récupération des POTG
                Window_GetPOTG window_GetPOTG = new Window_GetPOTG(this);
                window_GetPOTG.ShowDialog();

                //button_erreurs
                int nbTfErreurs = 0;
                tfWithErreur = new List<TempsFort>();
                Utilities.resource.TempsForts.ForEach(x =>
                {
                    if (x.Héro == Héro.null_)
                    {
                        nbTfErreurs++;
                        tfWithErreur.Add(x);
                    }
                });

                if (nbTfErreurs > 0)
                {
                    button_erreurs.Visibility = Visibility.Visible;
                    button_erreurs.Content = "Corriger les erreurs (" + nbTfErreurs + ")";
                }

                A_Afficher = new List<TempsFort>(Utilities.resource.TempsForts);

                // Affiche les tf 
                nbTFAffichés = 0;

                AfficherNext30TF();
            }
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (scrollViewer_tf.ScrollableHeight == e.VerticalOffset && WrapPanel_tf.Children.Count > 9)
            {
                AfficherNext30TF();
            }
        }

        /// <summary>
        /// Affiche les 30 prochains tf
        /// </summary>
        private void AfficherNext30TF()
        {
            switch (ModeAffichage)
            {
                case 0:
                    if (Utilities.resource.TempsForts != null)
                        A_Afficher = new List<TempsFort>(Utilities.resource.TempsForts);

                    break;
                case 1:
                    Utilities.resource.TempsForts.ForEach(x =>
                    {
                        if (x.IsPOTG)
                            A_Afficher.Add(x);
                    });
                    break;
                case 2:
                    Utilities.resource.TempsForts.ForEach(x =>
                    {
                        if (!x.IsPOTG)
                            A_Afficher.Add(x);
                    });
                    break;
            }

            for (int i = A_Afficher.Count - 1; i >= 0; i--)
            {
                if (!HéroFiltre.Contains(A_Afficher[i].Héro) || A_Afficher[i].Héro == Héro.null_)
                {
                    A_Afficher.RemoveAt(i);
                }
            }

            for (int i = A_Afficher.Count - 1; i >= 0; i--)
            {
                if (!String.IsNullOrEmpty(Search))
                {
                    if (!A_Afficher[i].Titre.ToLower().Contains(Search.ToLower()))
                    {
                        A_Afficher.RemoveAt(i);
                    }
                }
            }

            int temp = nbTFAffichés;

            for (int i = nbTFAffichés; i < temp + 30; i++)
            {
                try
                {
                    if (String.IsNullOrEmpty(A_Afficher[i].Titre))
                    {
                        A_Afficher[i].Titre = "Temps fort sans titre";
                    }

                    UserControl_TempsFort tempsFort = new UserControl_TempsFort(A_Afficher[i]);
                    WrapPanel_tf.Children.Add(tempsFort);

                }
                catch { }

                nbTFAffichés++;
            }

            try
            {
                label_nbtf.Content = A_Afficher.Count + " Temps forts.";
            }
            catch { }

        }

        /// <summary>
        /// Enlève le rechercher
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label_rechercher_MouseDown(object sender, MouseButtonEventArgs e)
        {
            label_rechercher.Visibility = Visibility.Hidden;
            textbox_Search.Focus();
        }

        /// <summary>
        /// Remet le rechercher si le texte est vide
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textbox_Search_LostFocus(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(textbox_Search.Text))
                label_rechercher.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Affiche uniquement les potg
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton_potg_Checked(object sender, RoutedEventArgs e)
        {
            WrapPanel_tf.Children.Clear();
            A_Afficher.Clear();

            nbTFAffichés = 0;
            ModeAffichage = 1;

            AfficherNext30TF();

            scrollViewer_tf.ScrollToHome();
        }

        /// <summary>
        /// Affiche uniquement les tf
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton_tf_Checked(object sender, RoutedEventArgs e)
        {
            WrapPanel_tf.Children.Clear();
            A_Afficher.Clear();

            nbTFAffichés = 0;
            ModeAffichage = 2;

            AfficherNext30TF();

            scrollViewer_tf.ScrollToHome();
        }

        /// <summary>
        /// Affiche tous
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton_tous_Checked(object sender, RoutedEventArgs e)
        {
            ModeAffichage = 0;

            WrapPanel_tf.Children.Clear();
            A_Afficher.Clear();

            nbTFAffichés = 0;

            AfficherNext30TF();

            scrollViewer_tf.ScrollToHome();
        }

        /// <summary>
        /// Fenêtre trie par héro
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window_TrieParHéro window_TrieParHéro = new Window_TrieParHéro();
            window_TrieParHéro.ShowDialog();

            WrapPanel_tf.Children.Clear();
            A_Afficher.Clear();

            nbTFAffichés = 0;
            AfficherNext30TF();

            scrollViewer_tf.ScrollToHome();
        }

        /// <summary>
        /// Modifie option de recherche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textbox_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(textbox_Search.Text))
            {
                Search = textbox_Search.Text;

                WrapPanel_tf.Children.Clear();
                A_Afficher.Clear();

                nbTFAffichés = 0;

                AfficherNext30TF();
            }
            else
            {
                Search = string.Empty;

                WrapPanel_tf.Children.Clear();
                A_Afficher.Clear();

                nbTFAffichés = 0;

                AfficherNext30TF();

                scrollViewer_tf.ScrollToHome();
            }
        }


        /// <summary>
        /// Corrige les erreurs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_erreurs_Click(object sender, RoutedEventArgs e)
        {
            Window_CorrigerErreur window_CorrigerErreur = new Window_CorrigerErreur(tfWithErreur);
            window_CorrigerErreur.ShowDialog();
        }

        /// <summary>
        /// Créer une compilation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Créer une compilation
            scrollViewer_tf.Margin = new Thickness(scrollViewer_tf.Margin.Left, scrollViewer_tf.Margin.Top, scrollViewer_tf.Margin.Right, 125);
            scrollViewer_compilation.Visibility = Visibility.Visible;
            label_WarningAucunTF.Visibility = Visibility.Visible;
            grid_timeline.Visibility = Visibility.Visible;

            button_ajouterTous.IsEnabled = true;

            // Ajoute l'option dans le context menu des UC temps fort "Ajouter à la compilation"
            foreach (UserControl_TempsFort userControl_TempsFort in WrapPanel_tf.Children)
            {
                userControl_TempsFort.EnabledOrDisableCompilationOption(true);
            }
        }

        /// <summary>
        /// Ajoute une vidéo à la compilation
        /// </summary>
        /// <param name="tempsFort"></param>
        internal void AddHighlightToCompilation(TempsFort tempsFort)
        {
            // Enlève le label de warning
            label_WarningAucunTF.Visibility = Visibility.Hidden;

            wrapPanel_timeline.Children.Add(new UserControl_CompilationItem(tempsFort));
            scrollViewer_compilation.ScrollToRightEnd();
        }

        /// <summary>
        /// Créer une compilation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (wrapPanel_timeline.Children.Count > 1)
                {
                    MessageBox.Show("Un dossier s'ouvrira contenant votre compilation sous le nom de \"output.mp4\". Merci de ne pas l'ouvrir tant que la console ne s'est pas fermé.", "Attention!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    List<string> filespath = new List<string>();
                    foreach (UserControl_CompilationItem userControl in wrapPanel_timeline.Children)
                    {
                        filespath.Add(userControl.TempsFort.Path);
                    }

                    CréerUnMontage.CréerMontage(filespath);
                }
                else
                {
                    MessageBox.Show("Il faut minimum 2 temps forts dans la compilation.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Arrête la compilation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                wrapPanel_timeline.Children.Clear();
                grid_timeline.Visibility = Visibility.Hidden;
                button_ajouterTous.IsEnabled = false;
                scrollViewer_compilation.Visibility = Visibility.Hidden;
                label_WarningAucunTF.Visibility = Visibility.Hidden;
                scrollViewer_tf.Margin = new Thickness(scrollViewer_tf.Margin.Left, scrollViewer_tf.Margin.Top, scrollViewer_tf.Margin.Right, 10);

                foreach (UserControl_TempsFort userControl_TempsFort in WrapPanel_tf.Children)
                    userControl_TempsFort.AjouterALaCompilation.IsEnabled = false;
            }
        }

        /// <summary>
        /// Ajoute tous les temps "A affcher" dans la compil
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_ajouterTous_Click(object sender, RoutedEventArgs e)
        {
            foreach(TempsFort tempsFort in A_Afficher)
            {
                wrapPanel_timeline.Children.Add(new UserControl_CompilationItem(tempsFort));
                label_WarningAucunTF.Visibility = Visibility.Hidden;
            }
        }
    }
}
