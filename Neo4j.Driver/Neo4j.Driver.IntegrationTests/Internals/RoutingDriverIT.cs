using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4j.Driver.V1;
using Xunit.Abstractions;

namespace Neo4j.Driver.IntegrationTests.Internals
{
    public abstract class RoutingDriverIT : IDisposable
    {
        protected string Label { get; }

        protected ITestOutputHelper Output { get; }
        protected CausalCluster Cluster { get; }
        //protected Uri ServerEndPoint { get; }
        protected IAuthToken AuthToken { get; }

        protected RoutingDriverIT(ITestOutputHelper output)
        {
            Output = output;

            string postfix = ITHelper.Instance.RegisterCluster();
            Label = $"Label{postfix}";

            Debug.WriteLine($"Label is {Label}");

            Cluster = ITHelper.Instance.Cluster;
            //ServerEndPoint = Server.BoltUri;
            AuthToken = Cluster.AuthToken;
        }

        public void Dispose()
        {
            ITHelper.Instance.UnregisterStandAlone();
        }

    }
}
