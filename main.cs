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
    class PdfSeiseiKun
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

            // set class instance
            var manegemant = new Manegement(
                writer,
                root,
                f,
                new int[3] { 0x00, 0xd4, 0xff }, // cyan
                new int[3] { 0x00, 0x00, 0x00 } // black
                );

            /*** start main ***/

            // for itemize text
            string itemizeTmp = "";
            // start read text file
            while (!file.EndOfStream)
            {
                // read text
                string text = file.ReadLine();
                // check content type.  category is type string
                string category = Manegement.checkContent(text);

                //  switch action for category
                switch (category)
                {
                    case "text":
                        manegemant.AddText(text);
                        break;

                    case "title":
                        manegemant.AddTitle(text.Substring(2));
                        break;

                    case "itemize":
                        // set text for itemize.  '\' is for new-line char in 'AddText()'
                        // if end itemize(one or more blank-line in text), goto case pass and write text
                        itemizeTmp += "・ " + text.Substring(2) + @"\";
                        break;

                    case "empty":
                        // write blank to make blank slide
                        manegemant.AddText(" ");
                        break;

                    case "pass":
                        // if end itemize, write it
                        if (itemizeTmp != "")
                        {
                            manegemant.AddText(itemizeTmp.TrimEnd(new char[] { '\\' }));
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
                        string[] contents = Manegement.checkConfigAndFactor(text);

                        switch (contents[0])
                        {
                            case "image":
                                try
                                {
                                    if (Regex.IsMatch(contents[1], @"https?://.*")  // URL
                                        || Regex.IsMatch(contents[1], @"^[c-zC-Z]:\.*") // absolute path (win)
                                        || Regex.IsMatch(contents[1], @"^/.*")) // absolute path (unix)
                                    {
                                        manegemant.AddImage(contents[1]);
                                    }
                                    // if relative path, add insert file directory
                                    else
                                    {
                                        manegemant.AddImage(directory + contents[1]);
                                    }
                                }
                                // if cant find image, write massege "Image error, file not found"
                                catch (WebException)
                                {
                                    var tmp = manegemant.textColor;
                                    manegemant.textColor = new int[3] { 0xff, 0, 0 };
                                    manegemant.AddText("Image error, file not found");
                                    manegemant.textColor = tmp;
                                }
                                break;

                            case "pdf":
                                if (Regex.IsMatch(contents[1], @"https?://.*")  // URL
                                        || Regex.IsMatch(contents[1], @"^[a-zA-Z]:\\.*") // absolute path (win)
                                        || Regex.IsMatch(contents[1], @"^/.*")) // absolute path (unix)
                                {
                                    manegemant.AddPdf(contents[1]);
                                }
                                // if relative path, add insert file directory
                                else
                                {
                                    manegemant.AddPdf(directory + contents[1]);
                                }
                                break;


                            case "titleColor":
                                manegemant.titleColor = Manegement.colorToIntList(contents[1]);
                                break;

                            case "textColor":
                                manegemant.textColor = Manegement.colorToIntList(contents[1]);
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
                manegemant.AddTitle(" ");
                root.Close();
            }
            return;
        }

    }
}

