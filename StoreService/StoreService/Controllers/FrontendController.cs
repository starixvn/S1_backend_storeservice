using Microsoft.AspNetCore.Mvc;
using StoreService.Core.Service.BLL;
using StoreService.Core.Service.Model;
using StoreService.Core.Service.Model.Enums;
using StoreService.Core.Service.Model.Frontend;
using StoreService.Core.Service.Model.Member;
using StoreService.Core.Service.Model.Redemption;
using GetListRequest = StoreService.Core.Service.Model.Frontend.GetListRequest;

namespace StoreService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FrontendController : ControllerBase
    {
        private readonly GiftManagementBLL _giftManagementBll;
        private readonly MemberBLL _memberBll;
        private readonly RedemptionBLL _redemptionBll;

        public FrontendController(IServiceProvider provider)
        {
            _giftManagementBll = provider.GetRequiredService<GiftManagementBLL>();
            _memberBll = provider.GetRequiredService<MemberBLL>();
            _redemptionBll = provider.GetRequiredService<RedemptionBLL>();
        }

        [HttpPost]
        public async Task<ApiResult> GetGiftListAsync(GetListRequest request)
        {
            var result =
                await _giftManagementBll.GetGiftListAsync(new FrontendGetListParams(request));

            return new ApiResult<IEnumerable<GiftListModel>>
            {
                Data = result,
                ErrorCode = (int)ErrorCodeEnum.Success
            };
        }
        
        [HttpPost]
        public async Task<ApiResult> SetFavoriteAsync(SetFavoriteRequest request)
        {
            var success =
                await _memberBll.SetCustFavoriteGiftAsync(new SetFavoriteParams(request));

            return new ApiResult(success ? ErrorCodeEnum.Success : ErrorCodeEnum.Failed);
        }

        [HttpPost]
        public async Task<ApiResult> GetMyPurchesAsync(GetMyPurchesRequest request)
        {
            var result = await _redemptionBll.GetMyPurchesAsync(new GetMyPurchesParams(request));

            return new ApiResult<MyPurchesResult>
            {
                Data = result,
                ErrorCode = (int)ErrorCodeEnum.Success
            };
        }
    }
}
