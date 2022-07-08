using Newtonsoft.Json;
using StoreService.Core.Service.Model.Based;

namespace StoreService.Core.Service.Model.PointSetting;

public class InactivePointExpirySetting : LastUpdateModel
{
    [JsonIgnore]
    public int SiteId { get; set; }
    public int Days { get; set; }
    public int Status { get; set; }
}

public class TurnoverPointSetting : LastUpdateModel
{
    private DateTimeOffset _startTime { get; set; }
    private DateTimeOffset _endTime { get; set; }
    
    public int TpCategoryId { get; set; }
    public string TpCategoryName { get; set; }
    public decimal Turnover { get; set; }
    public decimal PointRate { get; set; }
    public int PromotionStatus { get; set; }
    public decimal PromotionPointRate { get; set; }
    public DateTimeOffset PromotionStartTime
    {
        get => _startTime;
        set => _startTime = new DateTimeOffset(value.DateTime, TimeSpan.Zero);
    }
    public DateTimeOffset PromotionEndTime
    {
        get => _endTime;
        set => _endTime = new DateTimeOffset(value.DateTime, TimeSpan.Zero);
    }
}

public class SimpleTurnoverPointSetting
{
    private DateTimeOffset _startTime { get; set; }
    private DateTimeOffset _endTime { get; set; }
    
    public int TpCategoryId { get; set; }
    public int VipLevel { get; set; }
    public string VipLevelName { get; set; }
    public decimal Turnover { get; set; }
    public decimal PointRate { get; set; }
    public int PromotionStatus { get; set; }
    public decimal PromotionPointRate { get; set; }
    public DateTimeOffset PromotionStartTime
    {
        get => _startTime;
        set => _startTime = new DateTimeOffset(value.DateTime, TimeSpan.Zero);
    }
    public DateTimeOffset PromotionEndTime
    {
        get => _endTime;
        set => _endTime = new DateTimeOffset(value.DateTime, TimeSpan.Zero);
    }
}

public class TurnoverPointSettingResult
{
    public int VipLevel { get; set; }
    public string VipLevelName { get; set; }
    public IEnumerable<TpCategoryTurnoverPointSetting> TpCategoryTurnoverPointSetting { get; set; }
}

public class TpCategoryTurnoverPointSetting
{
    public int TpCategoryId { get; set; }
    public decimal Turnover { get; set; }
    public decimal PointRate { get; set; }
    public int PromotionStatus { get; set; }
    public decimal PromotionPointRate { get; set; }
    public DateTimeOffset PromotionStartTime { get; set; }
    public DateTimeOffset PromotionEndTime { get; set; }
}

public class DepositPointSetting : LastUpdateModel
{
    private DateTimeOffset _startTime { get; set; }
    private DateTimeOffset _endTime { get; set; }
    
    [JsonIgnore]
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public string IsoCode { get; set; }
    public decimal DepositAmount { get; set; }
    [JsonIgnore]
    public decimal PointRate { get; set; } // 這是無優惠的原始值，為了不讓前端混亂暫時不回傳
    public bool PromotionStatus { get; set; }
    public decimal PromotionPointRate { get; set; }
    public DateTimeOffset PromotionStartTime
    {
        get => _startTime;
        set => _startTime = new DateTimeOffset(value.DateTime, TimeSpan.Zero);
    }
    public DateTimeOffset PromotionEndTime
    {
        get => _endTime;
        set => _endTime = new DateTimeOffset(value.DateTime, TimeSpan.Zero);
    }
}

public class TurnoverPointStatusSetting : LastUpdateModel
{
    public int TpGameTypeId { get; set; }
    public string TpGameTypeName { get; set; }
    public int Status { get; set; }
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
    public string CaculateDate { get; set; }
    public DateTimeOffset CurrencyTimezonePromotionStartTime { get; set; }
    public DateTimeOffset CurrencyTimezonePromotionEndTime { get; set; }
    public bool IsInPromotionPeriod { get; set; }
}