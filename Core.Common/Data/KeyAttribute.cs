using System;

namespace Core.Common.Data {
    [AttributeUsage(AttributeTargets.Property)]
    public class KeyAttribute : Attribute {
        public bool AutoNumber { get; set; }
    }
}
