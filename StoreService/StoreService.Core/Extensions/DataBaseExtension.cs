using System.Data;
using Dapper;

namespace StoreService.Extensions;

public static class DataBaseExtension
{
    public static DynamicParameters AddPagerParams(this DynamicParameters para, int pageSize, int startIndex)
    {
        para.Add("_PageSize", pageSize);
        para.Add("_StartIndex", startIndex);
        para.Add("_TotalPages", dbType: DbType.Int32, direction: ParameterDirection.Output);
        para.Add("_RowCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

        return para;
    }
}