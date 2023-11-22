using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DayDoc.Components.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter)]
    public class BindExcludeIdAttribute : Attribute, IPropertyFilterProvider
    {
        public Func<ModelMetadata, Boolean> PropertyFilter { get; }

        public BindExcludeIdAttribute()
        {
            PropertyFilter = (metadata) => metadata.PropertyName != "Id";
        }
    }
}