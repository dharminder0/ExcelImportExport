using System;

namespace Core.Common.Data {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class AliasAttribute : Attribute {
        public string Name { get; set; }
    }
}
