using ContractService.Application.Abstractions.Services;
using Microsoft.Playwright;

namespace ContractService.Infrastructure.Services.ContractsGeneration;

public class PdfContractGenerator : IPdfContractGenerator
{
    public async Task Generate(string html, string outputPath)
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
        var page = await browser.NewPageAsync();
        
        await page.SetContentAsync(html);
        await page.PdfAsync(new () {Path = outputPath});
    }
}