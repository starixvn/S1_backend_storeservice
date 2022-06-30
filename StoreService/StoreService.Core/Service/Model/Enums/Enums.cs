namespace StoreService.Core.Service.Model.Enums;

public enum DataBaseEnum
{
    Main,
    Report
}

public enum GeneralStatusEnum
{
    Enabled = 1,
    Disabled = 0
}

public enum PointTypeEnum
{
    Deposit = 1,
    Turnover = 2,
    /// <summary>
    /// 兌換物品
    /// </summary>
    Redemption = 3,
    /// <summary>
    /// 手動
    /// </summary>
    Manual = 4,
    System = 5
}

public enum DepositRewardPointQueueStatusEnum
{
    Running = 1,
    Finish = 2
}