using System.ComponentModel.DataAnnotations;

namespace Watermelon.website.Models;

public class User
{
    [Required] public Guid Id { get; set; }

    [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Данный формат почты невалиден!")]
    public string Email { get; set; }

    [RegularExpression(@"^\+[1-9]{1}[0-9]{3,14}$", ErrorMessage = "Данные номера телефона невалидны")]
    public string Mobile { get; set; }
    public string Password { get; set; }
}