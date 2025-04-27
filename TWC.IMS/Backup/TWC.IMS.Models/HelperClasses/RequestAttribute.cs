using System;

namespace TWC.IMS.Models.HelperClasses
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class RequestAttribute : Attribute
    {
        public virtual string TableName { get; set; }
        public virtual string StatusColumn { get; set; }
        public virtual string TransactionNoColumn { get; set; }
        public virtual string UniqueKeyColumn { get; set; }
        public virtual string Url { get; set; }
        public virtual string ProponentColumn { get; set; }
    }
}
