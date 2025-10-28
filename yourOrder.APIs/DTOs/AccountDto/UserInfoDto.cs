namespace yourOrder.APIs.DTOs.Account
{
    public class UserInfoDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Email { get; set; }
        public bool IsDeleted { get; set; }
        public IList<string> Roles { get; set; }
    }
}
