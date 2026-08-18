using Pulonia.Domain.Common;

namespace Pulonia.Domain.Entities;

public class Branch : BaseEntity<int>
{
    public string BranchCode { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;

    public Branch()
    {

    }
}