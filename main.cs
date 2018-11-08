using System;
using System.IO;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text.RegularExpressions;

namespace pdfSeiseiKun
{
    class Program
    {
        static void Main(string[] args)
        {
            // font, save-directory config file
            var configFile = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + "config");
            // input file(txt) path
            string inputPath = "";
            // output file(pdf) path
            string outputPath = "";
            // for insert file(image, pdf) path
            string directory = "";
            // read config of font
            string font = configFile.ReadLine().Trim(new char[] { '\n' }) + ",0";
            // if drag-and-drop txet file, read it
            try
            {
                // set path
                inputPath = args[0];
                outputPath = inputPath.Substring(0, inputPath.LastIndexOf('.')) + ".pdf";
                directory = Path.GetDirectoryName(inputPath) + "/";
            }
            // else double-click exe to start, read config and set in-out path
            catch(IndexOutOfRangeException)
            {
                // set path
                inputPath = configFile.ReadLine().Trim(new char[] { '\n' });
                outputPath = configFile.ReadLine().Trim(new char[] { '\n' });
                directory = Path.GetDirectoryName(inputPath) + "/";
            }
            // open document  aspect 960*540
            var root = new Document(new Rectangle(960, 540));
            var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
            var writer = PdfWriter.GetInstance(root, fs);
            root.Open();
            // open font file
            var f = BaseFont.CreateFont(font, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            // open text file
            var file = new StreamReader(inputPath);
            // set font color(title, other-text)
            var titleColorRGB = new int[3] { 0x00, 0xd4, 0xff };
            var textColorRGB = new int[3] { 0x00, 0x00, 0x00 };


            /*** start main ***/

            // for itemize text
            string itemizeTmp = "";
            // start read text file
            while (!file.EndOfStream)
            {
                // read text
                string text = file.ReadLine();
                // check content type.  category is type string
                string category = checkContent(text);

                //  switch action for category
                switch (category)
                {
                    case "text":
                        AddContents.AddText(writer.DirectContent, f, root, text, textColorRGB);
                        break;

                    case "title":
                        AddContents.AddTitle(writer.DirectContent, f, root, text.Substring(2), titleColorRGB);
                        break;

                    case "itemize":
                        // set text for itemize.  '\' is for new-line char in 'AddText()'
                        // if end itemize(one or more blank-line in text), goto case pass and write text
                        itemizeTmp += "・ " + text.Substring(2) + @"\";
                        break;

                    case "empty":
                        // write blank to make blank slide
                        AddContents.AddTitle(writer.DirectContent, f, root, " ", textColorRGB);
                        break;

                    case "pass":
                        // if end itemize, write it
                        if (itemizeTmp != "")
                        {
                            AddContents.AddText(writer.DirectContent, f, root, itemizeTmp.TrimEnd(new char[] { '\\' }), textColorRGB);
                            // reset tmp
                            itemizeTmp = "";
                        }
                        break;

                    // if insert file, or set config
                    case "factor":
                    case "config":
                        // get text for config
                        // string[0] is type
                        // conten[1] is content
                        string[] contents = checkConfigAndFactor(text);

                        switch (contents[0])
                        {
                            case "image":
                                try
                                {
                                    if (Regex.IsMatch(contents[1], @"https?://.*")  // URL
                                        || Regex.IsMatch(contents[1], @"^[c-zC-Z]:\.*") // absolute path (win)
                                        || Regex.IsMatch(contents[1], @"^/.*")) // absolute path (unix)
                                    {
                                        AddContents.AddImage(root, contents[1]);
                                    }
                                    // if relative path, add insert file directory
                                    else
                                    {
                                        AddContents.AddImage(root, directory + contents[1]);
                                    }
                                }
                                // if cant find image, write massege "Image error, file not found"
                                catch (WebException)
                                {
                                    AddContents.AddText(writer.DirectContent, f, root, "Image error, file not found", new int[3] { 0xff, 0, 0 });
                                }
                                break;

                            case "pdf":
                                AddContents.AddPdf(writer, directory + contents[1], f, root);
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

            try
            {
                root.Close();
            }
            // if document has no page, add blank to make blanc pdf
            catch
            {
                AddContents.AddTitle(writer.DirectContent, f, root, " ", textColorRGB);
                root.Close();
            }
            return;
        }


        static string checkContent(string text)
        {
            // { content type , regular expression text }
            var type = new string[,]
            {
                // { "pass",@"^\s*$" },   // finaly
                { "empty", @"^---$"},
                { "title", @"^#\s.+" },
                { "itemize", @"^-\s.+" },
                { "factor", @"^(\[)([^\[\]]+)(\s*:\s*)([^\[\]]+)(\])$" },
                { "config", @"^(\()([^\(\)]+)(\s*:\s*)([^\(\)]+)(\))$" },
                { "text", @"\S+" }
            };
            
            // check text and return
            for(int i = 0; i < type.Length/2; i++)
            {
                if (Regex.IsMatch(text, type[i, 1]))
                {
                    return type[i, 0];
                }
            }
            return "pass";
        }

        static string[] checkConfigAndFactor(string text)
        {
            var match = Regex.Match(text, @"^([\[\(])([^\[\(\]\)]+)(\s*:\s*)([^\[\(\]\)]+)([\]\)])$");

            var contents = new string[2];

            // [type : content]
            contents[0] = match.Groups[2].Value;
            contents[1] = match.Groups[4].Value;
            
            return contents;
        }

        static int[] colorToIntList(string colorcode)
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

