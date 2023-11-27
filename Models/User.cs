namespace UddanelsesAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public byte[] Password { get; set; }
        public byte[] salt { get; set; }
    }
}
