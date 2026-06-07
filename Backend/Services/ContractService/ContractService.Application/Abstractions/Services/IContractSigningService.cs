namespace ContractService.Application.Abstractions.Services;

public interface IContractSigningService
{
    public void SignPdf(string src, string dest, string pfxPath, string password,
        byte[]? signatureImage = null);
}