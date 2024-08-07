using PgManagerApp.Models.Room;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PgManagerApp.Models
{
    [NotMapped]
    public class InvoiceViewModel
    {
        public string? UserName { get; set; }
        public string? MobileNumber { get; set; }
        public string? Email { get; set; }
        public List<RoomDetail>? RoomDetails { get; set; }
        public string? TotalChargeAmount { get; set; }
        public string? TotalPaidAmount { get; set; }
    }

    [NotMapped]
    public class RoomDetail
    {
        public string RoomNumber { get; set; }
        public string ChargeAmount { get; set; }
        public string PaidAmount { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }


    [NotMapped]
    public class UserAddition
    {
        public string UserName { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    [NotMapped]
    public class DashboardViewModel
    {
        public string? TotalUsers { get; set; }
        public string? CountPaid { get; set; }
        public string? CountUnpaid { get; set; }
        public string? TotalAmount { get; set; }
        public string? TotalPendingAmount { get; set; }
    }

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
        public int? MasterId { get; set; }
    }
}
