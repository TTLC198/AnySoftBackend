namespace RPM_PR_LIB;

public class Permission : BaseModel 
{
    public override long Id { get; set; }
    public string resource { get; set; }
    public List<Role> Roles { get; set; }
}