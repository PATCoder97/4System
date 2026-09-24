namespace Winform4System.Business.Security
{
    public enum DomainCredentialValidationResult
    {
        Valid,
        Invalid,
        Unavailable
    }

    public interface IDomainCredentialValidator
    {
        DomainCredentialValidationResult Validate(string accountName, string password);
    }
}
