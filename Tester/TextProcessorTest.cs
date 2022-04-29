using Microsoft.VisualStudio.TestTools.UnitTesting;
using basementOfKursach;
using System.IO;
using System.Windows;
using DocumentFormat.OpenXml;
using System.Threading;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;

namespace Tester
{
    [TestClass]
    public class TextProcessorTest
    {
        const string startText = "В настоящий момент Illumina предлагает продукты для секвенирования, генетического анализа, анализа однонуклеотидных полиморфизмов, экспрессии генов и белков, в том числе и с использованием технологии ДНК-микрочипов. 26 января 2007 года Illumina приобрела компанию Solexa, Inc, которая разрабатывает продукты для генетического анализа, в том числе для полного секвенирование генома, анализа экспрессии генов и анализа малых РНК. В конце 2014 году представила платформу для секвенирования полного генома человека с стоимостью реактивов около $1000 - Illumina X Ten.";

        [TestMethod]
        public void GetKeyLetter_TestString()
        {
            //Arrange
            string key = "genera";
            string text1 = "Qиod eraт dem0nstr@ndum";
            string expected1 = "generagenerageneragener";
            string actual = "";
            //Act
            for (int i = 0; i < text1.Length; i++)
            {
                actual += TextProcessor.GetKeyLetter(key, i);
            }
            //Assert
            Assert.AreEqual(expected1, actual, "Error in string formation!");
        }
        
        [TestMethod]
        public void KeyValidation_Test()
        {
            //Arrange 
            string[] strings = new string[]
            {
            "abcd2ef", "!gfjgkf", "12345",
            "]", "абсд", null, "", "  ", "fdfфаыфа",
            "ПРИВЕТ", "приВет", "HELLp"
            };
            string[] expected = new string[]
            {
                "", "", "",
                "", "абсд", "", "", "", "",
                "привет", "привет", ""
            };
            
            //Act
            for (int i = 0; i < strings.Length; i++)
            {
                strings[i] = TextProcessor.KeyValidation(strings[i]);
            }
            //Assert
            Assert.AreEqual(string.Join("!", expected), string.Join("!", strings), "Key hasn't been checked correctly");
        }
       
       
        

        [TestMethod]
        public void TextFromFileTXT_FileExists_test()
        {
            //Arrange 
            string testText = "Hello, I'm here TemPoRaLlY!\n \t !№;%:?*() Потом я буду удалён!";
            string tempPath = Path.Combine(Directory.GetCurrentDirectory(), "temp.txt");
            using (FileStream fs = new FileStream(tempPath, FileMode.Create))
            {
                using (StreamWriter sr = new StreamWriter(fs))
                {
                    sr.Write(testText);
                }
            }
            TextProcessor tp = new Encryptor("");
            // Act
            tp.TextFromFileTXT(tempPath);
            
            //Assert
            Assert.AreEqual(testText, tp.Text, $"The texts are different!");
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }  
        }
        [TestMethod]
        public void TextFromDOCX_FileExists_test()
        {
            //Arrange
            //        public  void TextFromFileDOCX(string path)
            string path = Path.Combine(Directory.GetCurrentDirectory(), "temp.docx");
            if (File.Exists(path)) return;
            using (WordprocessingDocument doc = WordprocessingDocument.Create(path, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = doc.AddMainDocumentPart();

                mainPart.Document = new Document(

                    new Body(

                        new Paragraph(

                            new Run(

                                new Text(startText)))));
            }
            TextProcessor processor = new Encryptor("");
            //Act
            processor.TextFromFileDOCX(path);
            string actText = processor.Text;

            //Assert
            Assert.AreEqual(startText, actText);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        [TestMethod]
        public void TextFromFileDOCX_FileNotExists_test()
        {
            TextProcessor processor = new Encryptor("");
            processor.TextFromFileDOCX("/anyfile.text");
        }
        [TestMethod]
        public void TextFromFileTXT_FileNotExists_test()
        {
            TextProcessor processor = new Encryptor("");
            processor.TextFromFileTXT("/anyfile.text");
        }
   
        
      [TestMethod]
        public void SaveToTXT_FileNotExist_test()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "tempFileSave.txt");
            Encryptor processor = new Encryptor(startText);
            string processedText = processor.Encrypt("");
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = path;
            string savedText = ""; 
            //Act
            TextProcessor.SaveToTXT(dlg.FileName, processedText);
            //assert
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    savedText = sr.ReadToEnd();
                }
            }
            Assert.AreEqual(processedText, savedText);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
            
        }
        [TestMethod]
        [Timeout(60000)]
        public void SaveToTXT_FileExist_test()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "tempFileSave.txt");
            File.Create(path).Close();

            Encryptor processor = new Encryptor(startText);
            string processedText = processor.Encrypt("");
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = path;
            string savedText = "";

            //Act
            TextProcessor.SaveToTXT(dlg.FileName, processedText);
            //assert
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    savedText = sr.ReadToEnd();
                }
            }
            Assert.AreEqual(processedText, savedText);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

        }
        [TestMethod]
        [ExpectedException(typeof(System.IO.FileNotFoundException))]
        public void SaveToTXT_FileisAlreadyOpen_test()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "tempFileSave.txt");
            using (FileStream fs1 = new FileStream(path, FileMode.Open))
            {
                Encryptor processor = new Encryptor(startText);
                string processedText = processor.Encrypt("");
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = path;
                string savedText = "";

                //Act
                TextProcessor.SaveToTXT(dlg.FileName, processedText);
                //assert
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        savedText = sr.ReadToEnd();
                    }
                }
                Assert.AreEqual(processedText, savedText);
            }
            if (File.Exists(path))
            {
                File.Delete(path);
            }

        }

        [TestMethod]
        public void SaveToDOCX_FileNotExists()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "tempFileSave.docx");
            Encryptor processor = new Encryptor(startText);
            string processedText = processor.Encrypt("а");
            
            string savedText = "";
            //Act
            TextProcessor.SaveToDOCX(path, processedText);
           
            using (WordprocessingDocument wordprocessing = WordprocessingDocument.Open(path, false))
            {
                if (wordprocessing.MainDocumentPart != null)
                {
                    Body body = wordprocessing.MainDocumentPart.Document.Body;

                    if (body != null)
                        savedText = body.InnerText.ToString();
                    wordprocessing.Dispose();
                }
            }
            
            //assert
            Assert.AreEqual(processedText, savedText, "Maybe, the problem's in the reading part?");
            
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        [TestMethod]
        public void SaveToDOCX_FileExists()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "tempFileSave.docx");
            var file = File.Open(path, FileMode.Create);
            file.Close();

            Encryptor processor = new Encryptor(startText);
            string processedText = processor.Encrypt("а");

            string savedText = "";
            //Act
            TextProcessor.SaveToDOCX(path, processedText);

            using (WordprocessingDocument wordprocessing = WordprocessingDocument.Open(path, false))
            {
                if (wordprocessing.MainDocumentPart != null)
                {
                    Body body = wordprocessing.MainDocumentPart.Document.Body;

                    if (body != null)
                        savedText = body.InnerText.ToString();
                    wordprocessing.Dispose();
                }
            }

            //assert
            Assert.AreEqual(processedText, savedText, "Maybe, the problem's in the reading part?");

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        [TestMethod]
       
        public void SaveToDOCX_FileisAlreadyOpen()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "tempFileSave.docx");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            var file = File.Open(path, FileMode.Create);
            

            Encryptor processor = new Encryptor(startText);
            string processedText = processor.Encrypt("а");
            

            //Act
            TextProcessor.SaveToDOCX(path, processedText);
            file.Close();
            string savedText =  File.ReadAllText(path);
            Assert.AreNotEqual(savedText, processedText, "Saved text shouldn't be same");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

        }


    }
}