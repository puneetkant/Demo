using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SuperSliderWin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static DispatcherTimer playTimer = new DispatcherTimer();
        private static string[] imageFiles;
        private static Random random;

        public Settings Settings { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            this.Settings = Settings.LoadSettings();
            playTimer.Interval = new TimeSpan(0,0, this.Settings.Timer);
            playTimer.Tick += new EventHandler(playTimer_Tick);
        }

        private void playTimer_Tick(object sender, EventArgs e)
        {
            int nextRandom = random.Next(0, imageFiles.Length-1);

            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri(imageFiles[nextRandom], UriKind.Absolute);
            bi3.EndInit();
            SlideImage.Stretch = Stretch.Fill;
            SlideImage.Source = bi3;
        }

        private void settingsMenu_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow(this.Settings);
            settingsWindow.ShowDialog();
        }

        private void PlayMenu_Click(object sender, RoutedEventArgs e)
        {
            if (PlayMenu.Header.ToString() == "Play")
            {
                PlayMenu.Header = "Stop";
                PlaySlides();
            }
            else
            {
                PlayMenu.Header = "Play";
                playTimer.Stop();
            }
        }

        private void PlaySlides()
        {
            if (Directory.Exists(this.Settings.Folders) == false)
                return;

            imageFiles = Directory.GetFiles(this.Settings.Folders, "*.jpg", SearchOption.AllDirectories);
            random = new Random();


            playTimer.Start();

        }
    }
}
