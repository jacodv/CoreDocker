using System.Collections.Generic;
using MediatR;

namespace CoreDocker.Core.Components.Users
{
    public class UserRegisteredNotification : IAsyncNotification
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; internal set; }
    }
}