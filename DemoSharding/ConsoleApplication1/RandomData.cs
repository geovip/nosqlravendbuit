using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DemoSharding
{
    public class RandomData
    {
        private Random random;

        public RandomData()
        {
            random = new Random((int)DateTime.Now.Ticks);
        }

        public string RandomString()
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", "");
            return path;
        }
    }
}
