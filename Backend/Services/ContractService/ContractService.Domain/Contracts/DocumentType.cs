using ContractService.Domain.Common;

namespace ContractService.Domain.Contracts;

public class DocumentType : Enumeration
{
    public static readonly DocumentType ReturnAct = new DocumentType(1, "ReturnAct");
    public static readonly DocumentType Contract = new DocumentType(2, "Contract");
    public static readonly DocumentType Addition= new DocumentType(3, "Addition");
    
    public DocumentType(int id, string name) : base(id, name)
    {
    }
}