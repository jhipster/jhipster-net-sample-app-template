using System.Security.Authentication;

namespace JHipsterNetSampleApplication.Security {
    public class UsernameNotFoundException : AuthenticationException {
        public UsernameNotFoundException(string message) : base(message)
        {
        }
    }
}
