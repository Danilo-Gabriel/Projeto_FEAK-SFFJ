using System.ComponentModel;
using System.Reflection;

namespace Application.services
{
    public static class DisplayNameResolver
    {
        public static string GetDisplayName<T>(string propertyPath)
        {
            // Ex: "Endereco.Rua"
            var parts = propertyPath.Split('.');
            Type type = typeof(T);

            foreach (var part in parts)
            {
                var property = type.GetProperty(part);
                if (property == null)
                    return propertyPath;

                var displayName = property
                    .GetCustomAttribute<DisplayNameAttribute>()?
                    .DisplayName;

                if (!string.IsNullOrWhiteSpace(displayName))
                    return displayName;

                type = property.PropertyType;
            }

            return propertyPath;
        }
    }
}