using ItsMyDoliprane.Enums;

namespace ItsMyDoliprane.Exceptions;

internal sealed class JsonErrorCodeException : Exception
{
    public JsonErrorCode ErrorCode { get; }

    internal JsonErrorCodeException(JsonErrorCode errorCode) {
        ErrorCode = errorCode;
    }
}
