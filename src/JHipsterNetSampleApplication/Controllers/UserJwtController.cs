using JHipsterNetSampleApplication.Models.Vm;
using JHipsterNetSampleApplication.Security.Jwt;
using JHipsterNetSampleApplication.Service;
using JHipsterNetSampleApplication.Web.Extensions;
using JHipsterNetSampleApplication.Web.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace JHipsterNetSampleApplication.Controllers {
    [Route("api")]
    [ApiController]
    public class UserJwtController : ControllerBase {
        private readonly IAuthenticationService _authenticationService;

        private readonly ILogger<UserJwtController> _log;
        private readonly ITokenProvider _tokenProvider;

        public UserJwtController(ILogger<UserJwtController> log, IAuthenticationService authenticationService,
            ITokenProvider tokenProvider)
        {
            _log = log;
            _authenticationService = authenticationService;
            _tokenProvider = tokenProvider;
        }

        [HttpPost("authenticate")]
        [ValidateModel]
        public async Task<ActionResult<JwtToken>> Authorize([FromBody] LoginVm loginVm)
        {
            var user = await _authenticationService.Authenticate(loginVm.Username, loginVm.Password);
            var rememberMe = loginVm.RememberMe;
            var jwt = _tokenProvider.CreateToken(user, rememberMe);
            var httpHeaders = new HeaderDictionary {
                [JwtConstants.AuthorizationHeader] = $"{JwtConstants.BearerPrefix} {jwt}"
            };
            return Ok(new JwtToken(jwt)).WithHeaders(httpHeaders);
        }
    }

    public class JwtToken {
        public JwtToken(string idToken)
        {
            IdToken = idToken;
        }

        [JsonProperty("id_token")] private string IdToken { get; }
    }
}
