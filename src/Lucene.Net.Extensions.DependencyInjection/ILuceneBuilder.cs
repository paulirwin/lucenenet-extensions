/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

using System;

namespace Lucene.Net.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides methods to register Lucene indexes and related services (readers, searchers, writers, analyzers).
    /// </summary>
    public interface ILuceneBuilder
    {
        /// <summary>
        /// Adds a new Lucene index with the specified name and configuration options.
        /// </summary>
        /// <param name="name">The unique name of the index.</param>
        /// <param name="configure">A delegate to configure <see cref="LuceneIndexOptions"/>.</param>
        /// <returns>An <see cref="IIndexBuilder"/> for chaining index-specific registrations.</returns>
        IIndexBuilder AddIndex(string name, Action<LuceneIndexOptions> configure);
    }
}
