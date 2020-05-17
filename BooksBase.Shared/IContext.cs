namespace BooksBase.Shared
{
    public interface IContext
    {
        void SeedDatabase(IPermissionService permissionService);
    }
}
