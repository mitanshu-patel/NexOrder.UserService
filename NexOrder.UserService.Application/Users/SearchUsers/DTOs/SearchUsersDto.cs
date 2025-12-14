namespace NexOrder.UserService.Application.Users.SearchUsers.DTOs
{
    public record SearchUsersDto
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }
    }
}
