using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CoreDocker.Api.Components.Projects;
using CoreDocker.Api.Components.Users;
using CoreDocker.Api.GraphQl;
using CoreDocker.Api.GraphQl.DynamicQuery;
using CoreDocker.Api.Security;
using CoreDocker.Core.Components.Users;
using CoreDocker.Core.Startup;
using CoreDocker.Core.Vendor;
using CoreDocker.Dal.InMemoryCollections;
using CoreDocker.Dal.Models.Projects;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Dal.Persistance;
using CoreDocker.Utilities.Helpers;
using GraphQL;
using GraphQL.Authorization;
using GraphQL.Http;
using GraphQL.Types;
using GraphQL.Validation;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CoreDocker.Api.AppStartup
{
    public class IocApi : IocCoreBase
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static bool _isInitialized;
        private static readonly object _locker = new object();
        private static IocApi _instance;
        private static IServiceCollection _services;

        public IocApi()
        {
            var builder = new ContainerBuilder();
            SetupCore(builder);
            SetupCommonControllers(builder);
            SetupGraphQl(builder);

            SetupTools(builder);
            builder.Populate(_services);
            Container = builder.Build();
        }

        private static void SetupGraphQl(ContainerBuilder builder)
        {
            builder.RegisterType<DocumentExecuter>().As<IDocumentExecuter>().SingleInstance();
            builder.RegisterType<DocumentWriter>().As<IDocumentWriter>().SingleInstance();

            builder.RegisterType<OriginalDateGraphType>().SingleInstance();

            //validation
            //services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.RegisterType<RequiresAuthValidationRule>().As<IValidationRule>();
//            builder.RegisterType<AuthorizationValidationRule>().As<IValidationRule>();
            builder.RegisterType<AuthorizationEvaluator>().As<IAuthorizationEvaluator>().SingleInstance();

            builder.Register(s =>
            {
                var authSettings = new AuthorizationSettings();

                authSettings.AddPolicy("AdminPolicy", _ => _.RequireClaim("role", "Admin"));

                return authSettings;
            });

            builder.RegisterType<DefaultQuery>().SingleInstance();
            builder.RegisterType<DefaultMutation>().SingleInstance();
            builder.RegisterType<QueryResultSpecification>().SingleInstance();
            builder.RegisterType<DefaultSchema>().As<ISchema>().SingleInstance();

            /*user*/
            builder.RegisterType<UserSpecification>();
            builder.RegisterType<UsersSpecification>();
            builder.RegisterType<UserCreateUpdateSpecification>();
            builder.RegisterType<UsersMutationSpecification>();
            builder.RegisterType<RoleSpecification>();

            /*project*/
            builder.RegisterType<ProjectSpecification>();
            builder.RegisterType<OpenIdSettings>();
            builder.RegisterType<ProjectsSpecification>();
            builder.RegisterType<ProjectCreateUpdateSpecification>();
            builder.RegisterType<ProjectsMutationSpecification>();
            

         

            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
        }

        public static void Populate(IServiceCollection services)
        {
            if (_isInitialized) throw new Exception("Need to call Populate before first instance call.");
            _services = services;
        }

        #region Overrides of IocCoreBase

        protected override IGeneralUnitOfWorkFactory GetInstanceOfIGeneralUnitOfWorkFactory(IComponentContext arg)
        {
            var instanceOfIGeneralUnitOfWorkFactory = new InMemoryGeneralUnitOfWorkFactory();
            var generalUnitOfWork = instanceOfIGeneralUnitOfWorkFactory.GetConnection();
            var users = generalUnitOfWork.Users;
            var admin = new User() { Name = "Admin user", Email = "admin@admin.com", HashedPassword = PasswordHash.CreateHash("admin!") };
            admin.Roles.Add(RoleManager.Admin.Name);
            users.Add(admin).Wait();

            var guest = new User() { Name = "Guest", Email = "guest@guest.com", HashedPassword = PasswordHash.CreateHash("guest!") };
            guest.Roles.Add(RoleManager.Guest.Name);
            users.Add(guest).Wait();

            Enumerable.Range(1, 10)
                .Select(x => new Project() {Name = $"Project {x}.0"})
                .ForEach(x => generalUnitOfWork.Projects.Add(x).Wait());
            
            return instanceOfIGeneralUnitOfWorkFactory;
        }

        #endregion

        #region Private Methods

        private void SetupCommonControllers(ContainerBuilder builder)
        {
            builder.RegisterType<UserCommonController>();
            builder.RegisterType<ProjectCommonController>();

        }

        private void SetupTools(ContainerBuilder builder)
        {
            builder.RegisterType<UserCommonController>();
        }

        #endregion

        #region Instance

        public static IocApi Instance
        {
            get
            {
                if (_isInitialized) return _instance;
                lock (_locker)
                {
                    if (!_isInitialized)
                    {
                        _instance = new IocApi();
                        _isInitialized = true;
                    }
                }
                return _instance;
            }
        }

        public IContainer Container { get; }


        public T Resolve<T>()
        {
            return Container.Resolve<T>();
        }

        #endregion
    }
}
