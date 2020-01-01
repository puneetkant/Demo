using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SuperSliderWin
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public Settings Settings { get; set; }

        public SettingsWindow(Settings settings)
        {
            InitializeComponent();
            this.Settings = settings;
            UpdateUIFromSettings(settings);
        }

        private void UpdateUIFromSettings(Settings settings)
        {
            TimerTextBox.Text = settings.Timer.ToString();
            FadeInTimeTextBox.Text = settings.FadeInTime.ToString();
            FadeOutTimeTextBox.Text = settings.FadeOutTime.ToString();
            FolderTextBox.Text = settings.Folders;
            ShuffleCheckbox.IsChecked = settings.Shuffle;
            SyleCombobox.SelectedValue = settings.Style.ToString();
            PositionCombobox.SelectedValue = settings.Position;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateSettingsFromUI();
            Settings.SaveSettings(this.Settings);
            this.Close();
        }

        private void UpdateSettingsFromUI()
        {
            this.Settings.Timer = double.Parse(TimerTextBox.Text);
            this.Settings.FadeInTime = double.Parse(FadeInTimeTextBox.Text);
            this.Settings.FadeOutTime = double.Parse(FadeOutTimeTextBox.Text);
            this.Settings.Folders = FolderTextBox.Text;
            this.Settings.Shuffle = (bool)ShuffleCheckbox.IsChecked;
            this.Settings.Style = (Stretch)Enum.Parse(typeof(Stretch), SyleCombobox.SelectionBoxItem.ToString(), true);
            this.Settings.Position = (Positions)Enum.Parse(typeof(Positions), PositionCombobox.SelectionBoxItem.ToString(), true);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DefalutButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateUIFromSettings(new Settings());
        }
    }
}
