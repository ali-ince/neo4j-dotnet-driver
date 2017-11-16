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
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Neo4j.Driver.Internal;
using Neo4j.Driver.V1;
using Xunit;
using Xunit.Abstractions;

namespace Neo4j.Driver.IntegrationTests
{
    public class BookmarkIT : DirectDriverIT
    {
        private IDriver Driver => Server.Driver;

        public BookmarkIT(ITestOutputHelper output) : base(output)
        {
        }

        [RequireServerVersionGreaterThanOrEqualToFact("3.1.0")]
        public void ShouldContainLastBookmarkAfterTx()
        {
            using (var session = Driver.Session())
            {
                session.LastBookmark.Should().BeNull();

                CreateNodeInTx(session);

                session.LastBookmark.Should().NotBeNull();
                session.LastBookmark.Should().StartWith("neo4j:bookmark:v1:tx");
            }
        }

        [RequireServerVersionGreaterThanOrEqualToFact("3.1.0")]
        public void BookmarkUnchangedAfterRolledBackTx()
        {
            using (var session = Driver.Session())
            {
                CreateNodeInTx(session);
                var bookmark = session.LastBookmark;
                bookmark.Should().NotBeNullOrEmpty();

                using (var tx = session.BeginTransaction())
                {
                    tx.Run($"CREATE (a:{Label})");
                    tx.Failure();
                }
                session.LastBookmark.Should().Be(bookmark);
            }
        }

        [RequireServerVersionGreaterThanOrEqualToFact("3.1.0")]
        public void BookmarkUnchangedAfterTxFailure()
        {
            using (var session = Driver.Session())
            {
                CreateNodeInTx(session);
                var bookmark = session.LastBookmark;
                bookmark.Should().NotBeNullOrEmpty();

                var tx = session.BeginTransaction();
                tx.Run("RETURN");
                tx.Success();
                var exception = Record.Exception(() => tx.Dispose());
                exception.Should().BeOfType<ClientException>();
                session.LastBookmark.Should().Be(bookmark);
            }
        }

        [RequireServerVersionGreaterThanOrEqualToFact("3.1.0")]
        public void ShouldIgnoreButLogInvalidBookmark()
        {
            var invalidBookmark = "invalid bookmark format";
            var loggerMock = new Mock<ILogger>();
            using(var driver = GraphDatabase.Driver(Server.BoltUri, Server.AuthToken, new Config {Logger = loggerMock.Object}))
            using (var session = (Session)driver.Session())
            {
                session.BeginTransaction(invalidBookmark);
                session.LastBookmark.Should().BeNull(); // ignored
            }
            loggerMock.Verify(x=>x.Info("Failed to recognize bookmark 'invalid bookmark format' and this bookmark is ignored.",
                It.IsAny<object[]>()), Times.Once); // but logged
        }

        [RequireServerVersionGreaterThanOrEqualToFact("3.1.0")]
        public void ShouldThrowForUnreachableBookmark()
        {
            using (var session = (Session)Driver.Session())
            {
                CreateNodeInTx(session);

                // Config the default server bookmark_ready_timeout to be something smaller than 30s to speed up this test
                var exception = Record.Exception(() => session.BeginTransaction(session.LastBookmark + "0"));
                exception.Should().BeOfType<TransientException>();
                exception.Message.Should().Contain("Database not up to the requested version:");
            }
        }


        [RequireServerVersionGreaterThanOrEqualToFact("3.1.0")]
        public void ShouldWaitOnBookmark()
        {
            using (var session = Driver.Session())
            {
                // get a bookmark
                session.LastBookmark.Should().BeNull();
                CreateNodeInTx(session);

                session.LastBookmark.Should().NotBeNull();
                session.LastBookmark.Should().StartWith(BookmarkHeader);
                var lastBookmarkNum = BookmarkNum(session.LastBookmark);

                var queue = new ConcurrentQueue<long>();
                // start a thread to create lastBookmark + 1 tx
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(100);
                    using (var anotherSession = Driver.Session())
                    {
                        CreateNodeInTx(anotherSession);
                        queue.Enqueue(BookmarkNum(anotherSession.LastBookmark));
                    }
                });

                // wait for lastBookmark + 1 and create lastBookmark + 2
                var waitForBookmark = $"{BookmarkHeader}{lastBookmarkNum + 1}";
                CreateNodeInTx(session, waitForBookmark);

                queue.Enqueue(BookmarkNum(session.LastBookmark));

                queue.Count.Should().Be(2);

                queue.TryDequeue(out var value).Should().BeTrue();
                value.Should().BeGreaterThan(lastBookmarkNum);
                
                queue.TryDequeue(out var nextValue).Should().BeTrue();
                nextValue.Should().BeGreaterThan(value);
            }
        }

        private const string BookmarkHeader = "neo4j:bookmark:v1:tx";

        private long BookmarkNum(string bookmark)
        {
            return Convert.ToInt64(bookmark.Substring(BookmarkHeader.Length));
        }

        private void CreateNodeInTx(ISession session, string bookmark = null)
        {
            using (var tx = ((Session)session).BeginTransaction(bookmark))
            {
                tx.Run($"CREATE (a:{Label})");
                tx.Success();
            }
        }
    }
}
