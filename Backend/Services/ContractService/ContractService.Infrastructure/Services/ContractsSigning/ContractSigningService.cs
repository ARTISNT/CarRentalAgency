using ContractService.Application.Abstractions.Services;
using iText.Bouncycastle.Crypto;
using iText.Bouncycastle.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Signatures;
using Org.BouncyCastle.Pkcs;

namespace ContractService.Infrastructure.Services.ContractsSigning;

public class ContractSigningService  : IContractSigningService
{
    public void SignPdf(string src, string dest, string pfxPath, string password)
    {
        using Stream pfxStream = new FileStream(pfxPath, FileMode.Open, FileAccess.Read);
        Pkcs12Store pkcs12Store = new Pkcs12StoreBuilder().Build();
        pkcs12Store.Load(pfxStream, password.ToCharArray());

        string? alias = null;
        foreach (string a in pkcs12Store.Aliases)
        {
            if (pkcs12Store.IsKeyEntry(a))
            {
                alias = a;
                break;
            }
        }

        if (alias == null)
            throw new InvalidOperationException("Закрытый ключ не найден в PFX файле.");

        AsymmetricKeyEntry keyEntry = pkcs12Store.GetKey(alias);
        IX509Certificate[] chain = pkcs12Store.GetCertificateChain(alias)
            .Select(c => new X509CertificateBC(c.Certificate))
            .Cast<IX509Certificate>()
            .ToArray();

        IPrivateKey privateKey = new PrivateKeyBC(keyEntry.Key);

        using PdfReader reader = new PdfReader(src);
        using Stream outputStream = new FileStream(dest, FileMode.Create, FileAccess.Write);

        StampingProperties stampingProperties = new StampingProperties();
        
        PdfSigner signer = new PdfSigner(reader, outputStream, stampingProperties);

        Rectangle rect = new Rectangle(36, 36, 200, 100); 
        PdfSignatureAppearance appearance = signer.GetSignatureAppearance();
        
        appearance
            .SetPageRect(rect)
            .SetPageNumber(1) 
            .SetReason("Тестирование подписания PDF")
            .SetLocation("Москва")
            .SetRenderingMode(PdfSignatureAppearance.RenderingMode.DESCRIPTION); 

        signer.SetFieldName("Signature1");

        IExternalSignature signature = new PrivateKeySignature(privateKey, DigestAlgorithms.SHA256);

        signer.SignDetached(signature, chain, null, null, null, 0, PdfSigner.CryptoStandard.CMS);
    } 
}