using SmartShift.Core.Data.Context;
using SmartShift.Core.Service.Abstraction;
using Unity;
using Unity.Lifetime;

namespace SmartShift.Core.Service
{
    public static class UnityConfig
    {
        private static IUnityContainer _container;

        public static IUnityContainer GetConfiguredContainer()
        {
            if (_container == null)
                RegisterComponents();

            return _container;
        }

        public static void RegisterComponents()
        {
            _container = new UnityContainer();

            // Register dependencies here
            _container.RegisterType<AppDbContext, AppDbContext>(new HierarchicalLifetimeManager());
            _container.RegisterType<IAuthService, AuthService>(new HierarchicalLifetimeManager());

            // Future services:
            // _container.RegisterType<ICompanyService, CompanyService>();
            // _container.RegisterType<IShiftService, ShiftService>();
        }
    }
}
