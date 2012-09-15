using System;
using NUnit.Framework;
using Ninject;

namespace AdvancedIoCTechniques
{
    public class OtherLifetimes
    {
        [Test]
        public void Cache_And_Collect()
        {
            // unique feature of Ninject called "Cache-and-Collect"
            var ninject = new StandardKernel();

            // we define any object to be the "scope"
            object scope = new object();

            ninject.Bind<Third0>().ToSelf().InScope(_ => scope);

            Assert.AreSame(
                ninject.Get<Third0>(),
                ninject.Get<Third0>());

            scope = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // since the scope has been garbage collected, these instances are no longer common
            Assert.AreNotSame(
                ninject.Get<Third0>(),
                ninject.Get<Third0>());
        }
    }
}