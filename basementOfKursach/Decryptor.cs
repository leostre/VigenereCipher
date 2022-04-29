namespace basementOfKursach
{
    public class Decryptor : TextProcessor
    {
        public string Decrypt(string key)
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
            string decrypted = "";
            
                    int countNotAlphabet = 0;
                    for (int i = 0; i < text.Length; i++)
                    {

                        char c = text[i];


                        if (!(char.IsLetter(c) && alphabet.Contains(char.ToLower(c))))
                        {
                            decrypted += c;
                            countNotAlphabet++;
                            continue;
                        }
                        c = char.ToLower(c);
                        int indexC = (alphabet.IndexOf(c) + n - alphabet.IndexOf(GetKeyLetter(key, i - countNotAlphabet))) % n;
                        decrypted += alphabet[indexC];
                    }
            processedText = decrypted;
            return decrypted;

        }
        public Decryptor(string text) : base(text) { }

    }
}
