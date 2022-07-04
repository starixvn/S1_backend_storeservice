using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreService.Core.Service.Model.TurnoverPeriod;

public class TurnoverPeriod
{
    public int SiteId { get; set; }
    public int CurrencyId { get; set; }
    public int RedeemHours { get; set; }
    public DateTime LastSettledTime { get; set; }
    public DateTime SpecificDate { get; set; }
}

public class TurnoverPeriodForWorkerUse: TurnoverPeriod
{
    public string TimeZone { get; set; }
    public DateTime CurrencyNowTIme { get; set; }
    public DateTime CurrencyLastSettledTime { get; set; }
}
