namespace NexOrder.UserService.Application.Users.UpdateUser
{
    public record UpdateUserCriteria
    {
        public string Name { get; init; }

        public string Email { get; init; }

        public string Password { get; init; }
    }
}
