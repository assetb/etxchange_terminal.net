namespace AltaBO
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string FullName { get; set; }
        public int CompanyId { get; set; }
        public int CustomerId { get; set; }
        public int SupplierId { get; set; }
        public int BrokerId { get; set; }
        public int PersonId { get; set; }
        public string Role { get; set; }
    }
}
