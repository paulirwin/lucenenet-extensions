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
    /// Provides <see cref="IndexReader"/> instances by name using dependency injection.
    /// </summary>
    public class IndexReaderProvider : IIndexReaderProvider
    {
        private readonly IServiceProvider _sp;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexReaderProvider"/> class.
        /// </summary>
        /// <param name="sp">The service provider used to resolve index readers.</param>
        public IndexReaderProvider(IServiceProvider sp)
        {
            _sp = sp;
        }

        /// <summary>
        /// Gets an <see cref="IndexReader"/> instance by its registered name.
        /// </summary>
        /// <param name="name">The name of the index reader.</param>
        /// <returns>An <see cref="IndexReader"/> instance.</returns>
        public IndexReader Get(string name)
        {
            var registration = _sp.GetRequiredKeyedService<IndexReaderRegistration>(name);
            return registration.GetReader(_sp);
        }
    }
}
