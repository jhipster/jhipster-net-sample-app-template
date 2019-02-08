using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace JHipsterNetSampleApplication.Data.Extensions {
    public static class DbSetExtensions {
        public static EntityEntry<TEntity> RemoveById<TEntity>(this DbSet<TEntity> receiver, object id)
            where TEntity : class
        {
            var container = Activator.CreateInstance<TEntity>();
            var idProperty = GetKeyProperty(container.GetType());
            idProperty.SetValue(container, id, null);
            receiver.Attach(container);
            return receiver.Remove(container);
        }

        private static PropertyInfo GetKeyProperty(Type type)
        {
            var key = type.GetProperties().FirstOrDefault(p =>
                p.Name.Equals("ID", StringComparison.OrdinalIgnoreCase)
                || p.Name.Equals(type.Name + "ID", StringComparison.OrdinalIgnoreCase));

            if (key != null) return key;
            key = type.GetProperties().FirstOrDefault(p =>
                p.CustomAttributes.Any(attr => attr.AttributeType == typeof(KeyAttribute)));

            if (key != null) return key;

            //https://stackoverflow.com/questions/25141955/entityframework-6-how-to-get-identity-field-with-reflection
            //TODO complete with FluentAPi
            return null;
        }
    }
}
