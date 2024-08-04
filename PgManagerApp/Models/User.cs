using System.ComponentModel.DataAnnotations;

public class User
{
   
}
public class UserRegistration
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(50, ErrorMessage = "Name can't be longer than 50 characters.")]
    public string? Name { get; set; }

    [StringLength(50, ErrorMessage = "Email can't be longer than 50 characters.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Mobile number is required.")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Enter valid mobile number.")]
    public string? MobileNumber { get; set; }

    [Required(ErrorMessage = "Address is required.")]
    public string? Address { get; set; }

    [Required(ErrorMessage = "Designation is required.")]
    public string? Designation { get; set; }

    [Required(ErrorMessage = "Identity type is required.")]
    public string? IdentityType { get; set; }

    [Required(ErrorMessage = "Identity number is required.")]
    [CustomValidation(typeof(UserRegistration), nameof(ValidateIdentityNumber))]
    public string? IdentityNumber { get; set; }

    public static ValidationResult? ValidateIdentityNumber(string identityNumber, ValidationContext context)
    {
        var instance = context.ObjectInstance as UserRegistration;
        if (instance != null)
        {
            if (instance.IdentityType == "Aadhar" && !System.Text.RegularExpressions.Regex.IsMatch(identityNumber, @"^\d{12}$"))
            {
                return new ValidationResult("Not a valid Aadhar number.");
            }
            else if (instance.IdentityType == "PAN" && !System.Text.RegularExpressions.Regex.IsMatch(identityNumber, @"^[A-Z]{5}[0-9]{4}[A-Z]{1}$"))
            {
                return new ValidationResult("Not a valid PAN number.");
            }
        }
        return ValidationResult.Success;
    }
}