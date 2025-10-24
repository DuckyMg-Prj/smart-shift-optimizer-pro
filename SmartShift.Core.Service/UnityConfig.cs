using SmartShift.Core.Data.Context;
using SmartShift.Core.Service.Abstraction;
using SmartShift.Core.Service.Company;
using SmartShift.Core.Service.Login;
using SmartShift.Core.Service.shift;
using System.ComponentModel;
using System.Xml.Linq;
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
            _container.RegisterType<ICompanyService, CompanyService>(new HierarchicalLifetimeManager());
            _container.RegisterType<IShiftService, ShiftService>(new HierarchicalLifetimeManager());
            _container.RegisterType<IShiftReportService, ShiftReportService>(new HierarchicalLifetimeManager());
            _container.RegisterType<IShiftAssignmentService, ShiftAssignmentService>(new HierarchicalLifetimeManager());
            _container.RegisterType<IShiftRequestService, ShiftRequestService>(new HierarchicalLifetimeManager());

        }
    }
}
