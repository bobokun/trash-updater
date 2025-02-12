﻿using JetBrains.Annotations;
using YamlDotNet.Serialization;

namespace Trash.Extensions
{
    public static class YamlDotNetExtensions
    {
        public static T? DeserializeType<T>(this IDeserializer deserializer, string data)
            where T : class
        {
            var extractor = deserializer.Deserialize<RootExtractor<T>>(data);
            return extractor.RootObject;
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private class RootExtractor<T>
            where T : class
        {
            public T? RootObject { get; }
        }
    }
}
