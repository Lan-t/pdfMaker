using iTextSharp.text;
using iTextSharp.text.pdf;
using System;

namespace pdfSeiseiKun
{
    public static class AddContents
    {
        public static void AddTitle(PdfContentByte master, BaseFont f, Document root, string title)
        {
            root.NewPage();
            master.SetFontAndSize(f, 70);
            master.SetCMYKColorFill(cyan: 255, magenta: 0, yellow: 0, black: 0);

            master.BeginText();
            master.ShowTextAligned(Element.ALIGN_LEFT, title, 50, 270, 0);
            master.EndText();
        }

        public static void AddText(PdfContentByte master, BaseFont f, Document root, string content)
        {
            int x = 50;
            int y = 223;

            string[] contents = content.Split(new char[] { '\\' });
            y += contents.Length * 27;

            root.NewPage();

            for (int i = 0; i < contents.Length; i++)
            {
                master.SetFontAndSize(f, 50);
                master.SetCMYKColorFill(0, 0, 0, black: 255);
                master.BeginText();
                master.ShowTextAligned(Element.ALIGN_LEFT, contents[i], x, y, 0);
                master.EndText();
                y -= 55;

            }
        }

        public static void AddImage(Document root, string path)
        {
            root.NewPage();
            Image image = Image.GetInstance(path);
            float x = 0, y = 0;
            if(image.Right / 960 < image.Top / 540)
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
        }
    }
}
