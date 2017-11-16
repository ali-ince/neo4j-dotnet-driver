using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Neo4j.Driver.V1;

namespace Neo4j.Driver.IntegrationTests.Internals
{
    public class ITHelper
    {
        public static readonly ITHelper Instance = new ITHelper();
        
        private ITHelper()
        {
            Debug.WriteLine("NEW INSTANCE OF ITHELPER");

            StandAloneResource = new ITResource<StandAlone>(ResetDatabase, null);
            ClusterResource = new ITResource<CausalCluster>(ResetDatabase, null);
        }

        private ITResource<StandAlone> StandAloneResource { get; }

        private ITResource<CausalCluster> ClusterResource { get; }

        public StandAlone StandAlone => StandAloneResource.Resource;

        public CausalCluster Cluster => ClusterResource.Resource;

        public string RegisterStandAlone()
        {
            return StandAloneResource.Register();
        }

        public void UnregisterStandAlone()
        {
            StandAloneResource.Unregister();
        }

        public string RegisterCluster()
        {
            return ClusterResource.Register();
        }

        public void UnregisterCluster()
        {
            ClusterResource.Unregister();
        }

        private void ResetDatabase(StandAlone standAlone)
        {
            ResetDatabase(standAlone.Driver);
        }

        private void ResetDatabase(CausalCluster cluster)
        {
            var core = cluster.AnyCore();
            using (var driver = GraphDatabase.Driver(core.BoltRoutingUri, cluster.AuthToken))
            {
                ResetDatabase(driver);
            }
        }

        private void ResetDatabase(IDriver driver)
        {
            using (var session = driver.Session(AccessMode.Write))
            {
                session.Run("MATCH (n) DETACH DELETE n").Consume();
            }
        }

        private class ITResource<T> where T: class, IDisposable, new()
        {
            private CancellationTokenSource cts = new CancellationTokenSource();
            private readonly SemaphoreSlim resourceLock = new SemaphoreSlim(1);
            private readonly ManualResetEventSlim resourceEvent = new ManualResetEventSlim(false);
            private int resourceCounter = 0;
            private int resourceSubscriberCounter = 0;

            public ITResource(Action<T> initAction, Action<T> disposeAction)
            {
                InitAction = initAction;
                DisposeAction = disposeAction;
            }

            private Action<T> InitAction { get; }

            private Action<T> DisposeAction { get; }

            public T Resource { get; private set; }

            public string Register()
            {
                var currentResourceCounter = Interlocked.Increment(ref resourceCounter);
                if (currentResourceCounter == 1)
                {
                    resourceLock.Wait(cts.Token);

                    try
                    {
                        try
                        {
                            Resource = new T();

                            InitAction?.Invoke(Resource);

                            resourceEvent.Set();
                        }
                        catch
                        {
                            cts.Cancel();
                        }
                    }
                    finally
                    {
                        resourceLock.Release();
                    }
                }

                resourceEvent.Wait(cts.Token);

                return $"{Interlocked.Increment(ref resourceSubscriberCounter):D4}";
            }

            public void Unregister()
            {
                var currentResourceCounter = Interlocked.Decrement(ref resourceCounter);
                if (currentResourceCounter == 0)
                {
                    resourceLock.Wait(cts.Token);

                    try
                    {
                        if (Resource != null)
                        {
                            try
                            {
                                DisposeAction?.Invoke(Resource);

                                Resource.Dispose();
                            }
                            catch
                            {
                                cts.Cancel();
                            }
                        }
                    }
                    finally
                    {
                        Resource = null;

                        cts = new CancellationTokenSource();

                        resourceLock.Release();

                        resourceEvent.Reset();
                    }
                }
            }

        }
    }
}
