// Copyright (c) 2002-2019 "Neo4j,"
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

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Neo4j.Driver
{
    /// <summary>
    ///  Common interface for components that can execute Neo4j statements.
    /// </summary>
    /// <remarks>
    /// <see cref="IAsyncSession"/> and <see cref="IAsyncTransaction"/>
    /// </remarks>
    public interface IAsyncStatementRunner
    {
        /// <summary>
        /// 
        /// Asynchronously run a statement and return a task of result stream.
        ///
        /// This method accepts a String representing a Cypher statement which will be 
        /// compiled into a query object that can be used to efficiently execute this
        /// statement multiple times. This method optionally accepts a set of parameters
        /// which will be injected into the query object statement by Neo4j. 
        ///
        /// </summary>
        /// <param name="statement">A Cypher statement.</param>
        /// <returns>A task of a stream of result values and associated metadata.</returns>
        Task<IStatementResultCursor> RunAsync(string statement);

        /// <summary>
        /// Asynchronously execute a statement and return a task of result stream.
        /// </summary>
        /// <param name="statement">A Cypher statement.</param>
        /// <param name="parameters">A parameter dictionary which is made of prop.Name=prop.Value pairs would be created.</param>
        /// <returns>A task of a stream of result values and associated metadata.</returns>
        Task<IStatementResultCursor> RunAsync(string statement, object parameters);

        /// <summary>
        /// 
        /// Asynchronously run a statement and return a task of result stream.
        ///
        /// This method accepts a String representing a Cypher statement which will be 
        /// compiled into a query object that can be used to efficiently execute this
        /// statement multiple times. This method optionally accepts a set of parameters
        /// which will be injected into the query object statement by Neo4j. 
        ///
        /// </summary>
        /// <param name="statement">A Cypher statement.</param>
        /// <param name="parameters">Input parameters for the statement.</param>
        /// <returns>A task of a stream of result values and associated metadata.</returns>
        Task<IStatementResultCursor> RunAsync(string statement, IDictionary<string, object> parameters);

        /// <summary>
        ///
        /// Asynchronously execute a statement and return a task of result stream.
        ///
        /// </summary>
        /// <param name="statement">A Cypher statement, <see cref="Statement"/>.</param>
        /// <returns>A task of a stream of result values and associated metadata.</returns>
        Task<IStatementResultCursor> RunAsync(Statement statement);
    }
}