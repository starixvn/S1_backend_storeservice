using Newtonsoft.Json;

namespace StoreService.Core.Service.Model.PointSetting;

public class GetInactivePointExpirySettingParams
{
    public GetInactivePointExpirySettingParams(GetInactivePointExpirySettingRequest req)
    {
        SiteId = req.SiteId;
    }
    public GetInactivePointExpirySettingParams(int siteId)
    {
        SiteId = siteId;
    }
    public int SiteId { get; set; }
}

public class SetInactivePointExpirySettingParams
{
    public SetInactivePointExpirySettingParams(SetInactivePointExpirySettingRequest req)
    {
        SiteId = req.SiteId;
        Days = req.Days;
        Status = (int)req.Status;
        Operator = req.Operator;
    }
    public int SiteId { get; set; }
    public int Days { get; set; }
    public int Status { get; set; }
    public string Operator { get; set; }
}

public class GetTurnoverPointSettingParams
{
    public GetTurnoverPointSettingParams(GetTurnoverPointSettingRequest req)
    {
        SiteId = req.SiteId;
        CurrencyId = req.CurrencyId;
        VipLevel = req.VipLevel;
    }
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public int VipLevel { get; set; }
}

public class GetTurnoverPointSettingListParams
{
    public GetTurnoverPointSettingListParams(GetTurnoverPointSettingListRequest req)
    {
        SiteId = req.SiteId;
        CurrencyId = req.CurrencyId;
    }
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
}

public class SetTurnoverPointSettingParams
{
    public SetTurnoverPointSettingParams(SetTurnoverPointSettingRequest req)
    {
        SiteId = req.SiteId;
        CurrencyId = req.CurrencyId;
        StrInfos = JsonConvert.SerializeObject(req.Infos);
        Operator = req.Operator;
    }
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public string StrInfos { get; set; }
    public string Operator { get; set; }
}

public class TurnoverPointSettingParams
{
    private DateTimeOffset? _startTime { get; set; }
    private DateTimeOffset? _endTime { get; set; }

    public int TpCategoryId { get; set; }
    public int VipLevel { get; set; }
    public decimal Turnover { get; set; }
    public decimal PointRate { get; set; }
    public int PromotionStatus { get; set; }
    public decimal PromotionPointRate { get; set; }

    public DateTimeOffset? PromotionStartTime
    {
        get => _startTime;
        set
        {
            if (value != null)
                _startTime = new DateTimeOffset(value.Value.ToUniversalTime().DateTime, TimeSpan.Zero);
        }
    }
    public DateTimeOffset? PromotionEndTime
    {
        get => _endTime;
        set
        {
            if (value != null)
                _endTime = new DateTimeOffset(value.Value.ToUniversalTime().DateTime, TimeSpan.Zero);
        }
    }
}

public class GetDepositPointSettingParams
{
    public GetDepositPointSettingParams()
    {
    }
    public GetDepositPointSettingParams(GetDepositPointSettingRequest req)
    {
        SiteId = req.SiteId;
    }
    public int SiteId { get; set; }
}

public class SetDepositPointSettingParams
{
    public SetDepositPointSettingParams(SetDepositPointSettingRequest req)
    {
        SiteId = req.SiteId;
        CurrencyId = req.CurrencyId;
        DepositAmount = req.DepositAmount;
        PointRate = 1; //req.PointRate; 暫時不會開放變更，先寫死
        PromotionStatus = (int)req.PromotionStatus;
        PromotionPointRate = req.PromotionPointRate;
        PromotionStartTime = req.PromotionStartTime != new DateTimeOffset() ? req.PromotionStartTime?.DateTime : null;
        PromotionEndTime = req.PromotionEndTime != new DateTimeOffset() ? req.PromotionEndTime?.DateTime : null;
        Operator = req.Operator;
    }
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public decimal DepositAmount { get; set; }
    public decimal PointRate { get; set; }

    /// <summary>
    /// 0: Enable, 1: Disabled
    /// </summary>
    public int PromotionStatus { get; set; }
    public decimal PromotionPointRate { get; set; }
    public DateTime? PromotionStartTime { get; set; }
    public DateTime? PromotionEndTime { get; set; }
    public string Operator { get; set; }
}

public class GetTurnoverPointStatusSettingParams
{
    public GetTurnoverPointStatusSettingParams(GetTurnoverPointStatusSettingRequest req)
    {
        SiteId = req.SiteId;
        CurrencyId = req.CurrencyId;
        TpCategoryId = req.TpCategoryId;
    }
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public int TpCategoryId { get; set; }
}

public class SetTurnoverPointStatusSettingParams
{
    public SetTurnoverPointStatusSettingParams(SetTurnoverPointStatusSettingRequest req)
    {
        SiteId = req.SiteId;
        CurrencyId = req.CurrencyId;
        StrInfos = JsonConvert.SerializeObject(req.Infos);
        Operator = req.Operator;
    }
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public string StrInfos { get; set; }
    public string Operator { get; set; }
}

public class TurnoverPointStatusParams
{
    public int TpGameTypeId { get; set; }
    /// <summary>
    /// 0: Enable, 1: Disabled
    /// </summary>
    public int Status { get; set; }
}