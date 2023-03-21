using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using Xabe.FFmpeg;
using System.Threading.Tasks;

namespace easy_convert
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        Dictionary<string, bool> allowed_types = new Dictionary<string, bool>()
        {
            [".mp4"] = true
        };

        bool get_type(string file)
        {
            string extension = System.IO.Path.GetExtension(file);

            foreach (var allowed_types in allowed_types)
            {
                if (allowed_types.Key == extension.ToLower())
                    return true;
            }

            return false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog directory = new OpenFileDialog();
            directory.Multiselect = false;

            if (directory.ShowDialog(this) == true)
                if (get_type(directory.FileName))
                    file_source.Text = directory.FileName;
                else
                {
                    MessageBox.Show("Выбрано не видео!");
                    return;
                }
        }

        string get_name(string name = "")
        {
            if (name == "" && file_type.Text != "")
                name = file_type.Text;

            if (name == "")
                return "default";
            else
            {
                int i = 0;

                if (File.Exists(name))
                {
                    do
                    {
                        i++;
                    }
                    while (File.Exists(name + "_" + i + "." + file_type.Text.ToLower()));
                }

                if (i > 0)
                    return name + "_" + i + "." + file_type.Text.ToLower();

                return name + "." + file_type.Text.ToLower();
            }
        }

        private void directory_click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog directory = new OpenFileDialog();
            directory.ValidateNames = false;
            directory.CheckFileExists = false;
            directory.CheckPathExists = true;
            directory.Multiselect = false;
            directory.FileName = "[Выберите папку]";

            if (directory.ShowDialog(this) == true)
                directory_textbox.Text = System.IO.Path.GetDirectoryName(directory.FileName);
        }

        private async void start_click(object sender, RoutedEventArgs e)
        {
            string extension;
            if (file_type.Text == "")
            {
                MessageBox.Show("Не выбран формат для конвертации!");
                return;
            }
            else
                extension = file_type.Text;

            if (file_source.Text == "")
            {
                MessageBox.Show("Не выбран файл для конвертации!");
                return;
            }

            if (directory_textbox.Text == "")
            {
                MessageBox.Show("Не выбран конечная папка!");
                return;
            }

            string name = get_name();
            string inputFile = file_source.Text;
            string outputFile = directory_textbox.Text + "/" + name;

            var conversion = FFmpeg.Conversions.FromSnippet.ExtractAudio(inputFile, outputFile);
            await conversion.Start();
        }
    }
}
