using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty.Lib
{
    public class ObjectId : AnyObjectId
    {
        private static string ZeroIdString;

        static ObjectId()
        {
            ZeroId = new ObjectId(0, 0, 0, 0, 0);
            ZeroIdString = ZeroIdString.ToString();
        }

        public static ObjectId ZeroId { get; private set; }

        protected ObjectId(int new_1, int new_2, int new_3, int new_4, int new_5)
        {

        }

        protected ObjectId(AnyObjectId src)
        {

        }
    }
}
