using Sitecore.Security.Accounts;
using Sitecore.Security.Authentication;
using Sitecore.Services.Core.Security;
using System;
using System.Security.Authentication;
using Sitecore.Services.Infrastructure.Sitecore.Security;
using Sitecore.Services.Infrastructure.Sitecore;
using Sitecore.Services.Infrastructure;
using Sitecore.Services;

namespace Sitecore.Support.Services.Infrastructure.Sitecore.Security
{
    public class UserService : global::Sitecore.Services.Infrastructure.Sitecore.Security.UserService
    {
        public void SwitchToUser(string username, bool authenticate)
        {
            User user = User.FromName(username, authenticate);
            new UserSwitcher(user);
        }

    }
}
