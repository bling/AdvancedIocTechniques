using System.Data.SqlClient;
using NUnit.Framework;

namespace AdvancedIoCTechniques
{
    public class WhyIoC
    {
        [Test]
        public void Back_To_Basics()
        {
            // first we have a blog
            var blog = new Blog();

            // then we write up a blog post
            var post = new Post
            {
                Author = "bling",
                Content = "All your base are belong to us."
            };

            // finally we have to save it
            blog.Save(post);
        }

        public class Blog
        {
            // but behide the scenes, we are saving to a database
            public void Save(Post post)
            {
                using (var sql = new SqlConnection())
                using (var cmd = sql.CreateCommand())
                {
                    cmd.CommandText = "insert into posts ...";
                    cmd.ExecuteNonQuery();
                }

                // what happens if the database blows up?
                // how do we test?
                // what if we don't want to use SQL anymore?
            }
        }
    }
}