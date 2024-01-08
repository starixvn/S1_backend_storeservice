using System.Runtime.Serialization;

using StoreService.Core.Service.Model.Enums;

namespace StoreService.Core.Exceptions;

public class StoreException : Exception, ISerializable
{
    public ErrorCodeEnum ErrorCode { get; set; }

    public StoreException()
    {
    }

    public StoreException(string message)
        : base(message)
    {
    }

    public StoreException(ErrorCodeEnum errorCode, string message, Exception inner)
        : base(message, inner)
    {
        ErrorCode = errorCode;
    }

    public StoreException(string message, Exception inner)
        : this(ErrorCodeEnum.SystemError, message, inner)
    {
    }
}