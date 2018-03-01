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
        //A4 size = 595 x 842

        int rowPosition = 20;

        int col1 = 20;
        int col2 = 150;
        int col3 = 220;
        int col4 = 320;
        int col5 = 420;


        PdfDocument document;
        PdfPage page;
        XGraphics gfx;
        XFont font12;
        XFont font12Bold;


        public void GenerateAllProductsReport(ItemDatabase itemDatabase)
        {
            init();
            fillHeaderFooter();
            fillItemLines(itemDatabase);  

            const string filename = "ProductReport.pdf";

            document.Save(filename);

            Process.Start(filename);
            
        }

        private void fillItemLines(ItemDatabase itemDatabase)
        {
            gfx.DrawString("All Products", font12Bold, XBrushes.Black, new XRect(0, rowPosition, page.Width, page.Height), XStringFormats.TopCenter);
            rowPosition += 40;

            gfx.DrawString("Item name", font12Bold, XBrushes.Black, new XRect(col1, rowPosition, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString("Price", font12Bold, XBrushes.Black, new XRect(col2, rowPosition, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString("Category", font12Bold, XBrushes.Black, new XRect(col3, rowPosition, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString("Quantity", font12Bold, XBrushes.Black, new XRect(col4, rowPosition, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString("Supplier", font12Bold, XBrushes.Black, new XRect(col5, rowPosition, page.Width, page.Height), XStringFormats.TopLeft);
            rowPosition += 20;
            gfx.DrawLine(XPens.DarkBlue, 10, rowPosition, 585, rowPosition);
            rowPosition += 7;

            double totalPrice = 0;
            double totalQty = 0;


            foreach (ItemPropEntry item in itemDatabase.Data)
            {
                gfx.DrawString(item.Name.Value, font12, XBrushes.Black, new XRect(col1, rowPosition, page.Width, page.Height), XStringFormats.TopLeft);
                gfx.DrawString(format2digitNumber(item.Price.Value.ToString()), font12, XBrushes.Black, new XRect(col2, rowPosition, page.Width, page.Height), XStringFormats.TopLeft);
                gfx.DrawString(item.Category.Name.Value, font12, XBrushes.Black, new XRect(col3, rowPosition, page.Width, page.Height), XStringFormats.TopLeft);
                gfx.DrawString(format2digitNumber(item.Qty.Value.ToString()), font12, XBrushes.Black, new XRect(col4, rowPosition, page.Width, page.Height), XStringFormats.TopLeft);
                gfx.DrawString(item.Supplier.Name.Value, font12, XBrushes.Black, new XRect(col5, rowPosition, page.Width, page.Height), XStringFormats.TopLeft);
                totalPrice += item.Price.Value;
                totalQty += item.Qty.Value;
                rowPosition += 20;
            }
            gfx.DrawLine(XPens.DarkBlue, 10, rowPosition, 585, rowPosition);
            rowPosition += 5;

            gfx.DrawString(format2digitNumber(totalPrice.ToString()), font12, XBrushes.Black, new XRect(col2, rowPosition, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString(format2digitNumber(totalQty.ToString()), font12, XBrushes.Black, new XRect(col4, rowPosition, page.Width, page.Height), XStringFormats.TopLeft);
        }

        private string format2digitNumber(string numberTxt)
        {
            double tmp = Convert.ToDouble(numberTxt);
            string result = tmp.ToString("0.00");

            return result;
        }

        private void init()
        {
            document = new PdfDocument();
            document.Info.Title = "Created with PDFsharp";
            page = document.AddPage();
            gfx = XGraphics.FromPdfPage(page);
            font12 = new XFont("Verdana", 12, XFontStyle.Regular);
            font12Bold = new XFont("Verdana", 12, XFontStyle.Bold);
            rowPosition += 100;
        }

        private void fillHeaderFooter()
        {
            //TODO path to Image folder?
            XImage image = XImage.FromFile("C:/Users/ratketom/source/repos/ATM_Store/TheUI/Images/logo.png");
            gfx.DrawImage(image, 380, -20, 251, 201);

            //company information
            gfx.DrawString(CompanyInfo.companyName, font12Bold, XBrushes.Black, new XRect(col1, rowPosition, page.Width, page.Height), XStringFormats.TopLeft);
            rowPosition += 20;
            gfx.DrawString(CompanyInfo.companyAddressStreet, font12, XBrushes.Black, new XRect(col1, rowPosition, page.Width, page.Height), XStringFormats.TopLeft);
            rowPosition += 20;
            gfx.DrawString(CompanyInfo.companyPhone, font12, XBrushes.Black, new XRect(col1, rowPosition, page.Width, page.Height), XStringFormats.TopLeft);
            rowPosition += 100;

        }

        public void printTransactions(TransactionDatabase transactionDatabase)
        {
            init();
            fillHeaderFooter();
            fillTransLines(transactionDatabase);

            const string filename = "Transactions.pdf";

            document.Save(filename);

            Process.Start(filename);
        }

        private void fillTransLines(TransactionDatabase transactionDatabase)
        {
            
            gfx.DrawString("All Transactions", font12Bold, XBrushes.Black, new XRect(0, rowPosition, page.Width, page.Height), XStringFormats.TopCenter);
            rowPosition += 40;

            gfx.DrawString("Date", font12Bold, XBrushes.Black, new XRect(col1, rowPosition, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString("Item Name", font12Bold, XBrushes.Black, new XRect(col2, rowPosition, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString("Quantity", font12Bold, XBrushes.Black, new XRect(col4, rowPosition, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString("Price total", font12Bold, XBrushes.Black, new XRect(col5, rowPosition, page.Width, page.Height), XStringFormats.TopLeft);
            rowPosition += 20;
            gfx.DrawLine(XPens.DarkBlue, 10, rowPosition, 585, rowPosition);
            rowPosition += 7;

            double totalPrice = 0;
            double totalQty = 0;


            foreach (TransactionPropEntry trans in transactionDatabase.Data)
            {
                gfx.DrawString(trans.TransactionTime.Value.ToString("dd.MM.yyyy hh:mm"), font12, XBrushes.Black, new XRect(col1, rowPosition, page.Width, page.Height), XStringFormats.TopLeft);
                gfx.DrawString(trans.ItemName.Value.ToString(), font12, XBrushes.Black, new XRect(col2, rowPosition, page.Width, page.Height), XStringFormats.TopLeft);
                gfx.DrawString(format2digitNumber(trans.Qty.Value.ToString()), font12, XBrushes.Black, new XRect(col4, rowPosition, page.Width, page.Height), XStringFormats.TopLeft);
                gfx.DrawString(format2digitNumber(String.Format("{0}", trans.Qty.Value * trans.SingleItemPrice.Value)), font12, XBrushes.Black, new XRect(col5, rowPosition, page.Width, page.Height), XStringFormats.TopLeft);
                totalPrice += trans.Qty.Value * trans.SingleItemPrice.Value;
                totalQty += trans.Qty.Value;
                rowPosition += 20;
            }
            gfx.DrawLine(XPens.DarkBlue, 10, rowPosition, 585, rowPosition);
            rowPosition += 5;

            gfx.DrawString(format2digitNumber(totalPrice.ToString()), font12, XBrushes.Black, new XRect(col5, rowPosition, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString(format2digitNumber(totalQty.ToString()), font12, XBrushes.Black, new XRect(col4, rowPosition, page.Width, page.Height), XStringFormats.TopLeft);
            
        }
    }
}
