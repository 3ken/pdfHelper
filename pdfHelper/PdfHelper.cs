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
            using var sourceStream = new MemoryStream(sourcePdf);
            using var resultStream = new MemoryStream();
            var sourceDoc = new PdfDocument(new PdfReader(sourceStream));
            using var writer = new PdfWriter(resultStream);
            var sourcePageSize = sourceDoc.GetFirstPage().GetPageSize();
            var sourcePageRatio = sourcePageSize.GetHeight() / sourcePageSize.GetWidth();

            var resultPdfDoc = new PdfDocument(writer);
            var resultPageSize = new Rectangle(0, 0, 102, 102 * sourcePageRatio);
            resultPdfDoc.SetDefaultPageSize(new PageSize(resultPageSize));
            resultPdfDoc.AddNewPage();

            var pageXObject = sourceDoc.GetFirstPage().CopyAsFormXObject(resultPdfDoc);
            var toExtract = new Rectangle(0, sourcePageSize.GetHeight() / 2, sourcePageSize.GetWidth() / 2.1f, sourcePageSize.GetHeight() / 2);

            // Create a formXObject of a page content, in which the area to move is cut.
            var formXObject1 = new PdfFormXObject(sourcePageSize);
            var canvas1 = new PdfCanvas(formXObject1, resultPdfDoc);
            canvas1.Rectangle(toExtract);

            // This method uses the even-odd rule to determine which regions lie inside the clipping path.
            canvas1.EoClip();
            canvas1.EndPath();

            // Create a formXObject of the area to move.
            var formXObject2 = new PdfFormXObject(new Rectangle(0, sourcePageSize.GetHeight() / 2, sourcePageSize.GetWidth() / 2.1f, sourcePageSize.GetHeight() / 2));
            var canvas2 = new PdfCanvas(formXObject2, resultPdfDoc);
            canvas2.Rectangle(toExtract);

            // This method uses the nonzero winding rule to determine which regions lie inside the clipping path.
            canvas2.Clip();
            canvas2.EndPath();
            canvas2.AddXObjectAt(pageXObject, 0, 0);

            var canvas = new PdfCanvas(resultPdfDoc.GetFirstPage());

            // Add the area to move content, shifted 10 points to the left and 2 points to the bottom.
            canvas.AddXObjectFittedIntoRectangle(formXObject2, resultPageSize);

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

    public static byte[] Scale(byte[] sourcePdf)
    {
        using var sourceStream = new MemoryStream(sourcePdf);
        using var resultStream = new MemoryStream();
        using var reader = new PdfReader(sourceStream);
        using var writer = new PdfWriter(resultStream);
        using var sourceDocument = new PdfDocument(reader);
        using var destinationDocument = new PdfDocument(writer);
        destinationDocument.SetDefaultPageSize(PageSize.A4);
        var sourcePage = sourceDocument.GetPage(1);
        var sourcePageSize = sourcePage.GetPageSize();
        var topLeftRect = new Rectangle(0, sourcePageSize.GetHeight(), sourcePageSize.GetWidth() / 2, sourcePageSize.GetHeight() / 2);

        var destinationPage = destinationDocument.AddNewPage();
        var destinationPageSize = destinationPage.GetPageSize();

        var destinationCanvas = new PdfCanvas(destinationPage);
        destinationCanvas.Rectangle(topLeftRect);
        destinationCanvas.Clip();

        var sourceCanvas = new PdfCanvas(sourcePage);
        sourceCanvas.SaveState();
        sourceCanvas.Rectangle(topLeftRect);
        sourceCanvas.Clip();
        var clippedXObject = new PdfFormXObject(topLeftRect);
        new PdfCanvas(clippedXObject, destinationDocument).AddXObject(sourcePage.CopyAsFormXObject(destinationDocument));
        sourceCanvas.RestoreState();

        var scaledRect = new Rectangle(0, 0, destinationPageSize.GetWidth(), destinationPageSize.GetHeight());
        destinationCanvas.AddXObjectAt(clippedXObject, 0, 0);

        sourceDocument.Close();
        destinationDocument.Close();

        return resultStream.ToArray();
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