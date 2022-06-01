namespace StoreService.Core.Service.Model.BaseData;

public class TpInfo
{
    public int TpId { get; set; }
    public string TpName { get; set; }
}

public class TpCategory
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class TpCategoryComparer : IEqualityComparer<TpCategory>
{
    public bool Equals(TpCategory t1, TpCategory t2)
    {
        return (t1.Id == t2.Id
            && t1.Name == t2.Name);
    }

    public int GetHashCode(TpCategory t)
    {
        return t.Name.GetHashCode();
    }
}

public class TpGameTypeInfo
{
    public int TpGameTypeId { get; set; }
    public string TpGameTypeIdName { get; set; }
    public int TpId { get; set; }
    public int TpCategoryId { get; set; }
    public string TpCategoryName { get; set; }
}

public class TpGameType
{
    public TpGameType(TpGameTypeInfo data)
    {
        Id = data.TpGameTypeId;
        Name = data.TpGameTypeIdName;
    }
    public int Id { get; set; }
    public string Name { get; set; }
}

public class VipLevel
{
    public int Level { get; set; }
    public string LevelName { get; set; }
}