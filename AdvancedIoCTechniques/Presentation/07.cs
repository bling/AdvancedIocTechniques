using System;
using Autofac;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace AdvancedIoCTechniques
{
    public class ContainerFacade
    {
        [Test]
        public void What_About_The_Resolution()
        {
            // we've talked about registration, let's talk about resolution!
        }

        /*
         * time t
         * ----------------------------------------------------->
         *         |                       |             |
         *     resolve A                resolve B    resolve C
         */
















        public class TheNaiveApproach
        {
            private readonly IContainer _container;

            public TheNaiveApproach(IContainer container)
            {
                _container = container;
            }
        }

















        [Test]
        public void So_How_Do_We_Use_The_Container_Without_A_Direct_Reference() // ??
        {
            // there are many standard ways that almost all containers support by default
        }










        public class TheOneMostLikeNew
        {
            private readonly Func<IFirst> _first;
            private readonly Func<ISecond> _second;
            private readonly Func<ISecond0, ISecond1, IFirst0> _firstWithOverrides;

            public TheOneMostLikeNew(Func<IFirst> first, Func<ISecond> second, Func<ISecond0, ISecond1, IFirst0> firstWithOverrides)
            {
                _first = first;
                _second = second;
                _firstWithOverrides = firstWithOverrides;
            }

            public void Process(object message)
            {
                var instance1 = _first();
                var instance2 = _second();
                var instance3 = _firstWithOverrides(null, null);

                //instance1.DoSomething(message);
                //instance2.DoSomething(message);
            }
        }

















        public class DeferredSingletonUse
        {
            private readonly Lazy<IFirst> _dependencyWhichTakesALongTimeToInit;

            public DeferredSingletonUse(Lazy<IFirst> dependencyWhichTakesALongTimeToInit)
            {
                _dependencyWhichTakesALongTimeToInit = dependencyWhichTakesALongTimeToInit;
            }
        }
        
        
        
        
        
        
    


















        public delegate IFirst0 MoreSpecific(ISecond0 can, ISecond1 name, ISecond2 arguments);

        [Test]
        public void Why_Delegates_May_Be_Better()
        {
            Func<int, int, int, int, int> func;
            MoreSpecific ms;
            //ms()
        }

        public interface ISuperFactory
        {
            IFirst TheMostControl(ISecond0 x);
            IFirst TheMostControl(ISecond0 x, ISecond1 y);
            IFirst TheMostControl(ISecond0 x, ISecond1 y, ISecond2 z);
        }



        [Test]
        public void Now_That_We_Abstracted_Creation_Of_Objects()
        {
            // what is missing??
        }
    }

    public interface IMessageHandlerFactory
    {
    }
}