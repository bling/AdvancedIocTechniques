using System;
using System.Data.SqlClient;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace AdvancedIoCTechniques
{
    public class Containers
    {
        [Test]
        public void Manual_Dependency_Injection_Can_Be_Painful()
        {
            var blog = new ComplexBlog(
                new First0(
                    new Second0(new Third0(), new Third1()),
                    new Second1(new Third0(), new Third1()),
                    new Second2(new Third0(), new Third1())),
                new First1(
                    new Second0(new Third0(), new Third1()),
                    new Second1(new Third0(), new Third1()),
                    new Second2(new Third0(), new Third1())));

            // for small applications this is not a big deal
            // for big applications, managing this can be difficult
        }

        [Test]
        public void So_We_Use_Containers()
        {
            // let's start with Unity, most likely for many of you the first container you used
            var c = new UnityContainer();

            c.RegisterInstance<Func<SqlConnection>>(() => new SqlConnection("localhost"));
            // we just need to tell the container all the different things available
            c.RegisterType<ComplexBlog>();
            c.RegisterType<IFirst0, First0>();
            c.RegisterType<IFirst1, First1>();
            c.RegisterType<ISecond0, Second0>();
            c.RegisterType<ISecond1, Second1>();
            c.RegisterType<ISecond2, Second2>();
            c.RegisterType<IThird0, Third0>();
            c.RegisterType<IThird1, Third1>();

            // and then we can just get the blog and the container figures everything out
            var muchEasier = c.Resolve<ComplexBlog>();
        }

        [Test]
        public void Sometimes_The_Specific_Implementation_Matters()
        {
            var c = new UnityContainer();

            c.RegisterType<IRepository<Foo>, InMemoryRepository<Foo>>();
            c.RegisterType<IRepository<Foo>, SqlRepository<Foo>>("sql");
            c.RegisterType<BlogCache>(
                new InjectionConstructor(
                    new ResolvedParameter<IRepository<Foo>>("sql"),
                    new ResolvedParameter<Foo>()));

            var sql = c.Resolve<IRepository<Foo>>("sql");
            var cache = c.Resolve<BlogCache>();
            Assert.AreEqual(sql.GetType(), cache.Repo.GetType());
        }

        [Test]
        public void Or_We_May_Need_To_Be_Concerned_About_Lifestyles()
        {
            var c = new UnityContainer();

            // like singletons
            // most common: some sort of connection to external source like a database, or caching
            c.RegisterType<ComplexBlog>(new ContainerControlledLifetimeManager());

            // or transient resolutions
            // any sort of "request"
            c.RegisterType<Post>(new TransientLifetimeManager());

            // people who work on ASP.NET or WCF will use scopes based on HttpContext or OperationContext
            c.RegisterType<BlogCache>(new PerHttpContextLifetimeManager());

            // or we can even try to get a little tricky
            c.RegisterType<IRepository<Post>, InMemoryRepository<Post>>(new PerThreadLifetimeManager());
        }

        [Test]
        public void Whats_Next()
        {
            // we've shown that we can use a container to contain things...what else can it do?

            // for starters, let's look at some features that make
            // registering components a lot more maintainable
        }

        #region stuff

        public class ComplexBlog
        {
            public ComplexBlog(IFirst0 a, IFirst1 b)
            {
            }
        }

        public class BlogCache
        {
            public IRepository<Foo> Repo { get; private set; }

            public BlogCache(IRepository<Foo> repo, Foo foo)
            {
                Repo = repo;
            }
        }

        public class PerHttpContextLifetimeManager : LifetimeManager
        {
            public override object GetValue()
            {
                throw new NotImplementedException();
            }

            public override void SetValue(object newValue)
            {
                throw new NotImplementedException();
            }

            public override void RemoveValue()
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}