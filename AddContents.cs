using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text.RegularExpressions;

namespace pdfSeiseiKun
{
    public class Manegement
    {
        PdfContentByte master { get; set; }
        PdfWriter writer { get; set; }
        Document root { get; set; }
        BaseFont font { get; set; }

        public string subtitle { get; set; }

        public int[] titleColor { get; set; } 
        public int[] textColor { get; set; }


        public Manegement(
            PdfWriter writer,
            Document root,
            string font,
            int[] titleColor,
            int[] textColor
            )
        {
            this.master = writer.DirectContent;
            this.writer = writer;
            this.root = root;
            this.font = BaseFont.CreateFont(font, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            this.titleColor = titleColor;
            this.textColor = textColor;
            this.subtitle = "";
        }


        public void AddTitle(string title)
        {
            root.NewPage();
            master.SetFontAndSize(font, 80);
            master.SetColorFill(new BaseColor(titleColor[0], titleColor[1], titleColor[2]));

            master.BeginText();
            master.ShowTextAligned(Element.ALIGN_LEFT, title, 50, 270, 0);
            master.EndText();

            AddSubTitle();
        }

        public void AddText(string content)
        {
            int x = 50;
            int y = 223;

            string[] contents = content.Split(new char[] { '\\' });
            y += contents.Length * 27;

            root.NewPage();

            for (int i = 0; i < contents.Length; i++)
            {

                master.SetFontAndSize(font, 50);
                master.SetColorFill(new BaseColor(textColor[0], textColor[1], textColor[2]));
                master.BeginText();
                master.ShowTextAligned(Element.ALIGN_LEFT, contents[i], x, y, 0);
                master.EndText();
                y -= 55;

            }

            AddSubTitle();
        }

        private void AddSubTitle()
        {
            int x = 930;
            int y = 470;

            master.SetFontAndSize(font, 50);
            master.SetColorFill(new BaseColor(titleColor[0], titleColor[1], titleColor[2]));
            master.BeginText();
            master.ShowTextAligned(Element.ALIGN_RIGHT, subtitle, x, y, 0);
            master.EndText();

        }

        public void AddImage(string path)
        {
            root.NewPage();
            Image image = Image.GetInstance(path);
            float x = 0, y = 0;
            if (image.Right / 960 < image.Top / 540)
            {
                y = 500;
                x = image.Right * (500f / image.Top);

                image.ScaleAbsolute(new Rectangle(x, y));
            }
            else
            {
                x = 900;
                y = image.Top * (900f / image.Right);

                image.ScaleAbsolute(new Rectangle(x, y));
            }

            image.SetAbsolutePosition((960 - x) / 2, (540 - y) / 2);
            root.Add(image);

            AddSubTitle();
        }
        

        public void AddPdf(string filename)
        {
            var reader = new PdfReader(filename);
            for (int i = 1; ; i++)
            {
                try     // pageの存在確認
                {
                    reader.GetPageSize(i);
                }
                catch (System.NullReferenceException e)
                {
                    break;
                }

                writer.NewPage();
                AddText(" ");
                writer.DirectContent.AddTemplate(writer.GetImportedPage(reader, i), 0, 0);

                AddSubTitle();
            }
        }




        public static string checkContent(string text)
        {
            // { content type , regular expression text }
            var type = new string[,]
            {
                // { "pass",@"^\s*$" },   // finaly
                { "empty", @"^---$"},
                { "title", @"^#\s.+" },
                { "subtitle", @"^##\s.+" },
                { "deletesubtitle", @"^##$" },
                { "multiplelines", "\"\"\"" },
                { "itemize", @"^-\s.+" },
                { "factor", @"^(\[)([^\[\]]+)(\s*:\s*)([^\[\]]+)(\])$" },
                { "config", @"^(\()([^\(\)]+)(\s*:\s*)([^\(\)]+)(\))$" },
                { "text", @"\S+" }
            };

            // check text and return
            for (int i = 0; i < type.Length / 2; i++)
            {
                if (Regex.IsMatch(text, type[i, 1]))
                {
                    return type[i, 0];
                }
            }
            return "pass";
        }

        public static string[] checkConfigAndFactor(string text)
        {
            var match = Regex.Match(text, @"^([\[\(])([^\[\(\]\)]+)(\s*:\s*)([^\[\(\]\)]+)([\]\)])$");

            var contents = new string[2];

            // [type : content]
            contents[0] = match.Groups[2].Value;
            contents[1] = match.Groups[4].Value;

            return contents;
        }

        public static int[] colorToIntList(string colorcode)
        {

            // read hex-color-code
            if (colorcode[0] == '#')
            {
                var code = Regex.Match(colorcode, @"\#([0-9]{2})([0-9]{2})([0-9]{2})");

                return new int[3] {
                int.Parse(code.Groups[1].Value,System.Globalization.NumberStyles.HexNumber),
                int.Parse(code.Groups[2].Value,System.Globalization.NumberStyles.HexNumber),
                int.Parse(code.Groups[3].Value,System.Globalization.NumberStyles.HexNumber)
                };
            }


            switch (colorcode)
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
