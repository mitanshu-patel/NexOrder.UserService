using NexOrder.UserService.Application.Users.SearchUsers.DTOs;

namespace NexOrder.UserService.Application.Users.SearchUsers
{
    public record SearchUsersResult(List<SearchUsersDto> Users, int TotalRecords);
}
