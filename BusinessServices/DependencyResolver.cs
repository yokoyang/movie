using System.ComponentModel.Composition;
using DataModel;
using DataModel.UnitOfWork;
using Resolver;

namespace BusinessServices
{
    [Export(typeof(IComponent))]
    public class DependencyResolver : IComponent
    {
        public void SetUp(IRegisterComponent registerComponent)
        {
            registerComponent.RegisterType<IMovieService, MovieService>();
            registerComponent.RegisterType<IUserService, UserService>();
            registerComponent.RegisterType<IShopCartService, ShopCartService>();
            registerComponent.RegisterType<IAdminService, AdminService>();
            registerComponent.RegisterType<IConcessionService, ConcessionService>();

        }
    }
}