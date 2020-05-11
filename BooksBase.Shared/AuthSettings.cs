namespace BooksBase.Shared
{
    public class AuthSettings
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Secret { get; set; }
        public int ExpirationTimeHours { get; set; }
    }
}
