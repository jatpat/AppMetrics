﻿// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using App.Metrics.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace App.Metrics.Formatters.Json.Facts.Helpers
{
    public enum HealthStatusSamples
    {
        Valid,
        NullHealthy,
        NullUnhealthy
    }

    public enum MetricDataSamples
    {
        SingleContext
    }

    public enum MetricTypeSamples
    {
        
    }


    public static class TestHelperExtensions
    {
        private static readonly Dictionary<HealthStatusSamples, string> HealthStatusFileSampleMapping = new Dictionary<HealthStatusSamples, string>
                                                                                                        {
                                                                                                            {
                                                                                                                HealthStatusSamples.Valid,
                                                                                                                "healthstatus"
                                                                                                            },
                                                                                                            {
                                                                                                                HealthStatusSamples.NullHealthy,
                                                                                                                "healthstatus_null_healthy"
                                                                                                            },
                                                                                                            {
                                                                                                                HealthStatusSamples.NullUnhealthy,
                                                                                                                "healthstatus_null_unhealthy"
                                                                                                            }
                                                                                                        };

        private static readonly Dictionary<MetricDataSamples, string> MetricDataFileSampleMapping = new Dictionary<MetricDataSamples, string>
                                                                                                    {
                                                                                                        {
                                                                                                            MetricDataSamples.SingleContext,
                                                                                                            "metricdata_single_context"
                                                                                                        }
                                                                                                    };

        // ReSharper disable CollectionNeverUpdated.Local no metric type sample files yet
        private static readonly Dictionary<MetricTypeSamples, string> MetricTypeFileSampleMapping = new Dictionary<MetricTypeSamples, string>();
        // ReSharper restore CollectionNeverUpdated.Local

        private static JToken ExtractHealthStatusSampleFromResourceFile(this HealthStatusSamples sample)
        {
            return ExtractJsonFromEmbeddedResource(HealthStatusFileSampleMapping[sample]);
        }

        private static JToken ExtractMetricDataSampleFromResourceFile(this MetricDataSamples sample)
        {
            return ExtractJsonFromEmbeddedResource(MetricDataFileSampleMapping[sample]);
        }

        private static JToken ExtractMetricSampleFromResourceFile(this MetricType metricType)
        {
            return ExtractJsonFromEmbeddedResource(metricType.ToString().ToLower());
        }

        private static JToken ExtractMetricTypeSampleFromResourceFile(this MetricTypeSamples sample)
        {
            return ExtractJsonFromEmbeddedResource(MetricTypeFileSampleMapping[sample]);
        }

        public static JToken ParseAsJson(this string json)
        {
            return JToken.Parse(json, new JsonLoadSettings { LineInfoHandling = LineInfoHandling.Ignore, CommentHandling = CommentHandling.Ignore });
        }

        public static JToken SampleJson(this MetricType metric) { return metric.ExtractMetricSampleFromResourceFile(); }

        public static JToken SampleJson(this EnvironmentInfo env) { return ExtractJsonFromEmbeddedResource("env"); }

        public static JToken SampleJson(this HealthStatusSamples sample) { return sample.ExtractHealthStatusSampleFromResourceFile(); }

        public static JToken SampleJson(this MetricDataSamples sample) { return sample.ExtractMetricDataSampleFromResourceFile(); }

        public static JToken SampleJson(this MetricTypeSamples sample) { return sample.ExtractMetricTypeSampleFromResourceFile(); }

        private static JToken ExtractJsonFromEmbeddedResource(string key)
        {
            var assemblyName = new AssemblyName("App.Metrics.Formatters.Json.Facts");
            using (
                var fileStream =
                    Assembly.Load(assemblyName).GetManifestResourceStream($"App.Metrics.Formatters.Json.Facts.JsonFiles.{key}.json"))
            {
                if (fileStream == null)
                {
                    return null;
                }

                using (var textReader = new StreamReader(fileStream))
                {
                    using (var jsonReader = new JsonTextReader(textReader))
                    {
                        return JToken.ReadFrom(jsonReader);
                    }
                }
            }
        }
    }
}