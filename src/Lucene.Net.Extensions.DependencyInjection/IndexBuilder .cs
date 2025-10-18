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

using Microsoft.Extensions.DependencyInjection;

namespace Lucene.Net.Extensions.DependencyInjection
{
    /// <summary>
    /// Implements <see cref="IIndexBuilder"/> to allow configuring writers for a specific index.
    /// Delegates back to <see cref="LuceneBuilder"/> for adding more indexes.
    /// </summary>
    internal class IndexBuilder : IIndexBuilder
    {
        /// <summary>
        /// Initializes a new instance of <see cref="IndexBuilder"/>.
        /// </summary>
        /// <param name="luceneBuilder">The main <see cref="LuceneBuilder"/> instance.</param>
        /// <param name="indexName">The name of the index being configured.</param>
        private readonly LuceneBuilder _luceneBuilder;
        private readonly string _indexName;

        public IndexBuilder(LuceneBuilder luceneBuilder, string indexName)
        {
            _luceneBuilder = luceneBuilder;
            _indexName = indexName;
        }

        public IIndexBuilder AddIndexWriter(Action<LuceneWriterOptions> configure)
        {
            _luceneBuilder.AddIndexWriter(_indexName, configure);
            return this; // keep chaining on the same index
        }

        // delegate back to LuceneBuilder for adding more indexes
        public IIndexBuilder AddIndex(string name, Action<LuceneIndexOptions> configure)
        {
            return _luceneBuilder.AddIndex(name, configure);
        }
    }
}
