using StoreService.Core.Service.Model.Enums;

namespace StoreService.Core.Service.Model;

public interface ISelLang
{
    string GetErrMsg(int code);
}

public class ApiResult
{
    public int ErrorCode { get; set; }
    private string _Message;
    protected ISelLang SelLang;

    public string Message
    {
        get
        {
            if (!string.IsNullOrEmpty(_Message))
            {
                return _Message;
            }
            else
            {
                if (SelLang != null)
                    _Message = SelLang.GetErrMsg(ErrorCode);
            }

            return _Message;
        }
        set => _Message = value;
    }

    public void SetSelLang(ISelLang selLang)
    {
        SelLang = selLang;
    }

    public string RawErrorCode { get; set; }
    public string RawMessage { get; set; }

    public ApiResult()
        : this((int)ErrorCodeEnum.SystemError, "")
    {
    }

    public ApiResult(ISelLang selLang)
        : this((int)ErrorCodeEnum.SystemError, "")
    {
        SelLang = selLang;
    }

    public ApiResult(ErrorCodeEnum errorCode, string message = "")
        : this((int)errorCode, message)
    {
    }

    public ApiResult(int errorCode, string message, string rawErrorCode = "", string rawMessage = "")
    {
        ErrorCode = errorCode;
        Message = message;
        RawErrorCode = rawErrorCode;
        RawMessage = rawMessage;
    }
}

public class ApiResult<TData> : ApiResult
{
    public ApiResult() : this((int)ErrorCodeEnum.SystemError, default)
    {
    }

    public ApiResult(ISelLang selLang) : this((int)ErrorCodeEnum.SystemError, default)
    {
        SelLang = selLang;
    }

    public ApiResult(ErrorCodeEnum errorCode, TData data, string message = "")
        : this((int)errorCode, data, message)
    {
    }

    public ApiResult(int errorCode, TData data, string message = "") : base(errorCode, message)
    {
        Data = data;
    }

    public TData Data { get; set; }
}

public class ApiListResult<TData> : ApiResult
{
    public ApiListResult() : this((int)ErrorCodeEnum.SystemError)
    {
    }

    public ApiListResult(ISelLang selLang) : this((int)ErrorCodeEnum.SystemError)
    {
        SelLang = selLang;
    }

    public ApiListResult(int errorCode, string message = "") : base(errorCode, message)
    {
    }

    public IEnumerable<TData> Data { get; set; }
    public int TotalPages { get; set; }
    public int PageNumber { get; set; }
    public int RowCount { get; set; }
}