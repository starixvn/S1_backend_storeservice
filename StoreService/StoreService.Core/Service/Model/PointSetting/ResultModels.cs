using StoreService.Core.Service.Model.Based;
using StoreService.Core.Service.Model.Enums;

namespace StoreService.Core.Service.Model.PointSetting;

public class InactivePointExpirySetting : LastUpdateModel
{
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
    private DateTimeOffset _startTime { get; set; }
    private DateTimeOffset _endTime { get; set; }

    public int TpCategoryId { get; set; }
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

public class DepositPointSetting : LastUpdateModel
{
    private DateTimeOffset _startTime { get; set; }
    private DateTimeOffset _endTime { get; set; }

    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public string IsoCode { get; set; }
    public decimal DepositAmount { get; set; }
    public decimal PointRate { get; set; }
    public GeneralStatusEnum PromotionStatus { get; set; }
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
    public int TpCategoryId { get; set; }
    public string TpCategoryName { get; set; }
    public int TpGameTypeId { get; set; }
    public string TpGameTypeName { get; set; }
    public int Status { get; set; }
}