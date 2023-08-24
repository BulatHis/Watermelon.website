using System.ComponentModel.DataAnnotations;
namespace Watermelon.website.Models;

public class BuyModel
{
    [Required]
    public Guid Id { get; set; }
    public string WatermelonName { get; set; }
    public string Address { get; set; }
    public string CountOfWatermelon { get; set; }
    public string Email { get; set; }
    public string Sale { get; set; }
    
}