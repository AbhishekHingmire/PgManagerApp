using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PgManagerApp.Models.Room
{
    public class RoomViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="Enter room number/name")]
        [Display(Name = "Room Number/Name*")]
        public string RoomNumber { get; set; }

        [Required(ErrorMessage = "Enter room capacity")]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0.")]
        [Display(Name = "Capacity*")]
        public string Capacity { get; set; }

        [NotMapped]
        public string? RemainingSpace { get; set; }

        [Display(Name = "Hot Water")]
        public bool HasHotWater { get; set; } = false;

        [Display(Name = "Shower")]
        public bool HasShower { get; set; } = false;

        [Display(Name = "Commode")]
        public bool HasCommode { get; set; } = false;

        [Display(Name = "WiFi")]
        public bool HasWiFi { get; set; } = false;

        [NotMapped]
        public bool IsEditable { get; set; } = false;

        [NotMapped]
        public List<RoomViewModel>? Rooms { get; set; }
        public int? MasterId { get; set; }
    }
}
