namespace JHipsterNet.Config {
    public class JHipsterSettings {
        public Security Security { get; set; } = new Security();
    }

    public class Security {
        public Authentication Authentication { get; set; } = new Authentication();
    }

    public class Authentication {
        public Jwt Jwt { get; set; } = new Jwt();
    }

    public class Jwt {
        public string Secret { get; set; }
        public string Base64Secret { get; set; }
        public int TokenValidityInSeconds { get; set; }
        public int TokenValidityInSecondsForRememberMe { get; set; }
    }
}
