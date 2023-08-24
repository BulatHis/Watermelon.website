using System.ComponentModel.DataAnnotations;
namespace Watermelon.website.Models;

public class Comments
{
    [Required]
    public Guid Id { get; set; }
    public string WatermelonName { get; set; }
    public string Email { get; set; }
    public string Comment { get; set; }
    
}