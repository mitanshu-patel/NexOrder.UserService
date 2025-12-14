namespace NexOrder.UserService.Application.Users.UpdateUser
{
    public record UpdateUserCommand(int UserId, UpdateUserCriteria Criteria);
}
