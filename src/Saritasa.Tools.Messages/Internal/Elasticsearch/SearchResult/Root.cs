﻿// Copyright (c) 2015-2017, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;

namespace Saritasa.Tools.Messages.Internal.Elasticsearch.SearchResult
{
    internal class Root
    {
        [JsonProperty(PropertyName = "took")]
        public string Took { get; set; }

        [JsonProperty(PropertyName = "timed_out")]
        public string TimedOut { get; set; }

        [JsonProperty(PropertyName = "_shards")]
        public Shards Shards { get; set; }

        [JsonProperty(PropertyName = "hits")]
        public Hits Hits { get; set; }
    }
}
