using System;

namespace ReactNative.Framework
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ReactModuleAttribute : Attribute
    {
        public string Name { get; set; }

        public Type Type { get; set; }

        public ReactModuleAttribute()
        {
        }

        public ReactModuleAttribute(string name)
        {
            Name = name;
        }
    }
}
