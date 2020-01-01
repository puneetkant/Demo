using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace SuperSliderWin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static DispatcherTimer _playTimer;
        private static string[] _imageFiles;
        private static Random _random;
        private static bool _updateFirstImage = true;
        private static TimeSpan _fadeInTime;
        private static TimeSpan _fadeOutTime;

        private static DoubleAnimation _fadeInAnimation;
        private static DoubleAnimation _fadeOutAnimation;

        public Settings Settings { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            this.Settings = Settings.LoadSettings();

            _fadeInTime = new TimeSpan(0, 0, 0, 0, (int)(this.Settings.FadeInTime * 1000));
            _fadeOutTime = new TimeSpan(0, 0, 0, 0, (int)(this.Settings.FadeOutTime * 1000));
            _fadeInAnimation = new DoubleAnimation(1d, _fadeInTime);
            _fadeOutAnimation = new DoubleAnimation(0d, _fadeOutTime);
            _playTimer = new DispatcherTimer();
            _random = new Random();
            _playTimer.Tick += new EventHandler(playTimer_Tick);

            ApplySettings();
        }

        private void ApplySettings()
        {
            SlideImage.Stretch = this.Settings.Style;
            SlideImageSecond.Stretch = this.Settings.Style;
            _playTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)(this.Settings.Timer * 1000));
        }

        private void playTimer_Tick(object sender, EventArgs e)
        {
            ShowNextImage();
        }

        private void ShowNextImage()
        {
            try
            {
                int nextRandom = _random.Next(0, _imageFiles.Length - 1);

                BitmapImage bitImage = new BitmapImage();
                bitImage.BeginInit();
                bitImage.UriSource = new Uri(_imageFiles[nextRandom], UriKind.Absolute);
                bitImage.EndInit();

                if (_updateFirstImage)
                {
                    SlideImage.Source = bitImage;
                    _updateFirstImage = false;
                    SlideImage.BeginAnimation(Image.OpacityProperty, _fadeInAnimation);
                    SlideImageSecond.BeginAnimation(Image.OpacityProperty, _fadeOutAnimation);
                }
                else
                {
                    SlideImageSecond.Source = bitImage;
                    _updateFirstImage = true;
                    SlideImage.BeginAnimation(Image.OpacityProperty, _fadeOutAnimation);
                    SlideImageSecond.BeginAnimation(Image.OpacityProperty, _fadeInAnimation);
                }
            }
            catch { }
        }

        private void settingsMenu_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow(this.Settings);
            settingsWindow.ShowDialog();

            ApplySettings();
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

            ShowNextImage();
            _playTimer.Start();
        }

        private void TransitionMenu_Click(object sender, RoutedEventArgs e)
        {
            var transitionWindow = new ImageTransition();
            transitionWindow.Show();
        }
    }
}
