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
            var configFile = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + "config");
            string inputPath = "";
            string outputPath = "";
            string font = configFile.ReadLine().Trim(new char[] { '\n' }) + ",0";
            try
            {
                inputPath = args[0];
                outputPath = inputPath.Substring(0, inputPath.LastIndexOf('.')) + ".pdf";
            }
            catch(IndexOutOfRangeException)
            {
                inputPath = configFile.ReadLine().Trim(new char[] { '\n' });
                outputPath = configFile.ReadLine().Trim(new char[] { '\n' });
            }
            var root = new Document(new Rectangle(960, 540));
            var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
            var writer = PdfWriter.GetInstance(root, fs);
            var f = BaseFont.CreateFont(font, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            root.Open();
            var file = new StreamReader(inputPath);

            var titleColorRGB = new int[3] { 0x00, 0xd4, 0xff };
            var textColorRGB = new int[3] { 0x00, 0x00, 0x00 };

            while (!file.EndOfStream)
            {
                string text = file.ReadLine();
                Console.WriteLine(text);

                string category = checkContent(text);

                switch (category)
                {
                    case "text":
                        AddContents.AddText(writer.DirectContent, f, root, text, textColorRGB);
                        break;

                    case "title":
                        AddContents.AddTitle(writer.DirectContent, f, root, text.Substring(1), titleColorRGB);
                        break;

                    case "empty":
                        AddContents.AddTitle(writer.DirectContent, f, root, " ", textColorRGB);
                        break;

                    case "pass":
                        continue;

                    case "factor":
                    case "config":
                        string[] contents = checkConfigAndFactor(text);

                        switch (contents[0])
                        {
                            case "image":
                                try
                                {
                                    AddContents.AddImage(root, contents[1]);
                                }
                                catch (WebException)
                                {
                                    AddContents.AddText(writer.DirectContent, f, root, "Image error, file not found", new int[3] { 0xff, 0, 0 });
                                }
                                break;

                            case "titleColor":
                                titleColorRGB = colorToIntList(contents[1]);
                                break;

                            case "textColor":
                                textColorRGB = colorToIntList(contents[1]);
                                break;
                        }

                        break;

                }


            }

            root.Close();
            return;
        }


        static string checkContent(string text)
        {
            if (text == "") { return "pass"; }
            else if (text == "---") { return "empty"; }
            else if (text[0] == '#') { return "title"; }
            else if (System.Text.RegularExpressions.Regex.IsMatch(text, "[*:*]")) { return "factor"; }
            else if (System.Text.RegularExpressions.Regex.IsMatch(text, @"\(*:*\)")) { return "config"; }
            else { return "text"; }
        }

        static string[] checkConfigAndFactor(string text)
        {
            Console.WriteLine("check: " + text);

            var contents = new string[2];
            contents[0] = text.Substring(1, text.IndexOf(':') - 1);
            contents[1] = text.Substring(text.IndexOf(':') + 1);
            contents[1] = contents[1].Substring(0, contents[1].Length - 1);

            return contents;
        }

        static int[] colorToIntList(string colorcode)
        {


            if (colorcode[0] == '#')
            {
                return new int[3] {
                int.Parse(colorcode.Substring(1, 2),System.Globalization.NumberStyles.HexNumber),
                int.Parse(colorcode.Substring(3, 2),System.Globalization.NumberStyles.HexNumber),
                int.Parse(colorcode.Substring(5, 2),System.Globalization.NumberStyles.HexNumber)
                };
            }
            

            switch(colorcode)
            {
                case "BLACK":
                    return new int[3] { 0, 0, 0 };

                case "RED":
                    return new int[3] { 0xff, 0, 0 };

                case "GREEN":
                    return new int[3] { 0, 0xff, 0 };

                case "BLUE":
                    return new int[3] { 0, 0, 0xff };

                case "CYAN":
                    return new int[3] { 0, 0xff, 0xff };

                case "MAGENTA":
                    return new int[3] { 0xff, 0, 0xff };

                case "YELLOW":
                    return new int[3] { 0xff, 0xff, 0 };

                default:
                    goto case "BLACK";
            }
        }
    }
}

