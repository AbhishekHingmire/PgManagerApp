using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PgManagerApp.Models
{

    public class UserRegistration
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string? Name { get; set; }

        [NotMapped]
        public string? PendingAmount { get; set; }
        public string? Email { get; set; }

        [StringLength(10)]
        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Enter valid mobile number.")]
        public string? MobileNumber { get; set; }

        [NotMapped]
        public string? RoomNumber { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Designation is required.")]
        public string? Designation { get; set; }

        [Required(ErrorMessage = "Identity type is required.")]
        public string? IdentityType { get; set; }
        public string? IdentityNumber { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        [NotMapped]
        public bool ViewOnly { get; set; }
        [NotMapped]
        public List<UserRegistration>? Users { get; set; }
    }
}
