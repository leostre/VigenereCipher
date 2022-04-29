using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
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

namespace basementOfKursach
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
         public static MainWindow window;
         string originalText = "";
         string processedText = "";
         string key = "";
         TextProcessor processor = new Decryptor("");

        public event PropertyChangedEventHandler? PropertyChanged;

        public  string OriginalText 
        {
            get { return originalText; }
            set { originalText = value; 
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OriginalText))); 
            }
        }
        public  string ProcessedText
        {
            get { return processedText; }
            set { processedText = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProcessedText))); }
        }
        public  string Key 
        {
            get { return key; } 
            set { key = TextProcessor.KeyValidation(value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProcessedText)));
            } 
        }

        public MainWindow()
        {
            InitializeComponent();
            window = this;
            this.Key = "скорпион";
            CurrentKey.Text = CurrentKey.Text.Insert(0, Key);
        }

        private void Encrypt_Button_Click(object sender, RoutedEventArgs e)
        {
            var encryptor = new Encryptor(OriginalText);
            processor = encryptor;
            ProcessedText = encryptor.Encrypt(Key);
            
            ProcessedTextBlock.Text = processedText;
        }

        private void Decrypt_Button_Click(object sender, RoutedEventArgs e)
        {
            var decryptor = new Decryptor(OriginalText);
            processor = decryptor;
            ProcessedText = decryptor.Decrypt(Key);
            ProcessedTextBlock.Text = processedText;
        }

        private  void Download_Text_Button_Click(object sender, RoutedEventArgs e)
        {
            OriginalText = "";
            processor.DownloadFile();
            OriginalText = OriginalTextBlock.Text.Insert(0, processor.Text);

        }

        private  void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            processor.SaveToFile();

        }

        private void Info_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string path;
            string path1 = @"../../../readme.txt";
            string path2 = @"../../../../readme.txt";
            if (File.Exists(path1))
            {
                path = path1;
            }
            else
            {
                path = path2;
            }

            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        MessageBox.Show(sr.ReadToEnd());
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}
