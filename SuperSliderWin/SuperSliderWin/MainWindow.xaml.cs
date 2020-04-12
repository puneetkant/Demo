using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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
        private static DoubleAnimation _fadeInAnimation;
        private static DoubleAnimation _fadeOutAnimation;
        private static FileStream _fileStream;
        private static Stack<FileStream> _previousFileStreams;

        public Settings Settings { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            this.Settings = Settings.LoadSettings();
            _playTimer = new DispatcherTimer();
            _random = new Random();
            _playTimer.Tick += new EventHandler(playTimer_Tick);
            _previousFileStreams = new Stack<FileStream>();

            ApplySettings();
        }

        private void ApplySettings()
        {
            SlideImage.Stretch = this.Settings.Style;
            SlideImage.StretchDirection = StretchDirection.Both;
            //SlideImageSecond.Stretch = this.Settings.Style;
            //SlideImageSecond.StretchDirection = StretchDirection.Both;
            var fadeInTime = new TimeSpan(0, 0, 0, 0, (int)(this.Settings.FadeInTime * 1000));
            var fadeOutTime = new TimeSpan(0, 0, 0, 0, (int)(this.Settings.FadeOutTime * 1000));
            _fadeInAnimation = new DoubleAnimation(1d, fadeInTime);
            _fadeOutAnimation = new DoubleAnimation(0d, fadeOutTime);
            _playTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)(this.Settings.Timer * 1000));
        }

        private void playTimer_Tick(object sender, EventArgs e)
        {
            ShowNextImage();
        }

        private async void ShowNextImage()
        {
            try
            {
                if (_imageFiles == null || _imageFiles.Length == 0)
                    return;
                string imageName = await GetNextImageName();
                ImageNameTextBlock.Text = imageName;

                //double snugContentWidth = this.ActualWidth;
                //double snugContentHeight = this.ActualHeight;

                //var horizontalBorderHeight = SystemParameters.ResizeFrameHorizontalBorderHeight;
                //var verticalBorderWidth = SystemParameters.ResizeFrameVerticalBorderWidth;
                //var captionHeight = SystemParameters.CaptionHeight;

                //var clientWidth = snugContentWidth + 2.0 * verticalBorderWidth;
                //var clientHeight = snugContentHeight + captionHeight + 2.0 * horizontalBorderHeight;
                PushPreviousFileStream();
                BitmapImage bitmapImage = await CreateBitmapFromImageName(imageName);

                //MainStackPanel.Height = clientHeight;
                //MainStackPanel.Width = clientWidth;


                if (_updateFirstImage)
                {
                    //SlideImage.Width = (int)clientWidth;
                    //SlideImage.Height = (int)clientHeight;
                    SlideImage.Source = bitmapImage;
                    _updateFirstImage = false;

                    SlideImage.BeginAnimation(Image.OpacityProperty, _fadeInAnimation);
                    SlideImageSecond.BeginAnimation(Image.OpacityProperty, _fadeOutAnimation);
                }
                else
                {
                    //SlideImageSecond.Width = (int)clientWidth;
                    //SlideImageSecond.Height = (int)clientHeight;

                    SlideImageSecond.Source = bitmapImage;
                    _updateFirstImage = true;
                    SlideImage.BeginAnimation(Image.OpacityProperty, _fadeOutAnimation);
                    SlideImageSecond.BeginAnimation(Image.OpacityProperty, _fadeInAnimation);
                }
                //MainStackPanel.Height = double.NaN;
                //MainStackPanel.Width = double.NaN;

                _fadeOutAnimation.Completed += (o, e) =>
                {
                    DisposePreviousFileStreams();
                };
            }
            catch (Exception) { }
        }

        private static void PushPreviousFileStream()
        {
            if (_fileStream != null)
            {
                _previousFileStreams.Push(_fileStream);
            }
        }

        private static async Task<BitmapImage> CreateBitmapFromImageName(string imageName)
        {
            BitmapImage bitImage = new BitmapImage();
            _fileStream = File.OpenRead(imageName);
            bitImage.CacheOption = BitmapCacheOption.OnLoad;
            bitImage.BeginInit();
            bitImage.StreamSource = _fileStream;
            //bitImage.DecodePixelWidth = (int)(clientWidth);  
            //bitImage.DecodePixelHeight = (int)(clientHeight - MainMenuBar.RenderSize.Height);
            bitImage.EndInit();
            bitImage.Freeze();
            return bitImage;
        }

        private static async Task<string> GetNextImageName()
        {
            int nextRandom = _random.Next(0, _imageFiles.Length - 1);
            var imageName = _imageFiles[nextRandom];
            return imageName;
        }

        private static async void DisposePreviousFileStreams()
        {
            lock (_previousFileStreams)
            {
                while (_previousFileStreams.Count > 0)
                {
                    var previousFileStream = _previousFileStreams.Pop();

                    if (previousFileStream != null)
                    {
                        previousFileStream.Close();
                        previousFileStream.Dispose();
                        previousFileStream = null;
                    }
                }
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
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
            UpdateImageFiles();
            ShowNextImage();
            _playTimer.Start();
        }

        private void UpdateImageFiles()
        {
            if (Directory.Exists(this.Settings.Folders) == false)
            {
                _imageFiles = null;
                return;
            }

            _imageFiles = Directory.GetFiles(this.Settings.Folders, "*.*", SearchOption.AllDirectories);
        }

        private void ShowRandomImageMenu_Click(object sender, RoutedEventArgs e)
        {
            UpdateImageFiles();
            ShowNextImage();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FullScreenMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = (MenuItem)sender;
            if (menu.Header.ToString() == "[ ]")
            {
                this.WindowStyle = WindowStyle.None;
                this.ResizeMode = ResizeMode.NoResize;
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                this.WindowState = WindowState.Maximized;

                menu.Header = "=";
            }
            else
            {
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.ResizeMode = ResizeMode.CanResize;
                //this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                this.WindowState = WindowState.Normal;

                menu.Header = "[ ]";
            }
        }
    }
}
