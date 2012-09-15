using System;
using Autofac;
using Autofac.Features.OwnedInstances;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Releasers;
using Castle.Windsor;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Ninject;

namespace AdvancedIoCTechniques
{
    public class CleaningUpGarbage
    {
        [Test]
        public void The_Rule_Of_Object_Ownership()
        {
            // whoever creates an resource is also responsible for disposing it
            // other common themes: locking, events, subscriptions, etc.
        }










        // Component Burden
        // ----------------

        public class SomeClass : IDisposable
        {
            private readonly IDependency _dependency;

            public SomeClass(IDependency dependency)
            {
                _dependency = dependency;
            }

            public void Dispose()
            {
                // to dispose or not to dispose,
                // that is the question...
                _dependency.Dispose();
            }
        }


        // we should think about the 'I' from S.O.L.I.D.
        















        [Test]
        public void Windsor_Takes_Component_Burden_Very_Seriously()
        {
            // newbies of Windsor inevitably run into this sooner or later

            var w = new WindsorContainer();
            w.Register(Component.For<Disposable>().LifestyleTransient());

            var d = w.Resolve<Disposable>();
            var wr = new WeakReference(d);
            d = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Assert.IsTrue(wr.IsAlive); // container is still tracking it

            w.Release(wr.Target);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Assert.IsFalse(wr.IsAlive);
        }











        [Test]
        public void Autofac_Also_Does_So_As_Well()
        {
            var cb = new ContainerBuilder();
            cb.RegisterType<Disposable>()
                .OwnedByLifetimeScope(); // this is the default
            var c = cb.Build();

            WeakReference wr;

            // in autofac land, you should never resolve from the root container
            using (var scope = c.BeginLifetimeScope())
            {
                var disposable = scope.Resolve<Disposable>();
                wr = new WeakReference(disposable);
                disposable = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                Assert.IsTrue(wr.IsAlive);
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Assert.IsFalse(wr.IsAlive);   
        }




        [Test]
        public void Pretty_Much_All_Other_Containers_Copy_This()
        {
            var ninject = new StandardKernel();
            ninject.Bind<Disposable>().ToSelf().InTransientScope();

            WeakReference wr;
            using (var scope = ninject.BeginBlock())
            {
                var d = scope.Get<Disposable>();
                wr = new WeakReference(d);
                d = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                Assert.IsTrue(wr.IsAlive);
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Assert.IsFalse(wr.IsAlive);
        }

        [Test]
        public void And_Another_Example()
        {
            var sm = new StructureMap.Container();
            sm.Configure(x => x.For<Disposable>().Transient());

            WeakReference wr;
            using (var scope = sm.GetNestedContainer())
            {
                var d = scope.GetInstance<Disposable>();
                wr = new WeakReference(d);
                d = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                Assert.IsTrue(wr.IsAlive);
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Assert.IsFalse(wr.IsAlive);
        }

        // now that we understand why releasing is important,
        // how do we inject this abstraction into the component?






        [Test]
        public void Using_Wrappers()
        {
            // another approach that comes from Autofac

            var cb = new ContainerBuilder();
            cb.RegisterType<Disposable>().AsSelf().OwnedByLifetimeScope();
            var c = cb.Build();

            Disposable d;
            using (var instance = c.Resolve<Owned<Disposable>>())
            {
                d = instance.Value;
                Assert.That(d.IsDisposed, Is.False);
            }

            Assert.That(d.IsDisposed, Is.True);

            // and now we can inject the factory into components
            var factory = c.Resolve<Func<Owned<Disposable>>>();
            Assert.NotNull(factory);
            Assert.NotNull(factory());
        }




















        [Test]
        public void Using_Black_Magic()
        {
            /*
            public interface IFactory<out T> : IDisposable
            {
                T Create<T>();
            }
            */

            var w = new WindsorContainer();
            w.AddFacility<TypedFactoryFacility>();
            w.Register(Component.For(typeof(IFactory<>))
                .AsFactory());
            w.Register(Component.For<Disposable>().LifestyleTransient());

            // windsor will create a runtime proxy behind the scenes
            Disposable d;
            using (var factory = w.Resolve<IFactory<Disposable>>())
            {
                d = factory.Create();
                Assert.That(d.IsDisposed, Is.False);
            }

            Assert.That(d.IsDisposed, Is.True);

            // since this is an interface that you defined, your domain never needs
            // to reference the container's namespace
        }

        #region setup

        public interface IDependency : IDisposable
        {
        }
        
        public interface IFactory<out T> : IDisposable
        {
            T Create();
        }

        #endregion
    }
}