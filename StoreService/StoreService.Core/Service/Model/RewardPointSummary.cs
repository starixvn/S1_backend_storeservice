using StoreService.Core.Service.Model.Enums;

namespace StoreService.Core.Service.Model;

public class CustPointSummary
{
    public int CustId { get; set; }
    public decimal PointBalance { get; set; }
    public decimal PointEarned { get; set; }
    public decimal PointUsed { get; set; }
    public decimal PointLocked { get; set; }
    public List<RewardPointSummary> Detail { get; set; }
}

public class RewardPointSummary
{
    public PointTypeEnum PointType { get; set; }
    public string PointTypeName
    {
        get
        {
            return this.PointType.ToString();
        }
    }
    public decimal PlusSummary { get; set; }
    public decimal MinusSummary { get; set; }
    public decimal TotalSummary { get; set; }
    public Dictionary<string, decimal> DetailRewardCoin { get; set; }
}

public class RedemptionPointSummary
{
    public int CustId { get; set; }
    public RedemptionEnum Status { get; set; }
    public decimal Summary { get; set; }
}

public class TurnoverPointSummary
{
    public int CustId { get; set; }
    public string CategoryName { get; set; }
    public decimal Summary { get; set; }
}
