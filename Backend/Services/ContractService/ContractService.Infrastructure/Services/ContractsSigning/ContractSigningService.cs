using ContractService.Application.Abstractions.Services;
using iText.Bouncycastle.Crypto;
using iText.Bouncycastle.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Signatures;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Pkcs;
using X509Name = Org.BouncyCastle.Asn1.X509.X509Name;

namespace ContractService.Infrastructure.Services.ContractsSigning;

public class ContractSigningService  : IContractSigningService
{
    public void SignPdf(string src, string dest, string pfxPath, string password,
        byte[]? signatureImage = null)
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
        var certificateEntries = pkcs12Store.GetCertificateChain(alias);
        
        IX509Certificate[] chain = certificateEntries
            .Select(c => new X509CertificateBC(c.Certificate))
            .Cast<IX509Certificate>()
            .ToArray();

        var bcCert = certificateEntries[0].Certificate;
        
        string cn = GetDnValue(bcCert.SubjectDN, X509Name.CN) ?? "";
        string o = GetDnValue(bcCert.SubjectDN, X509Name.O) ?? "";
        string l = GetDnValue(bcCert.SubjectDN, X509Name.L) ?? "Москва";

        IPrivateKey privateKey = new PrivateKeyBC(keyEntry.Key);

        byte[] fileBytes = File.ReadAllBytes(src);
        using PdfReader reader = new PdfReader(new MemoryStream(fileBytes));
        using Stream outputStream = new FileStream(dest, FileMode.Create, FileAccess.Write);

        StampingProperties stampingProperties = new StampingProperties();
        
        PdfSigner signer = new PdfSigner(reader, outputStream, stampingProperties);

        var rect = new Rectangle(36, 36, 300, 200); 
        PdfSignatureAppearance appearance = signer.GetSignatureAppearance();
        
        appearance
            .SetPageRect(rect)
            .SetPageNumber(1) 
            .SetReason(string.IsNullOrEmpty(o) ? "Электронная подпись" : $"Подписано: {o}")
            .SetLocation(l)
            .SetRenderingMode(PdfSignatureAppearance.RenderingMode.NAME_AND_DESCRIPTION);

        if (signatureImage != null)
        {
            var imageData = ImageDataFactory.Create(signatureImage);
            appearance.SetSignatureGraphic(imageData);
            appearance.SetRenderingMode(
                PdfSignatureAppearance.RenderingMode.GRAPHIC_AND_DESCRIPTION);
        }

        signer.SetFieldName("Signature1");

        IExternalSignature signature = new PrivateKeySignature(privateKey, DigestAlgorithms.SHA256);

        signer.SignDetached(signature, chain, null, null, null, 0, PdfSigner.CryptoStandard.CMS);
    }

    private static string? GetDnValue(X509Name dn, DerObjectIdentifier key)
    {
        var values = dn.GetValueList(key);
        return values?.OfType<string>().FirstOrDefault();
    } 
}