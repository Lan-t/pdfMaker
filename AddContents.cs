using iTextSharp.text;
using iTextSharp.text.pdf;
using System;

namespace pdfSeiseiKun
{
    public static class AddContents
    {
        public static void AddTitle(PdfContentByte master, BaseFont f, Document root, string title, int[] colorRGB)
        {
            root.NewPage();
            master.SetFontAndSize(f, 70);
            master.SetColorFill(new BaseColor(colorRGB[0], colorRGB[1], colorRGB[2]));

            master.BeginText();
            master.ShowTextAligned(Element.ALIGN_LEFT, title, 50, 270, 0);
            master.EndText();
        }

        public static void AddText(PdfContentByte master, BaseFont f, Document root, string content, int[] colorRGB)
        {
            int x = 50;
            int y = 223;

            string[] contents = content.Split(new char[] { '\\' });
            y += contents.Length * 27;

            root.NewPage();

            for (int i = 0; i < contents.Length; i++)
            {

                master.SetFontAndSize(f, 50);
                master.SetColorFill(new BaseColor(colorRGB[0], colorRGB[1], colorRGB[2]));
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

        /*
        public static void AddLink(PdfContentByte master, BaseFont f, Document root, string URL, string label)
        {
            var chunk = new Chunk(label);
            chunk.SetAnchor(URL);

            root.NewPage();
            root.Add(chunk);
            }
        */


        public static void AddPdf(PdfWriter writer, string filename, BaseFont f, Document root)
        {
            var reader = new PdfReader(filename);
            for (int i = 1; ; i++)
            {
                try     // pageの存在確認
                {
                    reader.GetPageSize(i);
                }
                catch(System.NullReferenceException e)
                {
                    break;
                }

                writer.NewPage();
                AddText(writer.DirectContent, f, root, " ", new int[] { 0, 0, 0 });
                writer.DirectContent.AddTemplate(writer.GetImportedPage(reader, i), 0, 0);
            }
        }
    }
}
