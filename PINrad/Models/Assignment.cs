namespace PINrad.Models
{
    public class Assignment
    {
        public int AssignmentId { get; set; }
        public int? CustomUserId { get; set; }
        public int? AssetId { get; set; }
        public DateTime AssignmentDate { get; set; }

        // Navigacijska svojstva
        public CustomUser? CustomUser { get; set; } 
        public Asset? Asset { get; set; }         
    }
}
