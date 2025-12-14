
using Newtonsoft.Json;

namespace NexOrder.UserService.Application.Users.SearchUsers
{
    public record SearchUsersQuery(int PageNumber, int PageSize, string? SearchText = null)
    {
        [JsonIgnore]
        public int PageIndex { get => this.PageNumber == 0 ? this.PageNumber : this.PageNumber - 1; }
    }
}
