using System.Collections;
using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NUnit.Framework;

namespace AdvancedIoCTechniques
{
    public class AspectOrientedProgramming
    {
        [Test]
        public void Redefine_The_Rules()
        {
            var hash = _windsor.Resolve<IDictionary>();

            hash["hello"] = 123;
            Assert.AreEqual(42, hash["hello"]);
        }

        #region setup

        private IWindsorContainer _windsor;

        [TestFixtureSetUp]
        public void Setup()
        {
            _windsor = new WindsorContainer();
            _windsor.Register(
                Component.For<AlwaysReturns42Interceptor>(),
                Component
                    .For<IDictionary>()
                    .ImplementedBy<Hashtable>()
                    .Interceptors<AlwaysReturns42Interceptor>());
        }

        public class AlwaysReturns42Interceptor : IInterceptor
        {
            public void Intercept(IInvocation invocation)
            {
                invocation.Proceed();
                invocation.ReturnValue = 42;
            }
        }

        #endregion
    }
}