using ContractService.Application.Abstractions.Services;
using Microsoft.Extensions.Configuration;

namespace ContractService.Infrastructure.Services.ContractsSigning;

public class ContractCertificateProvider : IContractCertificateProvider 
{
    public string PfxPath { get; }
    public string CertificatePath { get; }
    public string CertificatePassword { get; }

    public ContractCertificateProvider(IConfiguration configuration)
    {
        PfxPath = configuration["ContractCertificate:Pfx"] 
                   ?? throw new ArgumentNullException("ContractCertificate:PfxPath is not configured");
        
        CertificatePath = configuration["ContractCertificate:Certificate"]
            ?? throw new ArgumentNullException("ContractCertificate:CertificatePath is not configured");
        
        CertificatePassword = configuration["ContractCertificate:Password"]
            ?? throw new ArgumentNullException("ContractCertificate:CCertificatePassword is not configured");

    }
}