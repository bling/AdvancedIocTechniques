using System;
using System.Collections.Generic;

namespace AdvancedIoCTechniques
{
    internal class Program
    {
        private static void Main(string[] args)
        {
        }
    }

    public class Disposable : IDisposable
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            this.IsDisposed = true;
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }

    public interface IRepository<T>
        where T : IHaveId
    {
        IEnumerable<T> FindAll();

        T Find(Guid id);

        void Save(T item);
    }

    public class InMemoryRepository<T> : IRepository<T>
        where T : IHaveId
    {
        private readonly Dictionary<Guid, T> collection = new Dictionary<Guid, T>();

        public IEnumerable<T> FindAll()
        {
            return this.collection.Values;
        }

        public T Find(Guid id)
        {
            T result;
            this.collection.TryGetValue(id, out result);
            return result;
        }

        public void Save(T item)
        {
            this.collection[item.Id] = item;
        }
    }

    public class SqlRepository<T> : InMemoryRepository<T> where T : IHaveId
    {
    }

    public class MongoRepository<T> : InMemoryRepository<T> where T : IHaveId
    {
    }

    public class Foo : Disposable, IHaveId
    {
        public Guid Id { get; set; }
    }

    public class Bar : Disposable, IHaveId
    {
        public Guid Id { get; set; }
    }

    public interface IHaveId
    {
        Guid Id { get; }
    }

    public class Post : IHaveId
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Author { get; set; }
        public string Content { get; set; }
        public IList<Post> Comments { get; set; }
    }

    public class Blog
    {
    }
}