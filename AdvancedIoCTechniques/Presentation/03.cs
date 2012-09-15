using System.Linq;
using NUnit.Framework;

namespace AdvancedIoCTechniques
{
    public class Dependency_Injection
    {
        public class BetterBlog
        {
            private readonly IRepository<Post> _repository;

            // we inject an interface of something which can save posts
            public BetterBlog(IRepository<Post> repository)
            {
                _repository = repository;
            }

            public void Save(Post post)
            {
                // when saving, we pass the call into the repository, which can be any implementation
                // SQL, file, whatever storage mechanism is abstracted away
                _repository.Save(post);
            }

            public Post[] FindAllPosts()
            {
                return _repository.FindAll().ToArray();
            }
        }

        [Test]
        public void Now_It_Can_Be_Tested_Too()
        {
            // now testing is much easier, and there are no surprises
            var repo = new InMemoryRepository<Post>();
            var blog = new BetterBlog(repo);
            Assert.IsEmpty(blog.FindAllPosts());
            blog.Save(new Post());
            Assert.IsNotEmpty(blog.FindAllPosts());
        }




        // but don't forget the 'S' in S.O.L.I.D.!!!

















        public class DontForgetAboutSingleReponsibilityPrinciple
        {
            public DontForgetAboutSingleReponsibilityPrinciple(
                Disposable dispo,
                Foo foo,
                Bar bar,
                IFirst0 first0,
                IFirst1 first1,
                IFirst2 first2,
                ISecond0 second0,
                ISecond1 second1,
                ISecond2 second2,
                IThird0 third0,
                IThird1 third1)
            {
            }
        }
    }
}