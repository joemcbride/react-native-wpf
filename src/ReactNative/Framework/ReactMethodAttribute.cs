using System;

namespace ReactNative.Framework
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ReactMethodAttribute : Attribute
    {
        public string Name { get; set; }

        public ReactMethodAttribute()
        {
        }

        public ReactMethodAttribute(string name)
        {
            Name = name;
        }
    }
}
