using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Features.OwnedInstances;
using Castle.DynamicProxy;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NUnit.Framework;

namespace AdvancedIoCTechniques
{
    public class ScopedLifetimes
    {
        // let's dive deeper into scoped lifetimes...

        public void Scoping_Is_A_Natural_Way_We_Build_Applications()
        {
            /* 
             *   application   session    request     method
             *        |           |           |          |
             *        |---------->|           |          |
             *        |           |---------->|          |
             *        |           |           |--------->|
             *        |           |           |          |
             *        |           |           |<---------|
             *        |           |<----------|          |
             *        |<----------|           |          |
             *        |           |           |          |
             */

            // Most applications can be solved elegantly with
            // the following 3 dependency lifestyles:
            //   1) new instance every resolve (transient)
            //   2) same instance every resolve (singleton)
            //   3) same instance at scope level

            // one important rule!!
            // do NOT hold onto a dependency that should live shorter than you
            // i.e. application should create *and dispose* all sessions it creates
        }

        [Test]
        public void Explicit_Scope_Declaration()
        {
            var cb = new ContainerBuilder();
            cb.RegisterType<Application>().SingleInstance();

            // every session gets its own scope
            cb.RegisterType<Session>().OwnedByLifetimeScope();

            // every request is tied to the scope of the session that created it
            cb.RegisterType<Request>().OwnedByLifetimeScope();

            cb.RegisterType<Foo>().InstancePerOwned<Session>();
            
            var c = cb.Build();

            var app = c.Resolve<Application>();
            using (var session = app.CreateSession())
            {
                Foo f1, f2;

                Request r1;
                using (var request = session.Value.CreateRequest())
                {
                    r1 = request.Value;
                    f1 = r1.Foo;
                    request.Value.Process("something");
                }

                Request r2;
                using (var request = session.Value.CreateRequest())
                {
                    r2 = request.Value;
                    f2 = r2.Foo;
                    request.Value.Process("something");
                }

                Assert.AreSame(f1, f2);
                Assert.AreNotSame(r1, r2);
                Assert.IsTrue(r1.IsDisposed);
                Assert.IsTrue(r2.IsDisposed);
            }

            // using blocks guruantee proper disposal of all dependencies
        }








        public interface IScope : IDisposable
        {
            T Resolve<T>();
        }

        [Test]
        public void Simulating_Scoped_Lifestyles_With_Windsor()
        {
            var w = new WindsorContainer();

            // DISCLAIMER!!!!
            // I wrote this like 1am in the morning in like 10 minutes,
            // so you should probably test it more than this.... ;-)


            w.AddFacility<TypedFactoryFacility>();
            w.Register(Component.For<ScopeCacheInterceptor>()
                           .DependsOn(new { container = w })
                           .LifestyleTransient());
            w.Register(Component.For(typeof(IScope))
                           .Interceptors<ScopeCacheInterceptor>()
                           .AsFactory()
                           .LifestyleTransient());
            w.Register(Component.For<Disposable>()
                           .LifestyleCustom<ScopedToFactoryLifestyleManager>());

            Disposable a, b, c, d;
            using (var scope = w.Resolve<IScope>())
            {
                a = scope.Resolve<Disposable>();
                b = scope.Resolve<Disposable>();
                Assert.AreSame(a, b);

                using (var scope2 = scope.Resolve<IScope>())
                {
                    c = scope2.Resolve<Disposable>();
                    d = scope2.Resolve<Disposable>();
                    Assert.AreSame(c, d);
                    Assert.AreNotSame(b, c);
                }

                Assert.IsTrue(c.IsDisposed);
                Assert.IsTrue(d.IsDisposed);
            }

            Assert.IsTrue(a.IsDisposed);
            Assert.IsTrue(b.IsDisposed);
        }




















        public class Application
        {
            private readonly Func<Owned<Session>> _sessionFactory;

            public Application(Func<Owned<Session>> sessionFactory)
            {
                _sessionFactory = sessionFactory;
            }

            public Owned<Session> CreateSession()
            {
                return _sessionFactory();
            }
        }

        public class Session
        {
            private readonly Func<Owned<Request>> _requestFactory;
            
            public Session(Func<Owned<Request>> requestFactory)
            {
                _requestFactory = requestFactory;
            }

            public Owned<Request> CreateRequest()
            {
                return _requestFactory();
            }
        }

        public class Request : Disposable
        {
            public Foo Foo { get; set; }

            public Request(Foo foo)
            {
                Foo = foo;
            }

            public void Process(string operation)
            {
            }
        }

        #region windsor scope implementation

        public class ScopeCacheInterceptor : IInterceptor, IDisposable
        {
            private readonly IWindsorContainer _container;

            public ScopeCacheInterceptor(IWindsorContainer container)
            {
                _container = container;
            }

            private readonly Dictionary<MethodInfo, object> _scopeCache = new Dictionary<MethodInfo, object>();

            public void Intercept(IInvocation invocation)
            {
                if (invocation.Method.ReturnType == typeof(void))
                {
                    invocation.Proceed();
                    return;
                }

                var handler = _container.Kernel.GetHandler(invocation.Method.ReturnType);
                if (handler.ComponentModel.CustomLifestyle == typeof(ScopedToFactoryLifestyleManager))
                {
                    object value;
                    if (_scopeCache.TryGetValue(invocation.Method, out value))
                    {
                        invocation.ReturnValue = value;
                        return;
                    }

                    invocation.Proceed();
                    _scopeCache[invocation.Method] = invocation.ReturnValue;
                }
                else
                {
                    invocation.Proceed();
                }
            }

            public void Dispose()
            {
                foreach (var obj in _scopeCache.Values)
                {
                    _container.Release(obj);
                }
            }
        }

        public sealed class ScopedToFactoryLifestyleManager : TransientLifestyleManager
        {
        }

        #endregion
    }
}