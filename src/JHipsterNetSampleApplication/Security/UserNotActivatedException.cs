using System.Security.Authentication;

namespace JHipsterNetSampleApplication.Security {
    public class UserNotActivatedException : AuthenticationException {
        public UserNotActivatedException(string message) : base(message)
        {
        }
    }
}
