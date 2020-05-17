using BooksBase.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BooksBase.Infrastructure
{
    public class PermissionService : IPermissionService
    {
        private readonly List<Permission> _permissions = new List<Permission>();

        public void AddPermissions(Type type)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach(var field in fields)
            {
                var value = field.GetValue(null) as string;
                var display = value as string;

                DisplayAttribute[] attributes = (DisplayAttribute[])field.GetCustomAttributes(typeof(DisplayAttribute), false);

                if (attributes != null && attributes.Length > 0)
                {
                    Type resourceType = attributes[0].ResourceType;

                    if (resourceType != null)
                    {
                        var property = resourceType.GetProperty(attributes[0].Name);
                        if (property != null)
                        {
                            var propertyValue = property.GetValue(null);

                            if (propertyValue != null)
                            {
                                display = propertyValue.ToString();
                            }
                        }
                    }
                }

                AddPermission(value, display);
            }
        }

        public List<Permission> GetAllPermissions()
        {
            return _permissions.ToList();
        }

        public List<Permission> GetUserPermissions()
        {
            return _permissions.Where(permission => permission.Claim.Split(".").Last().StartsWith("Display"))
                .ToList();
        }

        private void AddPermission(string claim, string display)
        {
            _permissions.Add(new Permission
            {
                Claim = claim,
                Display = display
            });
        }
    }
}
