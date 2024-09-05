using Microsoft.AspNetCore.Identity;


    public class RegLogUser : IdentityUser //Useri sa pravima
    {
        public string PunoIme { get; set; } = string.Empty;
        public string Odjel { get; set; } = string.Empty;
        public string Rola { get; set; } = string.Empty;
}
