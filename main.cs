using System;
using System.IO;
using System.Net;
using iTextSharp.text;
using iTextSharp.text.pdf;


namespace pdfSeiseiKun
{
    class Program
    {
        static void Main(string[] args)
        {
            var root = new Document(new Rectangle(960,540));
            var fs = new FileStream(@"files\presen.pdf", FileMode.Create, FileAccess.Write);
            var writer = PdfWriter.GetInstance(root, fs);
            var f = BaseFont.CreateFont(@"c:\windows\fonts\UDDigiKyokashoN-R.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            root.Open();

            var file = new StreamReader(@"files\presen.txt");

            while (!file.EndOfStream)
            {
                string text = file.ReadLine();
                Console.WriteLine(text);

                if (text == "") { continue; }
                else if (text == "---") { AddContents.AddTitle(writer.DirectContent, f, root, " "); }
                else if (text[0] == '#') { AddContents.AddTitle(writer.DirectContent, f, root, text.Substring(1)); }
                else if (System.Text.RegularExpressions.Regex.IsMatch(text, "[[^:]*:"))
                {
                    string config = text.Substring(1, text.IndexOf(':') - 1);
                    string content = text.Substring(text.IndexOf(':') + 1);
                    content = content.Substring(0, content.Length - 1);

                    switch (config)
                    {
                        case "image":
                            {
                                try
                                {
                                    AddContents.AddImage(root, content);
                                }catch(WebException)
                                {
                                    AddContents.AddText(writer.DirectContent, f, root, "Image error, file not found");
                                }
                                break;
                            }
                        default:
                            AddContents.AddText(writer.DirectContent, f, root, content);
                            break;
                            
                    }



                }

                else { AddContents.AddText(writer.DirectContent, f, root, text); }

            }

            root.Close();
            return;
        }

        
    }
}

