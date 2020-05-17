using System;
using System.Collections.Generic;

namespace BooksBase.Shared
{
    public interface IPermissionService
    {
        void AddPermissions(Type type);
        List<Permission> GetAllPermissions();
        List<Permission> GetUserPermissions();
    }
}
