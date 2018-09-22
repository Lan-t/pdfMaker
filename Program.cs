using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace presen
{
    class Program
    {
        static void Main(string[] args)
        {
            var root = new Document(new Rectangle(960,540));
            var fs = new FileStream("presen.pdf", FileMode.Create, FileAccess.Write);
            var writer = PdfWriter.GetInstance(root, fs);
            var f = BaseFont.CreateFont(@"c:\windows\fonts\UDDigiKyokashoN-R.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            root.Open();

            var file = new StreamReader("presen.txt");

            while (!file.EndOfStream)
            {
                string text = file.ReadLine();
                Console.WriteLine(text);

                if (text == "") { continue; }
                else if (text == "---") { root.NewPage(); }
                else if (text[0] == '#') { AddTitle(writer.DirectContent, f, root, text.Substring(1)); }
                else { Addtext(writer.DirectContent, f, root, text); }

            }

            root.Close();
            return;
        }

        static void AddTitle(PdfContentByte master, BaseFont f,Document root,  string title)
        {
            root.NewPage();
            master.SetFontAndSize(f, 70);
            master.SetCMYKColorFill(cyan: 255, magenta: 0, yellow: 0, black: 0);

            master.BeginText();
            master.ShowTextAligned(Element.ALIGN_CENTER, title, 480, 250, 0);
            master.EndText();
        }

        static void Addtext(PdfContentByte master, BaseFont f, Document root, string content)
        {
            
            int y = 250;
            int x = 50;

            string[] contents = content.Split(new char[] { '\n' });

            for (int i = 0; i < contents.Length; i++)
            {
                root.NewPage();
                master.SetFontAndSize(f, 50);
                master.SetCMYKColorFill(0, 0, 0, black: 255);
                master.BeginText();
                master.ShowTextAligned(Element.ALIGN_LEFT, contents[i], x, y, 0);
                master.EndText();

            }


        }
    }
}
