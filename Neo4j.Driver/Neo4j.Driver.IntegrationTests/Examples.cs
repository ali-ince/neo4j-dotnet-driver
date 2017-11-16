﻿// Copyright (c) 2002-2017 "Neo Technology,"
// Network Engine for Objects in Lund AB [http://neotechnology.com]
// 
// This file is part of Neo4j.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
//The only imported needed for using this driver
using Neo4j.Driver.V1;
using Neo4j.Driver.IntegrationTests;
using Xunit;
using Xunit.Abstractions;

namespace Neo4j.Driver.Examples
{
    /// <summary>
    /// The driver examples since 1.2 driver
    /// </summary>
    public class Examples
    {

        public class AutocommitTransactionExample : BaseExample
        {
            public AutocommitTransactionExample(ITestOutputHelper output)
                : base(output)
            {
            }

            // tag::autocommit-transaction[]
            public void AddPerson(string name)
            {
                using (var session = Driver.Session())
                {
                    session.Run("CREATE (a:Person {name: $name})", new {name});
                }
            }
            // end::autocommit-transaction[]

            [RequireServerFact]
            public void TestAutocommitTransactionExample()
            {
                // Given & When
                AddPerson("Alice");
                // Then
                CountPerson("Alice").Should().Be(1);
            }
        }

        public class BasicAuthExample : BaseExample
        {
            public BasicAuthExample(ITestOutputHelper output)
                : base(output)
            {
            }

            // tag::basic-auth[]
            public IDriver CreateDriverWithBasicAuth(string uri, string user, string password)
            {
                return GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
            }
            // end::basic-auth[]

            [RequireServerFact]
            public void TestBasicAuthExample()
            {
                // Given
                using (var driver = CreateDriverWithBasicAuth(Uri, User, Password))
                using (var session = driver.Session())
                {
                    // When & Then
                    session.Run("RETURN 1").Single()[0].As<int>().Should().Be(1);
                }
            }
        }

        public class ConfigConnectionPoolExample : BaseExample
        {
            public ConfigConnectionPoolExample(ITestOutputHelper output, StandAloneIntegrationTestFixture fixture)
                : base(output)
            {
            }

            // tag::config-connection-pool[]
            public IDriver CreateDriverWithCustomizedConnectionPool(string uri, string user, string password)
            {
                return GraphDatabase.Driver(uri, AuthTokens.Basic(user, password),
                    new Config
                    {
                        MaxConnectionLifetime = TimeSpan.FromMinutes(30),
                        MaxConnectionPoolSize = 50,
                        ConnectionAcquisitionTimeout = TimeSpan.FromMinutes(2)
                    });
            }
            // end::config-connection-pool[]

            [RequireServerFact]
            public void TestConfigConnectionPoolExample()
            {
                // Given
                using (var driver = CreateDriverWithCustomizedConnectionPool(Uri, User, Password))
                using (var session = driver.Session())
                {
                    // When & Then
                    session.Run("RETURN 1").Single()[0].As<int>().Should().Be(1);
                }
            }
        }

        public class ConfigLoadBalancingStrategyExample : BaseExample
        {
            public ConfigLoadBalancingStrategyExample(ITestOutputHelper output)
                : base(output)
            {
            }

            // tag::config-load-balancing-strategy[]
            public IDriver CreateDriverWithCustomizedLoadBalancingStrategy(string uri, string user, string password)
            {
                return GraphDatabase.Driver(uri, AuthTokens.Basic(user, password),
                    new Config
                    {
                        LoadBalancingStrategy = LoadBalancingStrategy.LeastConnected
                    });
            }
            // end::config-load-balancing-strategy[]

            [RequireServerFact]
            public void TestConfigLoadBalancingStrategyExample()
            {
                // Given
                using (var driver = CreateDriverWithCustomizedLoadBalancingStrategy(Uri, User, Password))
                using (var session = driver.Session())
                {
                    // When & Then
                    session.Run("RETURN 1").Single()[0].As<int>().Should().Be(1);
                }
            }
        }

        public class ConfigConnectionTimeoutExample : BaseExample
        {
            public ConfigConnectionTimeoutExample(ITestOutputHelper output)
                : base(output)
            {
            }

            // tag::config-connection-timeout[]
            public IDriver CreateDriverWithCustomizedConnectionTimeout(string uri, string user, string password)
            {
                return GraphDatabase.Driver(uri, AuthTokens.Basic(user, password),
                    new Config {ConnectionTimeout = TimeSpan.FromSeconds(15)});
            }
            // end::config-connection-timeout[]

            [RequireServerFact]
            public void TestConfigConnectionTimeoutExample()
            {
                // Given
                using (var driver = CreateDriverWithCustomizedConnectionTimeout(Uri, User, Password))
                using (var session = driver.Session())
                {
                    // When & Then
                    session.Run("RETURN 1").Single()[0].As<int>().Should().Be(1);
                }
            }
        }

        public class ConfigMaxRetryTimeExample : BaseExample
        {
            public ConfigMaxRetryTimeExample(ITestOutputHelper output)
                : base(output)
            {
            }

            // tag::config-max-retry-time[]
            public IDriver CreateDriverWithCustomizedMaxRetryTime(string uri, string user, string password)
            {
                return GraphDatabase.Driver(uri, AuthTokens.Basic(user, password),
                    new Config {MaxTransactionRetryTime = TimeSpan.FromSeconds(15)});
            }
            // end::config-max-retry-time[]

            [RequireServerFact]
            public void TestConfigMaxRetryTimeExample()
            {
                // Given
                using (var driver = CreateDriverWithCustomizedMaxRetryTime(Uri, User, Password))
                using (var session = driver.Session())
                {
                    // When & Then
                    session.Run("RETURN 1").Single()[0].As<int>().Should().Be(1);
                }
            }
        }

        public class ConfigTrustExample : BaseExample
        {
            public ConfigTrustExample(ITestOutputHelper output)
                : base(output)
            {
            }

            // tag::config-trust[]
            public IDriver CreateDriverWithCustomizedTrustStrategy(string uri, string user, string password)
            {
                return GraphDatabase.Driver(uri, AuthTokens.Basic(user, password),
                    new Config {TrustStrategy = TrustStrategy.TrustAllCertificates});
            }
            // end::config-trust[]

            [RequireServerFact]
            public void TestConfigTrustExample()
            {
                // Given
                using (var driver = CreateDriverWithCustomizedTrustStrategy(Uri, User, Password))
                using (var session = driver.Session())
                {
                    // When & Then
                    session.Run("RETURN 1").Single()[0].As<int>().Should().Be(1);
                }
            }
        }

        public class ConfigUnencryptedExample : BaseExample
        {
            public ConfigUnencryptedExample(ITestOutputHelper output)
                : base(output)
            {
            }

            // tag::config-unencrypted[]
            public IDriver CreateDriverWithCustomizedSecurityStrategy(string uri, string user, string password)
            {
                return GraphDatabase.Driver(uri, AuthTokens.Basic(user, password),
                    new Config {EncryptionLevel = EncryptionLevel.None});
            }
            // end::config-unencrypted[]

            [RequireServerFact]
            public void TestConfigUnencryptedExample()
            {
                // Given
                using (var driver = CreateDriverWithCustomizedSecurityStrategy(Uri, User, Password))
                using (var session = driver.Session())
                {
                    // When & Then
                    session.Run("RETURN 1").Single()[0].As<int>().Should().Be(1);
                }
            }
        }

        public class CustomAuthExample : BaseExample
        {
            public CustomAuthExample(ITestOutputHelper output)
                : base(output)
            {
            }

            // tag::custom-auth[]
            public IDriver CreateDriverWithCustomizedAuth(string uri,
                string principal, string credentials, string realm, string scheme, Dictionary<string, object> parameters)
            {
                return GraphDatabase.Driver(uri, AuthTokens.Custom(principal, credentials, realm, scheme, parameters),
                    new Config {EncryptionLevel = EncryptionLevel.None});
            }
            // end::custom-auth[]

            [RequireServerFact]
            public void TestCustomAuthExample()
            {
                // Given
                using (var driver = CreateDriverWithCustomizedAuth(Uri, User, Password, "native", "basic", null))
                using (var session = driver.Session())
                {
                    // When & Then
                    session.Run("RETURN 1").Single()[0].As<int>().Should().Be(1);
                }
            }
        }

        public class KerberosAuthExample : BaseExample
        {
            public KerberosAuthExample(ITestOutputHelper output)
                : base(output)
            {
            }

            // tag::kerberos-auth[]
            public IDriver CreateDriverWithKerberosAuth(string uri, string ticket)
            {
                return GraphDatabase.Driver(uri, AuthTokens.Kerberos(ticket),
                    new Config { EncryptionLevel = EncryptionLevel.None });
            }
            // end::kerberos-auth[]

            [RequireServerFact]
            public void TestKerberosAuthExample()
            {
                // Given
                using (var driver = CreateDriverWithKerberosAuth(Uri, "kerberos ticket"))
                {
                    // When & Then
                    driver.Should().BeOfType<Internal.Driver>();
                }
            }
        }

        public class CypherErrorExample : BaseExample
        {
            public CypherErrorExample(ITestOutputHelper output)
                : base(output)
            {
            }

            // tag::cypher-error[]
            public int GetEmployeeNumber(string name)
            {
                using (var session = Driver.Session())
                {
                    return session.ReadTransaction(tx => SelectEmployee(tx, name));
                }
            }

            private int SelectEmployee(ITransaction tx, string name)
            {
                try
                {
                    var result = tx.Run("SELECT * FROM Employees WHERE name = $name", new {name});
                    return result.Single()["employee_number"].As<int>();
                }
                catch (ClientException ex)
                {
                    Output.WriteLine(ex.Message);
                    return -1;
                }
            }
            // end::cypher-error[]

            [RequireServerFact]
            public void TestCypherErrorExample()
            {
                // When & Then
                GetEmployeeNumber("Alice").Should().Be(-1);
            }
        }

        public class DriverLifecycleExampleTest : BaseExample
        {
            public DriverLifecycleExampleTest(ITestOutputHelper output)
                : base(output)
            {
            }

            // tag::driver-lifecycle[]
            public class DriverLifecycleExample : IDisposable
            {
                public IDriver Driver { get; }

                public DriverLifecycleExample(string uri, string user, string password)
                {
                    Driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
                }

                public void Dispose()
                {
                    Driver?.Dispose();
                }
            }
            // end::driver-lifecycle[]

            [RequireServerFact]
            public void TestDriverLifecycleExample()
            {
                // Given
                var driver = new DriverLifecycleExample(Uri, User, Password).Driver;
                using (var session = driver.Session())
                {
                    // When & Then
                    session.Run("RETURN 1").Single()[0].As<int>().Should().Be(1);
                }
            }
        }

        public class HelloWorldExampleTest : BaseExample
        {
            public HelloWorldExampleTest(ITestOutputHelper output)
                : base(output)
            {
            }

            [RequireServerFact]
            public void TestHelloWorldExample()
            {
                // Given
                using (var example = new HelloWorldExample(Uri, User, Password))
                {
                    // When & Then
                    example.PrintGreeting("Hello, world");
                }
            }

            // tag::hello-world[]
            public class HelloWorldExample : IDisposable
            {
                private readonly IDriver _driver;

                public HelloWorldExample(string uri, string user, string password)
                {
                    _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
                }

                public void PrintGreeting(string message)
                {
                    using (var session = _driver.Session())
                    {
                        var greeting = session.WriteTransaction(tx =>
                        {
                            var result = tx.Run("CREATE (a:Greeting) " +
                                                "SET a.message = $message " +
                                                "RETURN a.message + ', from node ' + id(a)",
                                new {message});
                            return result.Single()[0].As<string>();
                        });
                        Console.WriteLine(greeting);
                    }
                }

                public void Dispose()
                {
                    _driver?.Dispose();
                }

                public static void Main()
                {
                    using (var greeter = new HelloWorldExample("bolt://localhost:7687", "neo4j", "password"))
                    {
                        greeter.PrintGreeting("hello, world");
                    }
                }
            }
            // end::hello-world[]
        }

        public class ReadWriteTransactionExample : BaseExample
        {
            public ReadWriteTransactionExample(ITestOutputHelper output)
                : base(output)
            {
            }

            [RequireServerFact]
            public void TestReadWriteTransactionExample()
            {
                // When & Then
                AddPerson("Addison").Should().BeGreaterOrEqualTo(0L);
            }

            // tag::read-write-transaction[]
            public long AddPerson(string name)
            {
                using (var session = Driver.Session())
                {
                    session.WriteTransaction(tx => CreatePersonNode(tx, name));
                    return session.ReadTransaction(tx => MatchPersonNode(tx, name));
                }
            }

            private static void CreatePersonNode(ITransaction tx, string name)
            {
                tx.Run("CREATE (a:Person {name: $name})", new {name});
            }

            private static long MatchPersonNode(ITransaction tx, string name)
            {
                var result = tx.Run("MATCH (a:Person {name: $name}) RETURN id(a)", new {name});
                return result.Single()[0].As<long>();
            }
            // end::read-write-transaction[]
        }

        public class ResultConsumeExample : BaseExample
        {
            public ResultConsumeExample(ITestOutputHelper output)
                : base(output)
            {
            }

            // tag::result-consume[]
            public List<string> GetPeople()
            {
                using (var session = Driver.Session())
                {
                    return session.ReadTransaction(tx =>
                    {
                        var result = tx.Run("MATCH (a:Person) RETURN a.name ORDER BY a.name");
                        return result.Select(record => record[0].As<string>()).ToList();
                    });
                }
            }
            // end::result-consume[]

            [RequireServerFact]
            public void TestResultConsumeExample()
            {
                // Given
                Write("CREATE (a:Person {name: 'Ainsley'})");
                Write("CREATE (a:Person {name: 'Armstrong'})");
                // When & Then
                GetPeople().Should().Contain(new[] { "Ainsley", "Armstrong" });
            }
        }

        public class ResultRetainExample : BaseExample
        {
            public ResultRetainExample(ITestOutputHelper output)
                : base(output)
            {
            }

            // tag::result-retain[]
            public int AddEmployees(string companyName)
            {
                using (var session = Driver.Session())
                {
                    var employees = session.ReadTransaction(tx => tx.Run("MATCH (a:Employee) RETURN a.name AS name").ToList());
                    return employees.Sum(employee => session.WriteTransaction(tx =>
                    {
                        tx.Run("MATCH (emp:Employee {name: $person_name}) " +
                            "MERGE (com:Company {name: $company_name}) " +
                            "MERGE (emp)-[:WORKS_FOR]->(com)",
                            new {person_name = employee["name"].As<string>(), company_name = companyName});
                        return 1;
                    }));
                }
            }
            // end::result-retain[]

            [RequireServerFact]
            public void TestResultConsumeExample()
            {
                // Given
                Write("CREATE (a:Employee {name: 'Alice'})");
                Write("CREATE (a:Employee {name: 'Bob'})");
                // When & Then
                AddEmployees("Acme").Should().Be(2);
                Read("MATCH (emp:Employee)-[WORKS_FOR]->(com:Company) WHERE com.name = 'Acme' RETURN count(emp)")
                    .Single()[0].As<int>().Should().Be(2);
            }
        }

        public class ServiceUnavailableExample: IDisposable
        {
            private readonly IDriver Driver;
            private readonly string User = "neo4j";
            private readonly string Password = "neo4j";

            public ServiceUnavailableExample(ITestOutputHelper output)
            {
                Driver = GraphDatabase.Driver("bolt://localhost:8080", AuthTokens.Basic(User, Password),
                    new Config {MaxTransactionRetryTime = TimeSpan.FromSeconds(3)});
            }

            public void Dispose()
            {
                Driver.Dispose();
            }

            // tag::service-unavailable[]
            public bool AddItem()
            {
                try
                {
                    using (var session = Driver.Session())
                    {
                        return session.WriteTransaction(
                            tx =>
                            {
                                tx.Run("CREATE (a:Item)");
                                return true;
                            }
                        );
                    }
                }
                catch (ServiceUnavailableException)
                {
                    return false;
                }
            }
            // end::service-unavailable[]

            [RequireServerFact]
            public void TestServiceUnavailableExample()
            {
                AddItem().Should().BeFalse();
            }
        }

        public class SessionExample : BaseExample
        {
            public SessionExample(ITestOutputHelper output)
                : base(output)
            {
            }

            // tag::session[]
            public void AddPerson(string name)
            {
                using (var session = Driver.Session())
                {
                    session.Run("CREATE (a:Person {name: $name})", new {name});
                }
            }
            // end::session[]

            [RequireServerFact]
            public void TestSessionExample()
            {
                // Given & When
                AddPerson("Tom");
                // Then
                CountPerson("Tom").Should().Be(1);
            }
        }

        public class TransactionFunctionExample : BaseExample
        {
            public TransactionFunctionExample(ITestOutputHelper output) 
                : base(output)
            {
            }

            // tag::transaction-function[]
            public void AddPerson(string name)
            {
                using (var session = Driver.Session())
                {
                    session.WriteTransaction(tx => tx.Run("CREATE (a:Person {name: $name})", new {name}));
                }
            }
            // end::transaction-function[]

            [RequireServerFact]
            public void TestTransactionFunctionExample()
            {
                // Given & When
                AddPerson("Bailey");
                // Then
                CountPerson("Bailey").Should().Be(1);
            }
        }
    }

    public abstract class BaseExample : DirectDriverIT
    {
        protected IDriver Driver { set; get; }
        protected const string Uri = "bolt://localhost:7687";
        protected const string User = "neo4j";
        protected const string Password = "neo4j";

        protected BaseExample(ITestOutputHelper output) : base(output)
        {
            Driver = Server.Driver;
        }

        protected int CountPerson(string name)
        {
            using (var session = Driver.Session())
            {
                return session.ReadTransaction(
                    tx => tx.Run($"MATCH (a:Person {{ name: $name }}) RETURN count(a)",
                    new { name }).Single()[0].As<int>());
            }
        }

        protected void Write(string statement, IDictionary<string, object> parameters = null)
        {
            using (var session = Driver.Session())
            {
                session.WriteTransaction(tx =>
                    tx.Run(statement, parameters));
            }
        }

        protected IStatementResult Read(string statement, IDictionary<string, object> parameters = null)
        {
            using (var session = Driver.Session())
            {
                return session.WriteTransaction(tx =>
                    tx.Run(statement, parameters));
            }
        }
    }

    // TODO Remove it after we figure out a way to solve the naming problem
    internal static class ValueExtensions
    {
        public static T As<T>(this object value)
        {
            return V1.ValueExtensions.As<T>(value);
        }
    }
}
