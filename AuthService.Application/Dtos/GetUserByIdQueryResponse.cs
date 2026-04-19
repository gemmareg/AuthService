namespace AuthService.Application.Dtos
{
    public class GetUserByIdQueryResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public List<string> Permissions { get; set; } = new List<string>();
    }
}
