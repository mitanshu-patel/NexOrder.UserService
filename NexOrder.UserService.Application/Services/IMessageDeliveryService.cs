using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.UserService.Application.Services
{
    public interface IMessageDeliveryService
    {
        Task PublishMessageAsync<T>(T requestBody, string topicName);
    }
}
