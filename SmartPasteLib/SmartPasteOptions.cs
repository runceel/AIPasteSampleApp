using System.ComponentModel.DataAnnotations;

namespace SmartPasteLib;
public class SmartPasteOptions
{
    [Required]
    public string DeploymentName { get; set; } = "";
    [Required]
    public string Endpoint { get; set; } = "";
    public string? ApiKey { get; set; }
}
