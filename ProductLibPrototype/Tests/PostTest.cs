
using NUnit.Framework;

using ProductLibPrototype.Managers;

namespace ProductLibPrototype.Tests
{
   
    public class PostTest
    {
     
        public void TestPost()
        {
            PostCreator pc = new PostCreator();
            pc.CreatePost("TEST", "TST");


        }
    }
}