using System;

namespace LoadJira.Entities
{
    public class Detail
    {
        private int _id;
        private DateTime _created;
        private string _type;
        private string _from;
        private string _to;
        private Person _author;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public DateTime Created
        {
            get { return _created; }
            set { _created = value; }
        }

        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public string From
        {
            get { return _from; }
            set { _from = value; }
        }

        public string To
        {
            get { return _to; }
            set { _to = value; }
        }

        public Person Author
        {
            get { return _author; }
            set { _author = value; }
        }
    }
}
