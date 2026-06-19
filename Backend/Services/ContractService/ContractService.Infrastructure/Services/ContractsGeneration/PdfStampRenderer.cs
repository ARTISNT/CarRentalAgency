using System.Globalization;
using ContractService.Application.Abstractions.Services;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using Microsoft.Extensions.Logging;

namespace ContractService.Infrastructure.Services.ContractsGeneration;

public class PdfStampRenderer(ILogger<PdfStampRenderer> logger) : IPdfStampRenderer
{
    private const float Margin = 36f;
    private const float RectWidth = 200f;
    private const float RectHeight = 80f;
    private const float Padding = 6f;
    private const float TitleFontSize = 8f;
    private const float BodyFontSize = 7f;

    public void AddSignatureStamp(string pdfPath, string? organization = null)
    {
        if (!File.Exists(pdfPath))
            throw new FileNotFoundException($"PDF not found: {pdfPath}");

        string tempPath = pdfPath + ".stamp.tmp";
        if (File.Exists(tempPath))
            File.Delete(tempPath);

        try
        {
            byte[] fileBytes = File.ReadAllBytes(pdfPath);
            using var inputStream = new MemoryStream(fileBytes);
            using var reader = new PdfReader(inputStream);
            using var outputStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write);
            var stampingProperties = new StampingProperties().UseAppendMode();
            using var pdfDoc = new PdfDocument(reader, new PdfWriter(outputStream), stampingProperties);

            int lastPage = pdfDoc.GetNumberOfPages();
            if (lastPage < 1)
            {
                logger.LogWarning("PDF {Path} has no pages, skipping stamp", pdfPath);
                return;
            }

            var page = pdfDoc.GetPage(lastPage);
            var pageSize = page.GetPageSizeWithRotation();
            float pageWidth = pageSize.GetWidth();

            float rectWidth = Math.Min(RectWidth, Math.Max(0f, pageWidth - 2f * Margin));
            float rectHeight = RectHeight;
            float x = pageWidth - rectWidth - Margin;
            float y = Margin;

            var canvas = new PdfCanvas(page);
            var font = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA);
            var fontBold = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);

            canvas.SaveState()
                .SetFillColor(new DeviceRgb(245, 245, 245))
                .SetStrokeColor(new DeviceRgb(120, 120, 120))
                .SetLineWidth(0.6f)
                .Rectangle(x, y, rectWidth, rectHeight)
                .FillStroke()
                .RestoreState();

            float textX = x + Padding;
            float titleY = y + rectHeight - Padding - TitleFontSize;
            float bodyY1 = titleY - BodyFontSize - 2f;
            float bodyY2 = bodyY1 - BodyFontSize - 1f;

            canvas.SaveState()
                .BeginText()
                .SetFontAndSize(fontBold, TitleFontSize)
                .SetColor(new DeviceRgb(40, 40, 40), true)
                .MoveText(textX, titleY)
                .ShowText("Документ подписан электронной подписью")
                .EndText()
                .RestoreState();

            var dateText = $"Дата: {DateTime.Now.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture)}";
            canvas.SaveState()
                .BeginText()
                .SetFontAndSize(font, BodyFontSize)
                .SetColor(new DeviceRgb(80, 80, 80), true)
                .MoveText(textX, bodyY1)
                .ShowText(Truncate(dateText, font, BodyFontSize, rectWidth - 2 * Padding))
                .EndText()
                .RestoreState();

            if (!string.IsNullOrWhiteSpace(organization))
            {
                var orgText = $"Подписано: {organization}";
                canvas.SaveState()
                    .BeginText()
                    .SetFontAndSize(font, BodyFontSize)
                    .SetColor(new DeviceRgb(80, 80, 80), true)
                    .MoveText(textX, bodyY2)
                    .ShowText(Truncate(orgText, font, BodyFontSize, rectWidth - 2 * Padding))
                    .EndText()
                    .RestoreState();
            }

            logger.LogInformation(
                "Added signature stamp to {Path}, lastPage={Page}, rect=({X},{Y},{W},{H})",
                pdfPath, lastPage, x, y, rectWidth, rectHeight);
        }
        catch
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
            throw;
        }

        File.Move(tempPath, pdfPath, overwrite: true);
    }

    private static string Truncate(string text, PdfFont font, float fontSize, float maxWidth)
    {
        if (font.GetWidth(text, fontSize) <= maxWidth)
            return text;

        const string ellipsis = "...";
        string trimmed = text;
        while (trimmed.Length > 0 && font.GetWidth(trimmed + ellipsis, fontSize) > maxWidth)
            trimmed = trimmed[..^1];
        return trimmed + ellipsis;
    }
}
