﻿// Copyright (c) 2002-2019 "Neo4j,"
// Neo4j Sweden AB [http://neo4j.com]
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
using System.Threading.Tasks;
using Neo4j.Driver.Internal.MessageHandling.Metadata;
using Neo4j.Driver.Internal.Result;

namespace Neo4j.Driver.Internal.MessageHandling.V3
{
    internal class PullResponseHandler : MetadataCollectingResponseHandler
    {
        private readonly IResultStreamBuilder _streamBuilder;
        private readonly SummaryBuilder _summaryBuilder;
        private readonly IBookmarkTracker _bookmarkTracker;

        public PullResponseHandler(IResultStreamBuilder streamBuilder, SummaryBuilder summaryBuilder,
            IBookmarkTracker bookmarkTracker)
        {
            _streamBuilder = streamBuilder ?? throw new ArgumentNullException(nameof(streamBuilder));
            _summaryBuilder = summaryBuilder ?? throw new ArgumentNullException(nameof(summaryBuilder));
            _bookmarkTracker = bookmarkTracker;

            AddMetadata<BookmarkCollector, Bookmark>();
            AddMetadata<TimeToLastCollector, long>();
            AddMetadata<TypeCollector, StatementType>();
            AddMetadata<CountersCollector, ICounters>();
            AddMetadata<PlanCollector, IPlan>();
            AddMetadata<ProfiledPlanCollector, IProfiledPlan>();
            AddMetadata<NotificationsCollector, IList<INotification>>();
        }

        public override async Task OnSuccessAsync(IDictionary<string, object> metadata)
        {
            await base.OnSuccessAsync(metadata);

            _bookmarkTracker?.UpdateBookmark(GetMetadata<BookmarkCollector, Bookmark>());

            _summaryBuilder.ResultConsumedAfter = GetMetadata<TimeToLastCollector, long>();
            _summaryBuilder.Counters = GetMetadata<CountersCollector, ICounters>();
            _summaryBuilder.Notifications = GetMetadata<NotificationsCollector, IList<INotification>>();
            _summaryBuilder.Plan = GetMetadata<PlanCollector, IPlan>();
            _summaryBuilder.Profile = GetMetadata<ProfiledPlanCollector, IProfiledPlan>();
            _summaryBuilder.StatementType = GetMetadata<TypeCollector, StatementType>();

            await _streamBuilder.PullCompletedAsync(false, null);
        }

        public override Task OnFailureAsync(IResponsePipelineError error)
        {
            return _streamBuilder.PullCompletedAsync(false, error);
        }

        public override Task OnIgnoredAsync()
        {
            return _streamBuilder.PullCompletedAsync(false, null);
        }

        public override Task OnRecordAsync(object[] fieldValues)
        {
            return _streamBuilder.PushRecordAsync(fieldValues);
        }
    }
}