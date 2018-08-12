using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace LD42.MiningLogisticsGame.Engine
{
    public class GameContent
    {
        private static Dictionary<string, object> _content = new Dictionary<string, object>();

        public static void Put(string key, object value)
        {
            _content[key] = value;
        }

        public static T Get<T>(string key) where T : class
        {
            return _content[key] as T;
        }

        public static Lazy<T> LazyGet<T>(string key) where T : class => new Lazy<T>(() => Get<T>(key));
    }
}
