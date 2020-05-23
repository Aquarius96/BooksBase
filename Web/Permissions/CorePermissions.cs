using System.ComponentModel.DataAnnotations;
using Web.Resources;

namespace Web.Permissions
{
    public static class CorePermissions
    {
        private const string Prefix = "Permission.";

        [Display(Name = "DisplayUsers", ResourceType = typeof(Resource))]
        public const string DisplayUsers = Prefix + "DisplayUsers";
        [Display(Name = "ManageUsers", ResourceType = typeof(Resource))]
        public const string ManageUsers = Prefix + "ManageUsers";

        [Display(Name = "DisplayRoles", ResourceType = typeof(Resource))]
        public const string DisplayRoles = Prefix + "DisplayRoles";
        [Display(Name = "ManageRoles", ResourceType = typeof(Resource))]
        public const string ManageRoles = Prefix + "ManageRoles";

        [Display(Name = "DisplayAuthors", ResourceType = typeof(Resource))]
        public const string DisplayAuthors = Prefix + "DisplayAuthors";
        [Display(Name = "ManageAuthors", ResourceType = typeof(Resource))]
        public const string ManageAuthors = Prefix + "ManageAuthors";

        [Display(Name = "DisplayBooks", ResourceType = typeof(Resource))]
        public const string DisplayBooks = Prefix + "DisplayBooks";
        [Display(Name = "ManageBooks", ResourceType = typeof(Resource))]
        public const string ManageBooks = Prefix + "ManageBooks";
    }
}
