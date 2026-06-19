namespace ContractService.Application.Abstractions.Services;

public interface IPdfStampRenderer
{
    void AddSignatureStamp(string pdfPath, string? organization = null);
}
