namespace NexOrder.UserService.Application.Users.GetUser.DTOs
{
    public record GetUserDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }
    }
}
