using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace SuperSliderWin
{
    public class Settings
    {
        public static string SettingsPath = string.Empty;

        static Settings()
        {
            string settingsFileName = "SuperSliderWinSettings.xml";
            string specialFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folderPath = Path.Combine(specialFolder, "SuperSliderWin");

            if (Directory.Exists(folderPath) == false)
            {
                Directory.CreateDirectory(folderPath);
            }

            SettingsPath = Path.Combine(folderPath, settingsFileName);
        }

        public int Timer { get; set; } = 5;
        //public List<string> Folders { get; set; }
        public string Folders { get; set; } = "C:\\Images";
        public bool Shuffle { get; set; } = false;
        public Styles Style { get; set; } = Styles.Fit;
        public Positions Position { get; set; } = Positions.Center;

        public static void SaveSettings(Settings settings)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            using (TextWriter writer = new StreamWriter(SettingsPath))
            {
                serializer.Serialize(writer, settings);
            }
        }

        public static Settings LoadSettings()
        {
            if (File.Exists(SettingsPath) == false)
            {
                return new Settings();
            }

            XmlSerializer deserializer = new XmlSerializer(typeof(Settings));
            TextReader reader = new StreamReader(SettingsPath);
            object obj = deserializer.Deserialize(reader);
            var settings = (Settings)obj;
            reader.Close();

            return settings;
        }
    }

    public enum Styles
    {
        Fit,
        Fill,
        Original
    }

    public enum Positions
    {
        Top,
        Center,
        Bottom
    }
}
