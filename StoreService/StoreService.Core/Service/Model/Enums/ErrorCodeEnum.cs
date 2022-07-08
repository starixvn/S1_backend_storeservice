namespace StoreService.Core.Service.Model.Enums;

public enum ErrorCodeEnum
{
    Success = 0,
    Failed = -1,
    SystemError = 9000,
    RedemptionLimitIsClose = -1001,
    RedemptionLimitOverLimit = -1002
}