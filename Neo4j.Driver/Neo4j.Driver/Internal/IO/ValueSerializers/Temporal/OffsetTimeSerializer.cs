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

namespace Neo4j.Driver.Internal.IO.ValueSerializers.Temporal
{
    internal class OffsetTimeSerializer : IPackStreamSerializer
    {
        public const byte StructType = (byte) 'T';
        public const int StructSize = 2;

        public IEnumerable<byte> ReadableStructs => new[] {StructType};

        public IEnumerable<Type> WritableTypes => new[] {typeof(OffsetTime)};

        public object Deserialize(IPackStreamReader reader, byte signature, long size)
        {
            PackStream.EnsureStructSize("Time", StructSize, size);

            var nanosOfDay = reader.ReadLong();
            var offsetSeconds = reader.ReadInteger();

            return new OffsetTime(TemporalHelpers.NanoOfDayToTime(nanosOfDay), offsetSeconds);
        }

        public void Serialize(IPackStreamWriter writer, object value)
        {
            var time = value.CastOrThrow<OffsetTime>();

            writer.WriteStructHeader(StructSize, StructType);
            writer.Write(time.ToNanoOfDay());
            writer.Write(time.OffsetSeconds);
        }
    }
}