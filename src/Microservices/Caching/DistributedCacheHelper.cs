using Core.Caching;
using Microservices.Serialization;
using Microservices.Serialization.Impl;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microservices.Caching
{
    public class DistributedCacheHelper : ICacheHelper
    {
        private readonly IDistributedCache _cache;
        private readonly IJsonSerializer _jsonSerializer;

        public DistributedCacheHelper(IDistributedCache cache)
        {
            _cache = cache;
            _jsonSerializer = NewtonsoftJsonSerializer.New;
        }

        public T GetOrSet<T>(string key, DistributedCacheEntryOptions options, Func<T> valueFactory)
        {
            CacheEntry<T> cacheItem = Get<T>(key);

            if (cacheItem == null || !cacheItem.IsValid)
            {
                T value = valueFactory.Invoke();

                cacheItem = new CacheEntry<T>(key, value);

                if (options == null)
                {
                    options = new DistributedCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromHours(DefaultExpirationInHours));
                }

                Set(key, value, options);
            }

            return cacheItem.Value;
        }

        public T GetOrSet<T>(string key, Func<T> valueFactory)
        {
            return GetOrSet(key, null, valueFactory);
        }

        public TReturn GetOrSet<TReturn>(string key, Func<TReturn> valueFactory, int expirationInHours = 24)
        {
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(expirationInHours));

            return GetOrSet(key, options, valueFactory);
        }

        public object Remove(string key)
        {
            CacheEntry<object> cacheEntry = Get<object>(key);

            CacheKeyManager.Remove(key);

            _cache.Remove(key);

            return cacheEntry;
        }

        public void RemoveAll()
        {
            foreach (string key in CacheKeyManager.GetAllKeys())
            {
                Remove(key);
            }
        }

        public string FormatKey(params object[] args)
        {
            return String.Join(_delimeter, args);
        }

        private void Set<T>(string key, T item, DistributedCacheEntryOptions distributedCacheEntryOptions)
        {
            var cacheItem = new CacheEntry<T>(key, item);

            byte[] encodedCacheItem = ObjectToByteArray(cacheItem);

            _cache.Set(key, encodedCacheItem, distributedCacheEntryOptions);

            CacheKeyManager.Add(key);
        }

        private CacheEntry<T> Get<T>(string key)
        {
            CacheEntry<T> cacheItem = null;

            byte[] encodedCacheItem = _cache.Get(key);

            if (encodedCacheItem != null)
            {
                cacheItem = ByteArrayToObject<CacheEntry<T>>(encodedCacheItem);
            }

            return cacheItem;
        }

        private byte[] ObjectToByteArray<T>(T item)
        {
            string itemAsJson = _jsonSerializer.Serialize(item);

            return Encoding.UTF8.GetBytes(itemAsJson);
        }

        private T ByteArrayToObject<T>(byte[] bytes)
        {
            string itemAsJson = Encoding.UTF8.GetString(bytes);

            return _jsonSerializer.Deserialize<T>(itemAsJson);
        }

        private const int DefaultExpirationInHours = 24;
        private static readonly string _delimeter = "::";
    }

    public static class CacheKeyManager
    {
        private static ConcurrentDictionary<string, string> _cacheKeys = new ConcurrentDictionary<string, string>();

        public static void Add(string key)
        {
            _cacheKeys.TryAdd(key, key);
        }

        public static void Remove(string key)
        {
            _cacheKeys.TryRemove(key, out string s);
        }

        public static List<string> GetAllKeys()
        {
            return _cacheKeys.Keys.ToList();
        }
    }
}
