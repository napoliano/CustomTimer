using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomTimer
{
    public class Singleton<T> where T : class, new()
    {
        private static readonly Lazy<T> _lazy = new Lazy<T>(() => new T());

        public static T Instance => _lazy.Value;

        protected Singleton()
        { }
    }
}
