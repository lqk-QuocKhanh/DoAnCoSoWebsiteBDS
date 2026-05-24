namespace VietPropEstate.Application.Common.Exceptions;

public class ForbiddenAccessException : Exception
{
    public const string PhoneVerificationRequiredCode = "phone_verification_required";

    public ForbiddenAccessException() : base("Access is denied.") { }

    public ForbiddenAccessException(string message) : base(message) { }

    public ForbiddenAccessException(string message, string errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }

    public string? ErrorCode { get; }
}
