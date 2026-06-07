namespace ContractService.Application.Features.Contracts.GetContractPdf;

public class ContractPdfResponse
{
    public string FilePath { get; init; }
    public string FileName { get; init; }
    public string ContentType { get; init; } = "application/pdf";
    public bool Exists { get; init; }
}