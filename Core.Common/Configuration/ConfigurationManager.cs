using System.Collections.Generic;
using System.Collections.Specialized;

namespace Core.Common.Configuration {
    public class ConfigurationManager {
        static ConfigurationManager() {
            AppSettings = new NameValueCollection();
            ConnectionStrings = new Dictionary<string, ConfigConnection>();
        }
        public static NameValueCollection AppSettings { get; set; }
        public static Dictionary<string, ConfigConnection> ConnectionStrings { get; set; }
    }

    public class ConfigConnection {
        public string ConnectionString { get; set; }
    }
}
