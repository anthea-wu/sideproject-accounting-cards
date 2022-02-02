using System.Configuration;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.WebApi;

namespace accounting_cards
{
    public class AutofacConfig
    {
        /// <summary>
        /// 註冊DI注入物件資料
        /// </summary>
        public static void Register()
        {
            // 容器建立者
            var builder = new ContainerBuilder();

            // 註冊Controllers
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // 註冊DbContextFactory
            // string connectionString =
            //     ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            // builder.RegisterType<DbContextFactory>()
            //     .WithParameter("connectionString", connectionString)
            //     .As<IDbContextFactory>()
            //     .InstancePerHttpRequest();

            // 註冊 Repository UnitOfWork
            builder.Register(x => new DetailRepository()).As<IDetailRepository>().InstancePerRequest();
            builder.Register(x => new CardRepository()).As<ICardRepository>().InstancePerRequest();
            builder.Register(x => new DataService()).As<IDataService>().InstancePerRequest();

            // 註冊Services
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.Name.EndsWith("Services"))
                .AsImplementedInterfaces();

            // 建立容器
            var container = builder.Build();

            // 解析容器內的型別
            var resolverApi = new AutofacWebApiDependencyResolver(container);

            // 建立相依解析器
            GlobalConfiguration.Configuration.DependencyResolver = resolverApi;
        }
    }
}