using Microsoft.AspNetCore.Mvc;
using StoreService.Core.Service.BLL;
using StoreService.Core.Service.Model;
using StoreService.Core.Service.Model.Enums;
using StoreService.Core.Service.Model.Frontend;

namespace StoreService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FrontendController : ControllerBase
    {
        private readonly GiftManagementBLL _giftManagementBll;

        public FrontendController(IServiceProvider provider)
        {
            _giftManagementBll = provider.GetRequiredService<GiftManagementBLL>();
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
    }
}
