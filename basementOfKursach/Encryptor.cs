namespace basementOfKursach
{
    public class Encryptor : TextProcessor
    {
        public string Encrypt(string key)
        {
            if (key == "" || key == null)
            {
                key = "скорпион";
                if (MainWindow.window != null && MainWindow.window.CurrentKey != null)
                {
                    MainWindow.window.CurrentKey.Text = key; MainWindow.window.Key = "скорпион";
                }
            }
            short n = (short)alphabet.Length;
            string encrypted = "";
            int countNotAlphabet = 0;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
               //if (char.IsLetter(c)) c = char.ToLower(c);

                if (!(char.IsLetter(c) && alphabet.Contains(char.ToLower(c))))
                {
                    encrypted += c;
                    countNotAlphabet++;
                    continue;
                }
                c = char.ToLower(c);
                int indexC = (alphabet.IndexOf(c) + alphabet.IndexOf(GetKeyLetter(key, i - countNotAlphabet))) % n;
                encrypted += alphabet[indexC];
                
            }
            processedText = encrypted;
            return encrypted;
            
        }
        public Encryptor(string text) : base(text) { }
    }
}
