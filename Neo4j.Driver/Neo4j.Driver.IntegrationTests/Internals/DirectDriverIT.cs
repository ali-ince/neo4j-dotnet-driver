using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4j.Driver.IntegrationTests.Internals;
using Neo4j.Driver.V1;
using Xunit.Abstractions;

namespace Neo4j.Driver.IntegrationTests
{
    public abstract class DirectDriverIT: IDisposable
    {
        protected string Label { get; }

        protected ITestOutputHelper Output { get; }
        protected StandAlone Server { get; }
        protected Uri ServerEndPoint { get; }
        protected IAuthToken AuthToken { get; }


        protected DirectDriverIT(ITestOutputHelper output)
        {
            Output = output;

            string postfix = ITHelper.Instance.RegisterStandAlone();
            Label = $"Label{postfix}";

            Debug.WriteLine($"Label is {Label}");

            Server = ITHelper.Instance.StandAlone;
            ServerEndPoint = Server.BoltUri;
            AuthToken = Server.AuthToken;
        }

        public void Dispose()
        {
            ITHelper.Instance.UnregisterStandAlone();
        }

    }
}
