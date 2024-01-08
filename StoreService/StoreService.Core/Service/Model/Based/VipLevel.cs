namespace StoreService.Core.Service.Model.Based;

public class VipLevel
{
    public VipLevel(int id, string name)
    {
        LevelId = id;
        LevelName = name;
    }

    public int LevelId { get; set; }
    public string LevelName { get; set; }
}

public class VipLevelComparer : IEqualityComparer<VipLevel>
{
    public bool Equals(VipLevel t1, VipLevel t2)
    {
        return (t1.LevelId == t2.LevelId
                && t1.LevelName == t2.LevelName);
    }

    public int GetHashCode(VipLevel t)
    {
        return t.LevelName.GetHashCode();
    }
}