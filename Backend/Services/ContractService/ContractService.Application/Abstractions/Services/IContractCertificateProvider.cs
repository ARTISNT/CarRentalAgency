namespace ContractService.Application.Abstractions.Services;

public interface IContractCertificateProvider
{
    string PfxPath { get; }
    string CertificatePath { get; }
    string CertificatePassword { get; }
}