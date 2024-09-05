namespace PINrad.Models
{
    public class Asset
    {
        public int AssetId { get; set; }
        public int? InventarniBroj { get; set; }
        public string Opis { get; set; } = string.Empty;
        public decimal KupovnaVrijednost { get; set; }
        public DateTime DatumKupnje { get; set; }

        // Povezani korisnik
        public int? UserID { get; set; }  
        public CustomUser? AssignedUser { get; set; }

    }
}
