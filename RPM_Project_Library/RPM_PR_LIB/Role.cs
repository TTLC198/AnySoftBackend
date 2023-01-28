namespace RPM_PR_LIB;

public class Role : BaseModel
{
    public override long Id { get; set; }
    public string name { get; set; }
    public List<User> Users { get; set; } = new();
    public List<Permission> Permissions { get; set; }
}