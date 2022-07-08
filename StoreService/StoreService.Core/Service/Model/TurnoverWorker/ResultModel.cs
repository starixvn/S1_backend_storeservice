using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreService.Core.Service.Model;

namespace StoreService.Core.Service.Model.TurnoverWorker;

public class TurnoverPeriodForWorkerUse //: TurnoverPeriod
{
    //TODO：繼承不到...
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public int RedeemHours { get; set; }
    public DateTime LastSettledTime { get; set; }
    public string TimeZone { get; set; }
    public DateTime CurrencyNowTime { get; set; }
    public DateTime CurrencyLastSettledTime { get; set; }
}

public class TurnoverCustListModel
{
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
    public int VipLevel { get; set; }
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
    public decimal Rate { get; set; }
    public decimal UsedTurnover { get; set; }
    public decimal UnUseTurnOver { get; set; }
    public decimal AddPoint { get; set; }
}
