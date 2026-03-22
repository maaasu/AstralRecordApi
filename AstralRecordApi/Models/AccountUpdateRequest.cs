namespace AstralRecordApi.Models;

public class AccountUpdateRequest
{
    public string? AccountName { get; set; }
    public bool? IsActive { get; set; }
    public byte? Mode { get; set; }
    public Guid UpdatedBy { get; set; }
}
