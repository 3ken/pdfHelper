using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;

namespace pdfHelper;

public static class PdfHelper
{
    public static byte[] RescalePdfFromTopLeft(byte[] sourcePdf)
    {
        try
        {
            using var resultStream = new MemoryStream();
            var sourceDoc = new PdfDocument(new PdfReader(new MemoryStream(sourcePdf)));
            var sourcePageSize = sourceDoc.GetFirstPage().GetPageSize();

            var resultPdfDoc = new PdfDocument(new PdfWriter(resultStream));
            var resultPageSize = PageSize.A4;
            resultPdfDoc.SetDefaultPageSize(new PageSize(resultPageSize));
            resultPdfDoc.AddNewPage();

            var pageXObject = sourceDoc.GetFirstPage().CopyAsFormXObject(resultPdfDoc);
            var toExtract = new Rectangle(0, sourcePageSize.GetHeight() / 2, sourcePageSize.GetWidth() / 2.1f, sourcePageSize.GetHeight() / 2);

            // Create a formXObject of the area to move.
            var formXObject = new PdfFormXObject(new Rectangle(0, sourcePageSize.GetHeight() / 2, sourcePageSize.GetWidth() / 2.1f, sourcePageSize.GetHeight() / 2));
            var pdfCanvas = new PdfCanvas(formXObject, resultPdfDoc);
            pdfCanvas.Rectangle(toExtract);

            // This method uses the nonzero winding rule to determine which regions lie inside the clipping path.
            pdfCanvas.Clip();
            pdfCanvas.EndPath();
            pdfCanvas.AddXObjectAt(pageXObject, 0, 0);

            var canvas = new PdfCanvas(resultPdfDoc.GetFirstPage());

            // Add the area to move result document.
            canvas.AddXObjectFittedIntoRectangle(formXObject, resultPageSize);

            sourceDoc.Close();
            resultPdfDoc.Close();

            return resultStream.ToArray();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public static byte[] RemoveShippingAndReturnFromDeliveryNote(byte[] sourcePdf)
    {
        try
        {
            using var targetStream = new MemoryStream();
            using var reader = new PdfReader(new MemoryStream(sourcePdf));
            using var writer = new PdfWriter(targetStream);
            using (var pdfDoc = new PdfDocument(reader, writer))
            {
                for (var i = pdfDoc.GetNumberOfPages() - 1; i > 0; i--)
                {
                    if (i == pdfDoc.GetNumberOfPages()) continue;
                    pdfDoc.RemovePage(i);
                }
            }

            return targetStream.ToArray();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}