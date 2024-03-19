using pdfHelper;

const string inputShippingPdf = "C:/tmp/shippinglabel.pdf"; // Path to the source PDF file
const string inputReturnPdf = "C:/tmp/returnlabel.pdf";
const string deliveryNoteInputPdf = "C:/tmp/deliverynote2.pdf";

var outputShippingPdf = Path.Combine(Path.GetDirectoryName(inputShippingPdf)!, Path.GetFileNameWithoutExtension(inputShippingPdf) + Guid.NewGuid() + Path.GetExtension(inputShippingPdf));
var outputReturnPdf = Path.Combine(Path.GetDirectoryName(inputReturnPdf)!, Path.GetFileNameWithoutExtension(inputReturnPdf) + Guid.NewGuid() + Path.GetExtension(inputReturnPdf));
var deliveryNoteOutputPdf = Path.Combine(Path.GetDirectoryName(deliveryNoteInputPdf)!, Path.GetFileNameWithoutExtension(deliveryNoteInputPdf) + Guid.NewGuid() + Path.GetExtension(deliveryNoteInputPdf));

var fileBytes = File.ReadAllBytes(inputShippingPdf);
using var inputStreamShipping = new MemoryStream(fileBytes);
var outputFile1 = PdfHelper.RescalePdfFromTopLeft(inputStreamShipping.ToArray());
File.WriteAllBytes(outputShippingPdf, outputFile1);
Console.WriteLine("RescalePdfFromTopLeft shipping operation complete.");

fileBytes = File.ReadAllBytes(inputReturnPdf);
using var inputStreamReturn = new MemoryStream(fileBytes);
var outputFile2 = PdfHelper.RescalePdfFromTopLeft(inputStreamReturn.ToArray());
File.WriteAllBytes(outputReturnPdf, outputFile2);
Console.WriteLine("RescalePdfFromTopLeft return operation complete.");

var deliveryNoteFileBytes = File.ReadAllBytes(deliveryNoteInputPdf);
using var inputStreamDeliveryNote = new MemoryStream(deliveryNoteFileBytes);
var outputFile3 = PdfHelper.RemoveShippingAndReturnFromDeliveryNote(inputStreamDeliveryNote.ToArray());
File.WriteAllBytes(deliveryNoteOutputPdf, outputFile3);
Console.WriteLine("RemoveShippingAndReturnFromDeliveryNote operation complete.");