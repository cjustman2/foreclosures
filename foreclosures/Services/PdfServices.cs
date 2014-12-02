using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using HtmlAgilityPack;
using foreclosures.Utilities;
using System.Net;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using tessnet2;
using Ghostscript;
using GhostscriptSharp;
using GhostscriptSharp.Settings;
using System.Text.RegularExpressions;
using System.IO;

namespace foreclosures.Utilities
{
    public class PdfServices

    {
        public void PdfToImage(string sourcePdfFilePath, string destinationPngFilePath)
        {
            // Use GhostscriptSharp to convert the pdf to a png
            GhostscriptWrapper.GenerateOutput(sourcePdfFilePath, destinationPngFilePath,
                new GhostscriptSettings
                {
                    Device = GhostscriptDevices.pngalpha,
                    Page = new GhostscriptPages
                    {
                        // Only make a thumbnail of the first page
                        Start = 1,
                        End = 1,
                        AllPages = false
                    },
                    Resolution = new Size
                    {
                        
                        Height = 150,
                        Width = 150
                    },
                    Size = new GhostscriptPageSize
                    {
                       
                        Native = GhostscriptPageSizes.letter
                    }
                }
            );
        }






        public string ReadPdfFile(string filePath)
        {
            StringBuilder text = new StringBuilder();

            try
            {

                if (System.IO.File.Exists(filePath))
                {

                    iTextSharp.text.pdf.PdfReader pdfReader1 = new iTextSharp.text.pdf.PdfReader(filePath);

                    for (int page = 1; page < pdfReader1.NumberOfPages + 1; page++)
                    {
                        iTextSharp.text.pdf.PdfReader pdfReader = new iTextSharp.text.pdf.PdfReader(filePath);
                        iTextSharp.text.pdf.parser.ITextExtractionStrategy strategy = new iTextSharp.text.pdf.parser.SimpleTextExtractionStrategy();
                        string currentText = iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);

                        currentText =
                Encoding.UTF8.GetString(Encoding.Convert(
                    Encoding.Default,
                    Encoding.UTF8,
                    Encoding.Default.GetBytes(currentText)));

                        text.Append(currentText);
                        pdfReader.Close();
                    }

                }
            }
            catch
            {
                throw;
            }

            return text.ToString();
        }
    }
}