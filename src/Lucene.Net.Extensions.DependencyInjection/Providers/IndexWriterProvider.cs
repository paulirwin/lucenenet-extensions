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

using Lucene.Net.Index;
using Lucene.Net.Extensions.DependencyInjection.Registrations;
using Microsoft.Extensions.DependencyInjection;

namespace Lucene.Net.Extensions.DependencyInjection.Providers
{
    /// <summary>
    /// Provides <see cref="IndexWriter"/> instances by name using dependency injection.
    /// </summary>
    public class IndexWriterProvider : IIndexWriterProvider
    {
        private readonly IServiceProvider _sp;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexWriterProvider"/> class.
        /// </summary>
        /// <param name="sp">The service provider used to resolve index writers.</param>
        public IndexWriterProvider(IServiceProvider sp)
        {
            _sp = sp;
        }

        /// <summary>
        /// Gets an <see cref="IndexWriter"/> instance by its registered name.
        /// </summary>
        /// <param name="name">The name of the index writer.</param>
        /// <returns>An <see cref="IndexWriter"/> instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown if no writer is registered for the specified name.</exception>
        public IndexWriter Get(string name)
        {
            var registration = _sp.GetKeyedService<IndexWriterRegistration>(name);
            // Explicit null-check to give a personal clearer error about Writer instead of DI resolution failure.
            if (registration == null)
                throw new InvalidOperationException(
            $"No writer is registered for index '{name}'. " +
            $"Did you forget to configure WriterLifetime?");

            return registration.GetWriter(_sp);
        }
    }
}
