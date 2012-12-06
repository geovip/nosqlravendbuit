using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace sqldbuit
{
    public class RandomData
    {
        private Random random;

        public RandomData()
        {
            random = new Random((int)DateTime.Now.Ticks);
        }

        public string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;

            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        public string RandomString()
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", "");
            return path;
        }
    }
}
