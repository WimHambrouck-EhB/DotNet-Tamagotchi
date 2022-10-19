using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace TamaSim
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Tamagotchi tamagotchi;
        private readonly BitmapImage doodImage;
        private readonly BitmapImage seniorImage;
        private readonly BitmapImage eiImage;
        private readonly BitmapImage babyImage;
        private readonly BitmapImage kindImage;
        private readonly BitmapImage puberImage;
        private readonly BitmapImage volwassenImage;

        public MainWindow()
        {
            InitializeComponent();

            // initialize images
            eiImage = InitImage(Properties.Resources.tamagotchi_ei);
            babyImage = InitImage(Properties.Resources.tamagotchi_baby);
            kindImage = InitImage(Properties.Resources.tamagotchi_kind);
            puberImage = InitImage(Properties.Resources.tamagotchi_puber);
            volwassenImage = InitImage(Properties.Resources.tamagotchi_volwassen);
            seniorImage = InitImage(Properties.Resources.tamagotchi);
            doodImage = InitImage(Properties.Resources.tamagotchi_dood);

            NameWindow nameWindow = new NameWindow();

            if (nameWindow.ShowDialog().GetValueOrDefault())
            {
                tamagotchi = new Tamagotchi(nameWindow.TamaName);
                UpdateGUI();
                tamagotchi.LevensstadiumChangedEvent += Tamagotchi_LevensstadiumChangedEvent;
                tamagotchi.ParameterChangedEvent += Tamagotchi_ParameterChangedEvent;
            }
            else
            {
                Close();
            }
        }



        /// <summary>
        /// Zet image van resources om naar BitmapImage voor gebruik in Image UI-element
        /// </summary>
        /// <param name="imageData">Een afbeelding, als array van bytes.</param>
        /// <returns>Een <see cref="BitmapImage"/> opgevuld met de <paramref name="imageData"/>.</returns>
        private BitmapImage InitImage(byte[] imageData)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = new MemoryStream(imageData);
            image.EndInit();

            return image;
        }

        private void Tamagotchi_LevensstadiumChangedEvent(Levensstadium levensstadium)
        {
            // als de cases van een switch niet veel code bevatten (in dit geval enkel een toewijzing aan newImage), kan je de switch sinds C# 8 als expression schrijven en het resultaat hiervan opvangen in een variabele
            // cf.: https://www.c-sharpcorner.com/article/c-sharp-8-0-new-feature-swtich-expression/
            var newImage = levensstadium switch
            {
                Levensstadium.Ei => eiImage,
                Levensstadium.Baby => babyImage,
                Levensstadium.Kind => kindImage,
                Levensstadium.Puber => puberImage,
                Levensstadium.Volwassen => volwassenImage,
                Levensstadium.Senior => seniorImage,
                Levensstadium.Dood => doodImage,
                _ => volwassenImage,
            };

            /* Dispatcher.Invoke() wordt gebruikt om een stukje code op dezelfde thread uit te voeren als het oproepend object (in dit geval MainWindow). 
             * Dit om een exception met fout "The calling thread cannot access this object because a different thread owns it" te voorkomen.
             * UI mag immers slechts vanuit de main thread worden geüpdatet, om race conditions te voorkomen.
             */
            Dispatcher.Invoke(() =>
            {
                imgTamagotchi.Source = newImage;
                if (Levensstadium.Dood == levensstadium)
                {
                    btnEten.IsEnabled = false;
                    btnLeren.IsEnabled = false;
                    btnSpelen.IsEnabled = false;
                }
            });
        }

        private void Tamagotchi_ParameterChangedEvent()
        {
            Dispatcher.Invoke(() => UpdateGUI());
        }

        private void UpdateGUI()
        {
            lblNaam.Content = tamagotchi.Naam;
            lblLevensstadium.Content = tamagotchi.Levensstadium;
            lblGeluk.Content = tamagotchi.Geluk;
            lblHonger.Content = tamagotchi.Honger;
            lblIntelligentie.Content = tamagotchi.Intelligentie;
        }

        private void btnSpelen_Click(object sender, RoutedEventArgs e)
        {
            tamagotchi.Geluk++;
            UpdateGUI();
        }

        private void btnEten_Click(object sender, RoutedEventArgs e)
        {
            tamagotchi.Honger++;
            UpdateGUI();
        }

        private void btnLeren_Click(object sender, RoutedEventArgs e)
        {
            tamagotchi.Intelligentie++;
            UpdateGUI();
        }
    }
}
