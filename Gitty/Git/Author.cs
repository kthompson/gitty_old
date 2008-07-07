using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Gitty
{
    public partial class Git
    {
        [Synched]
        public class Author
        {

            public Author(string authorString)
            {
                Match m = Regex.Match(authorString, @"(.*?) <(.*?)> (\d+) (.*)");
                if (m.Success)
                {
                    _name = m.Captures[1].Value;
                    _email = m.Captures[2].Value;
                    _date = DateTime.Parse(m.Captures[3].Value);

                }
            }

            private string _name;
            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            private string _email;
            public string Email
            {
                get { return _email; }
                set { _email = value; }
            }

            private DateTime _date;
            public DateTime Date
            {
                get { return _date; }
                set { _date = value; }
            }
	
            
	
        }
    }
}
