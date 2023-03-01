namespace RPM_PR_LIB;

public class ProductJson
{
    public string Name { get; set; }
    public MinMaxObject Rating { get; set; }
    public MinMaxObject Cost { get; set; }
    public int Discount { get; set; }
    public long Category { get; set; }
    public Dictionary<int, List<string>> Attributes { get; set; }
    public int Quantity { get; set; }
    public SortOrder Order { get; set; }
}

public class MinMaxObject
{
    public float Min { get; set; }
    public float Max { get; set; }
}

public class SortOrder
{
    public string Type { get; set; }
    public int Direction { get; set; } // 0 - up, 1 - down
}