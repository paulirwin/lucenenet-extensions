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
using Lucene.Net.Index;
using Lucene.Net.Search;
using Microsoft.Extensions.DependencyInjection;

namespace Lucene.Net.Extensions.DependencyInjection.Registrations
{
    /// <summary>
    /// Registers and provides <see cref="IndexSearcher"/> instances with optional singleton caching.
    /// Supports different service lifetimes (Singleton, Scoped, Transient) for dependency injection.
    /// </summary>
    public class IndexSearcherRegistration : IDisposable
    {
        private readonly string _name;
        private readonly ServiceLifetime _lifetime;
        private IndexSearcher? _cachedSearcher;
        private readonly object _lock = new();
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexSearcherRegistration"/> class.
        /// </summary>
        /// <param name="name">The unique name of the index searcher registration.</param>
        /// <param name="config">Index configuration options.</param>
        public IndexSearcherRegistration(string name, LuceneIndexOptions config)
        {
            _name = name;
            _lifetime = config.SearcherLifetime;
        }

        /// <summary>
        /// Gets an <see cref="IndexSearcher"/> instance according to the configured lifetime.
        /// </summary>
        /// <param name="sp">The service provider to resolve dependencies.</param>
        /// <returns>An <see cref="IndexSearcher"/> instance.</returns>
        /// <exception cref="NotSupportedException">Thrown if the configured service lifetime is unsupported.</exception>
        public IndexSearcher GetSearcher(IServiceProvider sp)
        {
            return _lifetime switch
            {
                ServiceLifetime.Singleton => GetSingletonSearcher(sp),
                ServiceLifetime.Scoped or ServiceLifetime.Transient => sp.GetRequiredKeyedService<IndexSearcher>(_name),
                _ => throw new NotSupportedException($"Unsupported lifetime: {_lifetime}")
            };
        }

        // Private helper that returns the cached singleton instance and refreshes it if needed.
        private IndexSearcher GetSingletonSearcher(IServiceProvider sp)
        {
            lock (_lock)
            {
                var readerReg = sp.GetRequiredKeyedService<IndexReaderRegistration>(_name);
                var currentReader = readerReg.GetReader(sp);

                if (_cachedSearcher?.IndexReader != currentReader)
                {
                    _cachedSearcher = new IndexSearcher(currentReader);
                }

                return _cachedSearcher;
            }
        }

        /// <summary>
        /// Releases all resources used by the <see cref="IndexSearcherRegistration"/>.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            lock (_lock)
            {
                _cachedSearcher = null;
            }

            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
