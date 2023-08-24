using System.ComponentModel.DataAnnotations;
namespace Watermelon.website.Models;

public class ShowComments
{
    [Required]
    public string Email { get; set; }
    public string Comment { get; set; }
}