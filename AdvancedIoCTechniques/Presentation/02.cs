using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using NUnit.Framework;

namespace AdvancedIoCTechniques
{
    public class ServiceLocatorPattern
    {
        public class Blog2
        {
            public void Save(Post post)
            {
                var repo = ServiceLocator.Resolve<IRepository<Post>>();
                repo.Save(post);
            }
        }

        // this is a simple solution, but it is not
        // without its problems, let's have a look...



















        [Test]
        public void Why_Is_It_So_Bad()
        {
            // let's try testing this thing
            var blog = new Blog2();

            ServiceLocator.Register<IRepository<Post>>(
                () => new InMemoryRepository<Post>());
            blog.Save(new Post());
        }
















        public class Blog3
        {
            public void Save(Post post)
            {
                var repo = ServiceLocator.Resolve<IRepository<Post>>();
                repo.Save(post);

                var stat = ServiceLocator.Resolve<IStat>();
                stat.Increment(MethodBase.GetCurrentMethod().Name);
            }
        }

        [Test]
        public void It_Can_Only_Get_Worse()
        {
            var blog = new Blog3();

            var post = new Post();
            ServiceLocator.Register<IRepository<Post>>(
                () => new InMemoryRepository<Post>());
            ServiceLocator.Register<IRepository<Post>>(
                () => new InMemoryRepository<Post>());
            blog.Save(post);
        }

        #region helper

        public interface IStat
        {
            void Increment(string name);
        }

        public static class ServiceLocator
        {
            private static readonly Dictionary<Type, Delegate> resolvers = new Dictionary<Type, Delegate>();

            public static void Register<T>(Func<T> resolver)
            {
                resolvers[typeof(T)] = resolver;
            }

            public static T Resolve<T>()
            {
                Delegate func;
                if (resolvers.TryGetValue(typeof(T), out func))
                {
                    return ((Func<T>)func)();
                }

                throw new InvalidOperationException(string.Format("{0} has not been registered.", typeof(T)));
            }
        }

        #endregion
    }
}