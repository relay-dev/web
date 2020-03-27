using Core.Plugins.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests.Providers
{
    /// This cannot be Auto Mocked, as IConfiguration methods used by AzureConnectionStringByConfigurationProvider are static extension methods and cannot be mocked
    /// Instead, we'll create a type down below that exhibits the behavior we'd otherwise mock
    [TestFixture]
    public class ConnectionStringProviderTests
    {
        [Test]
        public void GetConnectionString_ShouldReplaceAllPlaceholders_WhenPlaceholdersExist()
        {
            // Arrange
            const string connectionName = "DefaultConnection";
            const string expectedUsername = "This is the username";
            const string expectedPassword = "This is the password";

            var testConfigurationData = new Dictionary<string, string>
            {
                { "ConnectionStrings.DefaultConnection", "Data Source=some.server;Initial Catalog=MyDatabase;User ID={{DatabaseUsername}};Password={{DatabasePassword}};" },
                { "DatabaseUsername", expectedUsername },
                { "DatabasePassword", expectedPassword }
            };

            TestConfiguration TestConfiguration = new TestConfiguration(testConfigurationData);

            var cut = new Microservices.Providers.AzureConnectionStringByConfigurationProvider(TestConfiguration);

            // Act
            string actual = cut.Get(connectionName);

            // Assert
            Assert.IsFalse(actual.Contains("{{"));
            Assert.IsFalse(actual.Contains("}}"));
        }

        [Test]
        public void GetConnectionString_ShouldReplaceLeadingPlaceholderAsExpected_WhenPlaceholdersExist()
        {
            // Arrange
            const string connectionName = "DefaultConnection";
            const string expectedUsername = "This is the username";
            const string expectedPassword = "This is the password";

            var testConfigurationData = new Dictionary<string, string>
            {
                { "ConnectionStrings.DefaultConnection", "Data Source=some.server;Initial Catalog=MyDatabase;User ID={{DatabaseUsername}};Password={{DatabasePassword}};" },
                { "DatabaseUsername", expectedUsername },
                { "DatabasePassword", expectedPassword }
            };

            TestConfiguration TestConfiguration = new TestConfiguration(testConfigurationData);

            var cut = new Microservices.Providers.AzureConnectionStringByConfigurationProvider(TestConfiguration);

            // Act
            string actual = cut.Get(connectionName);

            // Assert
            Assert.IsTrue(actual.Contains(expectedUsername));
        }

        [Test]
        public void GetConnectionString_ShouldReplaceTrailingPlaceholderAsExpected_WhenPlaceholdersExist()
        {
            // Arrange
            const string connectionName = "DefaultConnection";
            const string expectedUsername = "This is the username";
            const string expectedPassword = "This is the password";

            var testConfigurationData = new Dictionary<string, string>
            {
                { "ConnectionStrings.DefaultConnection", "Data Source=some.server;Initial Catalog=MyDatabase;User ID={{DatabaseUsername}};Password={{DatabasePassword}};" },
                { "DatabaseUsername", expectedUsername },
                { "DatabasePassword", expectedPassword }
            };

            TestConfiguration TestConfiguration = new TestConfiguration(testConfigurationData);

            var cut = new Microservices.Providers.AzureConnectionStringByConfigurationProvider(TestConfiguration);

            // Act
            string actual = cut.Get(connectionName);

            // Assert
            Assert.IsTrue(actual.Contains(expectedPassword));
        }

        [Test]
        public void GetConnectionString_ShouldNotThrowAnException_WhenNoPlaceholdersExist()
        {
            // Arrange
            const string connectionName = "DefaultConnection";
            const string expectedUsername = "This is the username";
            const string expectedPassword = "This is the password";

            var testConfigurationData = new Dictionary<string, string>
            {
                { "ConnectionStrings.DefaultConnection", "Data Source=some.server;Initial Catalog=MyDatabase;User ID=NotAPlaceholder;Password=NotAPlaceholder;" },
                { "DatabaseUsername", expectedUsername },
                { "DatabasePassword", expectedPassword }
            };

            TestConfiguration TestConfiguration = new TestConfiguration(testConfigurationData);

            var cut = new Microservices.Providers.AzureConnectionStringByConfigurationProvider(TestConfiguration);

            // Act
            cut.Get(connectionName);

            // Assert
            Assert.Pass();
        }
    }

    public class TestConfiguration : IConfiguration
    {
        private readonly Dictionary<string, string> _testConfiguration;

        public TestConfiguration(Dictionary<string, string> testConfiguration)
        {
            _testConfiguration = testConfiguration;
        }

        public string this[string key]
        {
            get
            {
                return _testConfiguration[key];
            }
            set
            {
                _testConfiguration[key] = value;
            }
        }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            throw new NotImplementedException();
        }

        public IChangeToken GetReloadToken()
        {
            throw new NotImplementedException();
        }

        public IConfigurationSection GetSection(string key)
        {
            var filteredDictionary = _testConfiguration.Where(kvp => kvp.Key.StartsWith(key + ".")).ToDictionary(kvp => kvp.Key.Remove(key + "."), kvp => kvp.Value);

            return new TestConfigurationSection(filteredDictionary);
        }
    }

    public class TestConfigurationSection : IConfigurationSection
    {
        private readonly Dictionary<string, string> _testConfiguration;

        public TestConfigurationSection(Dictionary<string, string> testConfiguration)
        {
            _testConfiguration = testConfiguration;
        }

        public string this[string key]
        {
            get
            {
                return _testConfiguration[key];
            }
            set
            {
                _testConfiguration[key] = value;
            }
        }

        public string Key => throw new NotImplementedException();

        public string Path => throw new NotImplementedException();

        public string Value { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            throw new NotImplementedException();
        }

        public IChangeToken GetReloadToken()
        {
            throw new NotImplementedException();
        }

        public IConfigurationSection GetSection(string key)
        {
            throw new NotImplementedException();
        }
    }
}
