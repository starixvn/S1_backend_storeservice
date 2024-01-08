namespace StoreService.Core.Service.Model.TurnoverWorker;

public class TurnoverPeriodForWorkerUse
{
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public int RedeemHours { get; set; }
    public DateTime LastSettleTime { get; set; }
    public string TimeZone { get; set; }
    public DateTime CurrencyNowTime { get; set; }
    public DateTime CurrencyLastSettledTime { get; set; }
}

public class TurnoverSettingForWorkerUse
{
    public int TpCategoryId { get; set; }
    public int VipLevel { get; set; }
    public int TpGameTypeId { get; set; }
    public decimal Turnover { get; set; }
    public decimal PointRate { get; set; }
    public bool PromotionStatus { get; set; }
    public decimal PromotionPointRate { get; set; }
    public bool IsInPromotionPeriod { get; set; }
}

public class TurnoverCustListModel
{
    public string CaculateDate { get; set; }
    public int CustId { get; set; }
    public string UserName { get; set; }
    public int VipLevel { get; set; }
}

public class CustTurnoverBaseModel
{
    public int SiteId { get; set; }
    public int CustId { get; set; }
    public int CurrencyId { get; set; }
    public DateTime SettledDate { get; set; }
    public int CategoryId { get; set; }
    public int GameType { get; set; }
    public decimal ValidBet { get; set; }
}

public class CustRemainRecordModel
{
    public int CustId { get; set; }
    public int CategoryId { get; set; }
    public decimal RemainAmount { get; set; }
}

public class CustAddPointModel
{
    public string SettledDate { get; set; }
    public int CategoryId { get; set; }
    public bool IsUsePromotionRate { get; set; }
    public decimal RateTurnover { get; set; }
    public decimal Rate { get; set; }
    public decimal AddPoint { get; set; }
    public decimal ThisTurnover { get; set; }
    public decimal ThisRemain { get; set; }
    public decimal UsedTurnover { get; set; }
    public decimal UnUseTurnover { get; set; }
    public decimal UsedRemain { get; set; }
    public decimal UnUseRemain { get; set; }
    public decimal ChangeRemain { get; set; }
}
