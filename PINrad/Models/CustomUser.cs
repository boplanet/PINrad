using System.ComponentModel.DataAnnotations;

namespace PINrad.Models
{
    public class CustomUser
    {
        internal object Assignments;

        public int CustomUserId { get; set; }
        [Required]
       // [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        [Required]
        [StringLength(100)]
        public string Department { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        // Navigacijsko svojstvo za imovinu
        public ICollection<Asset> AssignedAssets { get; set; } = new List<Asset>();
    }
}
