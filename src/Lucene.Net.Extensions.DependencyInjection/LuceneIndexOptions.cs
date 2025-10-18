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
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Util;
using Microsoft.Extensions.DependencyInjection;
using LuceneDirectory = Lucene.Net.Store.Directory;

namespace Lucene.Net.Extensions.DependencyInjection
{
    /// <summary>
    /// Configuration options for a Lucene index, including directory, analyzer, and lifetimes for readers and searchers.
    /// </summary>
    public class LuceneIndexOptions
    {
        public string? IndexPath { get; set; }
        public Func<IServiceProvider, LuceneDirectory>? DirectoryFactory { get; set; }

        public Analyzer? Analyzer { get; set; }
        public LuceneVersion LuceneVersion { get; set; } = LuceneVersion.LUCENE_48;

        public bool EnableRefreshing { get; set; } = false;

        public ServiceLifetime ReaderLifetime { get; set; } = ServiceLifetime.Singleton;
        public ServiceLifetime SearcherLifetime { get; set; } = ServiceLifetime.Singleton;

        // Effective fallbacks
        public Analyzer EffectiveAnalyzer => Analyzer ?? new StandardAnalyzer(LuceneVersion);
    }
}
