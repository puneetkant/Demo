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
        private static DispatcherTimer _playTimer = new DispatcherTimer();
        private static string[] _imageFiles;
        private static Random _random;
        private static bool _updateFirstImage = true;

        public Settings Settings { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            this.Settings = Settings.LoadSettings();
            
            _playTimer.Tick += new EventHandler(playTimer_Tick);
        }

        private void playTimer_Tick(object sender, EventArgs e)
        {
            int nextRandom = _random.Next(0, _imageFiles.Length-1);

            BitmapImage bitImage = new BitmapImage();
            bitImage.BeginInit();
            bitImage.UriSource = new Uri(_imageFiles[nextRandom], UriKind.Absolute);
            bitImage.EndInit();

            if (_updateFirstImage)
            {
                SlideImage.Stretch = Settings.Style;
                SlideImage.Source = bitImage;
                SlideImage.Opacity = 1;
                SlideImageSecond.Opacity = 0;
                _updateFirstImage = false;
            }
            else
            {
                SlideImageSecond.Stretch = Settings.Style;
                SlideImageSecond.Source = bitImage;
                SlideImageSecond.Opacity = 1;
                SlideImage.Opacity = 0;
                _updateFirstImage = true;
            }
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
                _playTimer.Stop();
            }
        }

        private void PlaySlides()
        {
            if (Directory.Exists(this.Settings.Folders) == false)
                return;

            _imageFiles = Directory.GetFiles(this.Settings.Folders, "*.jpg", SearchOption.AllDirectories);
            _random = new Random();
            _playTimer.Interval = new TimeSpan(0, 0, 0, 0, (int) (this.Settings.Timer * 1000));

            _playTimer.Start();

        }

        private void TransitionMenu_Click(object sender, RoutedEventArgs e)
        {
            var transitionWindow = new ImageTransition();
            transitionWindow.Show();
        }
    }
}
