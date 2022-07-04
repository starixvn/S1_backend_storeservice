using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreService.Core.Service.BLL
{
    internal class WorkerParamBLL
    {

        private MemberService _memberService;
        private readonly ILogger<WorkerParamBLL> _logger;

        public WorkerParamBLL(IServiceProvider provider)
        {
            _memberService = provider.GetRequiredService<MemberService>();
            _logger = provider.GetRequiredService<ILogger<WorkerParamBLL>>();
        }

        public Task<PagerResultModel<MemberRewardPointInfo>> GetMemberRewardPointListAsync(GetListParams input)
            => _memberService.GetMemberRewardPointListAsync(input);

        public Task<PagerResultModel<MemberRewardPointLog>> GetMemberRewardPointLogAsync(GetMemberRewardPointLogParams input)
            => _memberService.GetMemberRewardPointLogAsync(input);
    }
}
