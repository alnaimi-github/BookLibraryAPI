using System.Runtime.Serialization;

namespace BookLibraryAPI.Core.Domain.Common.Exceptions;

[Serializable]
public class DomainException : Exception
{
    public string ErrorCode { get; }
    public string? PropertyName { get; }
    public IDictionary<string, object>? AdditionalData { get; }

    public DomainException(
        string message,
        string errorCode = "DOMAIN_ERROR",
        string? propertyName = null,
        IDictionary<string, object>? additionalData = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        PropertyName = propertyName;
        AdditionalData = additionalData;
    }

    protected DomainException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ErrorCode = info.GetString(nameof(ErrorCode)) ?? "DOMAIN_ERROR";
        PropertyName = info.GetString(nameof(PropertyName));
        AdditionalData = (IDictionary<string, object>?)info.GetValue(nameof(AdditionalData), typeof(IDictionary<string, object>));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(ErrorCode), ErrorCode);
        info.AddValue(nameof(PropertyName), PropertyName);
        info.AddValue(nameof(AdditionalData), AdditionalData);
    }
}

