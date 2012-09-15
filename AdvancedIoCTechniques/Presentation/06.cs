using System.Reflection;
using Autofac;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Ninject;
using Ninject.Extensions.Conventions;
using StructureMap;

namespace AdvancedIoCTechniques
{
    public class ConventionOverConfiguration
    {
        [Test]
        public void So_How_Do_We_Stop_Messing_Around_With_This()
        {
            var c = new UnityContainer();
            c.RegisterType<IFirst0, First0>();
            c.RegisterType<IFirst1, First1>();
            c.RegisterType<ISecond0, Second0>();
            c.RegisterType<ISecond1, Second1>();
            c.RegisterType<ISecond2, Second2>();
            c.RegisterType<IThird0, Third0>();
            c.RegisterType<IThird1, Third1>();

            // too much explicit configuration can be annoying and tedious
            // wouldn't it be better if there was some way to do that automatically?
        }

        [Test]
        public void Scanning_Assemblies_For_Components()
        {
            // most alternative containers to Unity support this out of the box
            // you can get 3rd party libraries to extend Unity (or DIY)

            var structureMap = new Container();
            structureMap.Configure(_ => _.Scan(a =>
            {
                a.TheCallingAssembly();
                a.WithDefaultConventions();
            }));
            Assert.IsNotNull(structureMap.GetInstance<IFirst1>());

            var windsor = new WindsorContainer();
            windsor.Register(Classes.FromThisAssembly()
                .Pick()
                //.Where(t=>t)
                .WithServiceDefaultInterfaces());
            Assert.IsNotNull(windsor.Resolve<IFirst1>());

            var ninject = new StandardKernel();
            ninject.Bind(x => x.FromThisAssembly()
                .SelectAllClasses()
                .BindDefaultInterfaces());
            Assert.IsNotNull(ninject.Get<IFirst1>());

            var autofacBuilder = new ContainerBuilder();
            autofacBuilder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .AsImplementedInterfaces();
            var autofac = autofacBuilder.Build();
            Assert.IsNotNull(autofac.Resolve<IFirst1>());

            // "scan everything" isn't exactly a convention, so let's take a look at some examples
        }

        [Test]
        public void Registering_All_Repositories_As_Singletons()
        {
            var ninject = new StandardKernel();
            ninject.Bind(_ => _.FromThisAssembly()
                                  .SelectAllClasses()
                                  .InheritedFrom(typeof(IRepository<>))
                                  .BindToSelf()
                                  .Configure(x => x.InSingletonScope()));
            Assert.AreSame(
                ninject.Get<InMemoryRepository<Post>>(),
                ninject.Get<InMemoryRepository<Post>>());
        }

        [Test]
        public void Tracing_Activation_Whenever_A_Certain_Set_Of_Components_Are_Resolved()
        {
            bool activated = false;

            var autofacBuilder = new ContainerBuilder();
            autofacBuilder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .AsSelf()
                .AsImplementedInterfaces()
                //.InNamespace("Namespace.With.New.Feature")
                .OnActivating(x => activated = true);

            var c = autofacBuilder.Build();
            c.Resolve<First0>();

            Assert.IsTrue(activated);
        }

        [Test]
        public void Implementing_New_Feature_Set_Using_New_Implementation_Of_Repository()
        {
            var c = new WindsorContainer();
            c.Register(
                Component.For<IRepository<Post>>().ImplementedBy<SqlRepository<Post>>(),
                Component.For<IRepository<Post>>().ImplementedBy<MongoRepository<Post>>().Named("mongo"),
                Classes.FromThisAssembly()
                .Where(t=>t.Name.EndsWith("Controller"))
                    //.InNamespace("Some.Bleeding.Edge.Feature")
                    .Configure(x => x.DependsOn(
                        Dependency.OnComponent(
                            typeof(IRepository<Post>), 
                            "mongo"))));
        }

        [Test]
        public void Most_Useful_For_Some_Sort_Of_Request_Handling()
        {
            // all of this can save for a lot of typing, but more importantly,
            // it makes it easier for things to automagically be configured the same way
            // as other types similar to itself.  common examples:
            //   ASP.NET MVC controllers
            //   "Request" handlers, like command handlers, event handlers, etc.
            //   WCF services
        }
    }
}