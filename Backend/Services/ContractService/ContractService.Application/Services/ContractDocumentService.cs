using ContractService.Application.Abstractions.Services;
using ContractService.Domain.Contracts;

namespace ContractService.Application.Services;

public class ContractDocumentService(
    IPdfContractGenerator pdfGenerator,
    IContractStorage storage,
    IContractSigningService signer,
    IContractCertificateProvider contractCertificateProvider)
{
    public async Task GenerateContract(Guid clientId, string contractContent, Contract contract)
    {
        storage.EnsureDirectoriesExist(clientId, contract);
        string path = storage.GetContractPath(clientId, contract);
        await pdfGenerator.Generate(contractContent, path);
    }
    
    public async Task GenerateAddition(Guid clientId, string additionContent, Contract contract)
    {
        storage.EnsureDirectoriesExist(clientId, contract);
        string path = storage.GetAdditionPath(clientId, contract);
        await pdfGenerator.Generate(additionContent, path);
    }

    public async Task GenerateReturnAct(Guid clientId, string returnActContent, Contract contract)
    {
        storage.EnsureDirectoriesExist(clientId, contract);
        string path = storage.GetReturnActPath(clientId, contract);
        await pdfGenerator.Generate(returnActContent, path);
    }

    public void SignContract(Guid clientId, Contract contract, byte[] signatureImage)
    {
        var contractPath = storage.GetContractPath(clientId, contract);
        
        if (!File.Exists(contractPath))
        {
            throw new FileNotFoundException(
                $"Contract not found: {contractPath}");
        }
        var signedContractPath = storage.GetContractSignedPath(clientId, contract);
        
        signer.SignPdf(contractPath,
            signedContractPath,
            contractCertificateProvider.PfxPath,
            contractCertificateProvider.CertificatePassword,
            signatureImage);
    }

    public void SignAddition(Guid clientId, Contract contract)
    {
        var path = storage.GetAdditionPath(clientId, contract);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                $"Addition not found: {path}");
        }

        signer.SignPdf(path, path,
            contractCertificateProvider.PfxPath,
            contractCertificateProvider.CertificatePassword);
    }

    public void SignReturnAct(Guid clientId, Contract contract)
    {
        var path = storage.GetReturnActPath(clientId, contract);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                $"Return act not found: {path}");
        }

        signer.SignPdf(path, path,
            contractCertificateProvider.PfxPath,
            contractCertificateProvider.CertificatePassword);
    }
}