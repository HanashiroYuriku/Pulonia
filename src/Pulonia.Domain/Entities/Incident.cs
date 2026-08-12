using System.ComponentModel;
using System.Reflection;
using Pulonia.Domain.Enums;

namespace Pulonia.Domain.Entities;

public class Incident
{
    public int Id { get; set; }
    public string StoreCode { get; set; } = string.Empty;
    public IncidentCategory IncidentCategory { get; set; }
    public DateTime DownTime { get; set; }
    public DateTime? UpTime { get; set; }
    public bool IsAcknowledged { get; set; } = false;
    public string? AcknowledgedBy { get; set; }
    public DateTime? AcknowledgedTime { get; set; }
    public string? AcknowledgedNote { get; set; }

    // public string CategoryDescription => 

    private string GetEnumDescription(Enum val)
    {
        var field = val.GetType().GetField(val.ToString());

        if (field == null) return val.ToString();

        var attribute = field.GetCustomAttribute<DescriptionAttribute>();
        return attribute == null ? val.ToString() : attribute.Description;
    }
}
