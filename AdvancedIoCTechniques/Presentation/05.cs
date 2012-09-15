using System.Reflection;
using Autofac;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using NUnit.Framework;
using StructureMap;
using StructureMap.Configuration.DSL;
using Module = Autofac.Module;

namespace AdvancedIoCTechniques
{
    public class Organization
    {
        [Test]
        public void Keep_Things_Simple_Stupid()
        {
            // all containers will have some sort of module or installer system which lets
            // you do your registrations in a controlled fashion
        }

        public class Single_Reponsibility_Principle
        {
            // since this implements IWindsorInstaller, during registration time this
            // can be automagically detected and registered into the container
            public class FooInstaller : IWindsorInstaller
            {
                public void Install(IWindsorContainer container, IConfigurationStore store)
                {
                    container.Register(Component.For<Foo>());
                }
            }

            // likewise, Autofac calls them modules
            public class SqlServicesModule : Module
            {
                protected override void Load(ContainerBuilder builder)
                {
                    builder.RegisterGeneric(typeof(SqlRepository<>)).As(typeof(IRepository<>));
                }
            }

            // StructureMap calls them registries
            public class MongoServicesRegistry : Registry
            {
                public MongoServicesRegistry()
                {
                    AddType(typeof(IRepository<>), typeof(MongoRepository<>));
                }
            }

            [Test]
            public void But_The_Idea_Is_The_Same()
            {
                // windsor has a "find everything" including referenced assenblies
                var w = new WindsorContainer();
                w.Install(FromAssembly.InThisApplication());
                Assert.IsNotNull(w.Resolve<Foo>()); // picked up from above

                // structuremap has something similar
                var sm = new Container();
                sm.Configure(c => c.Scan(s =>
                {
                    s.AssembliesFromApplicationBaseDirectory();
                    s.LookForRegistries();
                }));
                Assert.IsNotNull(sm.GetInstance<IRepository<Foo>>()); // picked up from above

                // other containers may require some more manual coding, like Autofac
                var cb = new ContainerBuilder();
                cb.RegisterAssemblyModules(Assembly.GetExecutingAssembly());
                var autofac = cb.Build();
                Assert.IsNotNull(autofac.Resolve<IRepository<Foo>>());
            }
        }
    }
}