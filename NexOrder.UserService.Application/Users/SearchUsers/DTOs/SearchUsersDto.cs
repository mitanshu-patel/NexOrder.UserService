namespace NexOrder.UserService.Application.Users.SearchUsers.DTOs
{
    public record SearchUsersDto
    {
        public int Id { get; init; }

        public string Email { get; init; }

        public string Name { get; init; }
    }
}
