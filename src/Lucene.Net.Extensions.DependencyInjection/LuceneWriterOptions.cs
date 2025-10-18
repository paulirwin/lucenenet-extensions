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
using Microsoft.Extensions.DependencyInjection;

namespace Lucene.Net.Extensions.DependencyInjection
{
    /// <summary>
    /// Configuration options for a Lucene <see cref="IndexWriter"/>.
    /// </summary>
    public class LuceneWriterOptions
    {
        public IndexDeletionPolicy? DeletionPolicy { get; set; }
        public ServiceLifetime WriterLifetime { get; set; } = ServiceLifetime.Singleton;

        public Action<IServiceProvider, IndexWriterConfig>? ConfigureIndexWriterConfig { get; set; }

        // Effective fallback
        public IndexDeletionPolicy EffectiveDeletionPolicy =>
            DeletionPolicy ?? new SnapshotDeletionPolicy(new KeepOnlyLastCommitDeletionPolicy());

        /// <summary>
        /// Applies the configured writer settings to the given <see cref="IndexWriterConfig"/>.
        /// </summary>
        /// <param name="sp">The DI service provider.</param>
        /// <param name="config">The <see cref="IndexWriterConfig"/> to apply settings to.</param>
        public void ApplyWriterSettings(IServiceProvider sp, IndexWriterConfig config)
        {
            ConfigureIndexWriterConfig?.Invoke(sp, config);

            // Apply default
            if (config.MaxBufferedDocs <= 0)
                config.MaxBufferedDocs = 1000;
        }
    }
}
