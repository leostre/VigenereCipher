using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.ComponentModel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace basementOfKursach
{
    public abstract class TextProcessor //: INotifyPropertyChanged //: Window
    {
        protected const string alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
        protected string text;
        protected static string processedText = "";

        public string Text
        { get { return text; } set { text = value; } }
        public TextProcessor(string text) => this.text = text;
        public void TextFromFileTXT(string path)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        text = sr.ReadToEnd();

                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }   
        }
        public string DownloadFile()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Title = "Выберите файл";
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            dlg.Filter = "Текстовый документ |*.txt|Microsoft Office Word|*.docx";
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                try
                {
                    if (dlg.FileName.EndsWith(".docx"))
                    {
                        TextFromFileDOCX(dlg.FileName);
                    }
                    else if (dlg.FileName.EndsWith(".txt"))
                    {
                        TextFromFileTXT(dlg.FileName);
                    }
                    else MessageBox.Show("Формат файла не поддерживается!");
                }
                catch (IOException e) when ((e.HResult & 0x0000FFFF) == 32)
                {
                    MessageBox.Show("В данный момент файл используется другой программой. Закройте её и попробуйте заново.");
                }
                catch (Exception ex){ MessageBox.Show("Упс! Ошибка сохранения! " + ex.Message ); }
            }
            return dlg.FileName;

        }
        public void TextFromFileDOCX(string path)
        {
            try
            {
                using (WordprocessingDocument wordprocessing = WordprocessingDocument.Open(path, true))
                {
                    Body body = wordprocessing.MainDocumentPart.Document.Body;
                    text = body.InnerText.ToString();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

        }

        
        public void SaveToFile()
        {
            var dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Title = "Сохранение файла";
            dlg.DefaultExt = ".txt";
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            dlg.AddExtension = true;           
            
            dlg.Filter = "Текстовый документ |*.txt|Microsoft Office Word|*.docx";
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                try
                {
                   if (dlg.FileName.EndsWith(".docx"))
                    {
                        SaveToDOCX(dlg.FileName, processedText);
                    }
                   else
                    {
                        SaveToTXT(dlg.FileName, processedText);
                    }

                }
                catch (IOException e) //when ((e.HResult & 0x0000FFFF) == 32)
                {
                    MessageBox.Show("В данный момент файл используется другой программой. Закройте её и попробуйте заново.");
                }
                catch (Exception ex) { MessageBox.Show("Ошибка сохранения! " + ex.Message); }
                finally { MessageBox.Show("Документ успешно сохранён!!"); }
            }
        }
        public static void SaveToTXT(string path, string text)
        {
           
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.Write(text); // ASYNC???
                    }
                }
            }
            catch (Exception ex){ MessageBox.Show(ex.Message); }
        }
        public static void SaveToDOCX(string path, string text)
        {
            
            try
            {
                using (WordprocessingDocument doc = WordprocessingDocument.Create(path, WordprocessingDocumentType.Document))

                {

                    //// Creates the MainDocumentPart and add it to the document (doc)

                    MainDocumentPart mainPart = doc.AddMainDocumentPart();

                    mainPart.Document = new Document(

                        new Body(

                            new Paragraph(

                                new Run(

                                    new Text(text)))));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static char GetKeyLetter(string key, int originalIndex)
        {
            return key[(int)(originalIndex % key.Length)];
        }
        
        public static string KeyValidation(string key)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key)) return "";
            
            if (key.ToLower().All(x => alphabet.Contains(x)))
            {
                return key.ToLower();
            }
            else MessageBox.Show("Ключ должен содержать только буквы русского алфавита: " + alphabet);
            return "";
            
        }

    }
}
