using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace p2p.Utils
{
    class Utils
    {
    }

    public static class ObjectMerger
    {
        public static void Merge<T>(T target, T source)
        {
            if (target == null || source == null)
                throw new ArgumentNullException("Neither target nor source can be null");

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                if (!prop.CanRead || !prop.CanWrite)
                    continue; // Saltar propiedades de solo lectura o solo escritura

                var sourceValue = prop.GetValue(source);
                var targetValue = prop.GetValue(target);

                if (sourceValue != null && !IsDefaultValue(sourceValue))
                {
                    prop.SetValue(target, sourceValue);
                }
            }
        }

        private static bool IsDefaultValue(object value)
        {
            var type = value.GetType();
            return type.IsValueType && Activator.CreateInstance(type).Equals(value);
        }
    }

}
