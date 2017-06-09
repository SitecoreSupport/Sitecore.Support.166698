using Sitecore.Services.Core.Security;
using Sitecore.Services.Infrastructure.Web.Http.Security;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Sitecore.Services.Infrastructure.Sitecore.Security;
using Sitecore.Services.Infrastructure.Sitecore;
using Sitecore.Services.Infrastructure;
using Sitecore.Services;
using Sitecore;

namespace Sitecore.Support.Services.Infrastructure.Sitecore.Security
{
    public class TokenDelegatingHandler : DelegatingHandler
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly IUserService _userService;

        public TokenDelegatingHandler() : this(new ConfiguredOrNullTokenProvider(new SigningTokenProvider()), new UserService())
        {

        }

        protected TokenDelegatingHandler(ITokenProvider tokenProvider, IUserService userService)
      : base()
        {
            _tokenProvider = tokenProvider;
            _userService = userService;
        }

        protected TokenDelegatingHandler(HttpMessageHandler innerHandler)
      : this(innerHandler, new ConfiguredOrNullTokenProvider(new SigningTokenProvider()), new UserService())
        {

        }

        protected TokenDelegatingHandler(HttpMessageHandler innerHandler, ITokenProvider tokenProvider, IUserService userService)
          : base(innerHandler)
        {
            _tokenProvider = tokenProvider;
            _userService = userService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            AttemptLoginWithToken(request);

            return await base.SendAsync(request, cancellationToken);
        }


        private void AttemptLoginWithToken(HttpRequestMessage request)
        {
            if (request.Headers.Contains("token"))
            {
                var token = _tokenProvider.ValidateToken(request.Headers.GetValues("token").FirstOrDefault());
                if (token.IsValid && token.Claims.Where(c => c.Type == "User").Count() == 1)
                {
                    //Support code start - creating our own user service here to avoid overriding the default functionality
                    var _supportUserService = new global::Sitecore.Support.Services.Infrastructure.Sitecore.Security.UserService();
                    var userName = token.Claims.First(c => c.Type == "User").Value;
                    _supportUserService.SwitchToUser(userName, true);
                    //Support code end
                }
            }
        }
    }
}
