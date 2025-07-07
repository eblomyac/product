using System.Globalization;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using ProtoLib.Model;
using Path = System.IO.Path;

namespace ProtoLib.Managers;

public class TransferPrinter
{
  
    static string ttf = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "calibri.TTF");
    static string ttfBold = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "calibrib.TTF");
    static TrueTypeFont baseFont = new TrueTypeFont(ttf);
    static TrueTypeFont baseFontBold = new TrueTypeFont(ttfBold);
    PdfFont font = PdfFontFactory.CreateFont(baseFont, "cp1251", true);
    PdfFont fontBold = PdfFontFactory.CreateFont(baseFontBold, "cp1251", true);
    public  string PdfFolder = "";

    public TransferPrinter(string folder)
    {
        PdfFolder = folder;
    }
    
    
    public string MakeTransferPdf(Transfer t)
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
        //font = PdfFontFactory.CreateFont(baseFont, "cp1251", false);
        string pdfName = Guid.NewGuid().ToString() + ".pdf";
        string fullFileName = Path.Combine(PdfFolder, pdfName);
        Directory.CreateDirectory(PdfFolder);
        PdfWriter pw = new PdfWriter(fullFileName);
        var pdfDoc = new PdfDocument(pw);
        pdfDoc.SetDefaultPageSize(PageSize.A4);
        var doc = new Document(pdfDoc);
        doc.SetFont(font);
        doc.SetFontSize(9);
        
        doc.SetBottomMargin(10);
        doc.SetLeftMargin(20);
        doc.SetRightMargin(10);
        doc.SetTopMargin(10);

        
        UnitValue[] unitValuesHeader = new UnitValue[] {UnitValue.CreatePercentValue(80),UnitValue.CreatePercentValue(20) };
        Table tableHeader = new Table(unitValuesHeader).UseAllAvailableWidth().SetBorder(Border.NO_BORDER);
        tableHeader.AddCell(new Cell().Add(new Paragraph($"Акт передачи {t.PaperId}").SetFontSize(16).SetTextAlignment(TextAlignment.CENTER)).SetBorder(Border.NO_BORDER));
        tableHeader.AddCell(new Cell().Add(new Paragraph($"от {t.CreatedStamp:dd.MM.yyy}").SetFontSize(16).SetTextAlignment(TextAlignment.RIGHT)).SetBorder(Border.NO_BORDER).SetHorizontalAlignment(HorizontalAlignment.RIGHT));

        tableHeader.SetMarginBottom(30);
        doc.Add(tableHeader);
        

        UnitValue[] unitValuesFrom = new UnitValue[] {
            UnitValue.CreatePercentValue(40),
            UnitValue.CreatePercentValue(20),
            UnitValue.CreatePercentValue(20),
            UnitValue.CreatePercentValue(20),
        };
        
        Table from = new Table(unitValuesFrom).UseAllAvailableWidth().SetBorder(Border.NO_BORDER);
        from.AddCell(new Cell().Add(new Paragraph($"{t.PostFromId}, сдал: ")).SetBorder(Border.NO_BORDER));
        from.AddCell(new Cell().Add(new Paragraph("______________________")).SetBorder(Border.NO_BORDER));
        from.AddCell(new Cell().Add(new Paragraph("______________________")).SetBorder(Border.NO_BORDER));
        from.AddCell(new Cell().Add(new Paragraph("______________________")).SetBorder(Border.NO_BORDER));
        from.AddCell(new Cell().Add(new Paragraph("                      ")).SetBorder(Border.NO_BORDER));
       
        from.AddCell(new Cell().Add(new Paragraph("должность").SetFontSize(6).SetTextAlignment(TextAlignment.CENTER)).SetBorder(Border.NO_BORDER));
        from.AddCell(new Cell().Add(new Paragraph("ФИО").SetFontSize(6).SetTextAlignment(TextAlignment.CENTER)).SetBorder(Border.NO_BORDER));
        from.AddCell(new Cell().Add(new Paragraph("подпись").SetFontSize(6).SetTextAlignment(TextAlignment.CENTER)).SetBorder(Border.NO_BORDER));
        
        Table to =new Table(unitValuesFrom).UseAllAvailableWidth().SetBorder(Border.NO_BORDER);
        to.AddCell(new Cell().Add(new Paragraph($"{t.PostToId}, принял: ")).SetBorder(Border.NO_BORDER));
        to.AddCell(new Cell().Add(new Paragraph("______________________")).SetBorder(Border.NO_BORDER));
        to.AddCell(new Cell().Add(new Paragraph("______________________")).SetBorder(Border.NO_BORDER));
        to.AddCell(new Cell().Add(new Paragraph("______________________")).SetBorder(Border.NO_BORDER));
        to.AddCell(new Cell().Add(new Paragraph("                      ")).SetBorder(Border.NO_BORDER));
       
        to.AddCell(new Cell().Add(new Paragraph("должность").SetFontSize(6).SetTextAlignment(TextAlignment.CENTER)).SetBorder(Border.NO_BORDER));
        to.AddCell(new Cell().Add(new Paragraph("ФИО").SetFontSize(6).SetTextAlignment(TextAlignment.CENTER)).SetBorder(Border.NO_BORDER));
        to.AddCell(new Cell().Add(new Paragraph("подпись").SetFontSize(6).SetTextAlignment(TextAlignment.CENTER)).SetBorder(Border.NO_BORDER));


        from.SetMarginBottom(30);
        to.SetMarginBottom(30);
        
        doc.Add(from);
        doc.Add(to);
        
        UnitValue[] unitValuesData = new UnitValue[] {
            UnitValue.CreatePercentValue(5),
            UnitValue.CreatePercentValue(25),
            UnitValue.CreatePercentValue(10),
            UnitValue.CreatePercentValue(10),
            UnitValue.CreatePercentValue(45),
            
        };
        Table data = new Table(unitValuesData).UseAllAvailableWidth();
        int loop = 1;
        data.AddCell("#").SetTextAlignment(TextAlignment.LEFT);
        data.AddCell("Артикул").SetTextAlignment(TextAlignment.CENTER);
        data.AddCell("ко-во").SetTextAlignment(TextAlignment.CENTER);
        data.AddCell("№ заказа").SetTextAlignment(TextAlignment.CENTER);
        data.AddCell("комментарии отклоняя").SetTextAlignment(TextAlignment.CENTER);
        
        foreach (var line in t.Lines)
        {
            data.AddCell(loop.ToString()).SetTextAlignment(TextAlignment.LEFT);
            data.AddCell(line.Article).SetTextAlignment(TextAlignment.CENTER);
            data.AddCell(line.Count.ToString()).SetTextAlignment(TextAlignment.CENTER);
            data.AddCell(line.OrderNumber.ToString()).SetTextAlignment(TextAlignment.CENTER);
            data.AddCell("").SetTextAlignment(TextAlignment.CENTER);
        }

        doc.Add(data);
        doc.Close();

        return pdfName;

    }

    public string MakeOtkReportKsk(string article, string orderNumber)
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
        //font = PdfFontFactory.CreateFont(baseFont, "cp1251", false);
        string pdfName = Guid.NewGuid().ToString() + ".pdf";
        string fullFileName = Path.Combine(PdfFolder, pdfName);
        Directory.CreateDirectory(PdfFolder);
        PdfWriter pw = new PdfWriter(fullFileName);
        var pdfDoc = new PdfDocument(pw);
        pdfDoc.SetDefaultPageSize(PageSize.A4);
        var doc = new Document(pdfDoc);
        doc.SetFont(font);
        doc.SetFontSize(9);
        
        doc.SetBottomMargin(10);
        doc.SetLeftMargin(20);
        doc.SetRightMargin(10);
        doc.SetTopMargin(10);
        
        
        doc.Close();
        return pdfName;
    }
    
    


}