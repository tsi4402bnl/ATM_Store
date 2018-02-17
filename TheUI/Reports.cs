using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;


namespace TheUI
{
    class Reports
    {

        public void GenerateAllProductsReport(ItemDatabase itemDatabase)
        {
            int tabX = 20;
            int tabY = 20;
            int spaceX = 40;
            int spaceY = 20;


            PdfDocument document = new PdfDocument();
            document.Info.Title = "Created with PDFsharp";
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Verdana", 12, XFontStyle.Regular);

            foreach (ItemPropEntry item in itemDatabase.Data)
            {
                gfx.DrawString(item.Name.Value,                 font, XBrushes.Black, new XRect(tabX, tabY, page.Width, page.Height), XStringFormats.TopLeft);
                gfx.DrawString(item.Price.Value.ToString(),     font, XBrushes.Black, new XRect(tabX + 4 * tabX, tabY, page.Width, page.Height), XStringFormats.TopLeft);
                gfx.DrawString(item.Category.Name.Value,        font, XBrushes.Black, new XRect(tabX + 6 * tabX, tabY, page.Width, page.Height), XStringFormats.TopLeft);
                gfx.DrawString(item.QtyPerBox.Value.ToString(),       font, XBrushes.Black, new XRect(tabX + 9 * tabX, tabY, page.Width, page.Height), XStringFormats.TopLeft);
                gfx.DrawString(item.Supplier.Name.Value,        font, XBrushes.Black, new XRect(tabX + 11 * tabX, tabY, page.Width, page.Height), XStringFormats.TopLeft);

                tabY += spaceY;
            }



            const string filename = "ProductReport.pdf";

            document.Save(filename);

            // ...and start a viewer.

            Process.Start(filename);
            
        }
    }
}
