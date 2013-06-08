using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Reader.Domain.Configuration
{
    public class UserSettingsSection : ConfigurationSection
    {
        [ConfigurationProperty("serviceUrl")]
        public ServiceUrlElement ServiceUrl
        {
            get
            {
                return (ServiceUrlElement)this["serviceUrl"];
            }
            set
            {
                this["serviceUrl"] = value;
            }
        }

        [ConfigurationProperty("account")]
        public AccountElement Account
        {
            get
            {
                return (AccountElement)this["account"];
            }
            set
            { 
                this["account"] = value; 
            }
        }

        [ConfigurationProperty("connectionString")]
        public ConnectionStringElement ConnectionString
        {
            get
            {
                return (ConnectionStringElement)this["connectionString"];
            }
            set
            {
                this["connectionString"] = value;
            }
        }

        public class AccountElement : ConfigurationElement
        {
            [ConfigurationProperty("username", IsRequired = true)]
            public string Username
            {
                get
                {
                    return (string)this["username"];
                }
                set
                {
                    this["username"] = value;
                }
            }

            [ConfigurationProperty("password", IsRequired = true)]
            public string Password
            {
                get
                {
                    return (string)this["password"];
                }
                set
                {
                    this["password"] = value;
                }
            }
        }

        public class ConnectionStringElement : ConfigurationElement
        {
            [ConfigurationProperty("value", IsRequired = true)]
            public string Value
            {
                get
                {
                    return (string)this["value"];
                }
                set
                {
                    this["value"] = value;
                }
            }
        }

        public class ServiceUrlElement : ConfigurationElement
        {
            [ConfigurationProperty("value", IsRequired = true)]
            public string Value
            {
                get
                {
                    return (string)this["value"];
                }
                set
                {
                    this["value"] = value;
                }
            }
        }
    }
}
