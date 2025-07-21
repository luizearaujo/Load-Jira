using System;

namespace LoadJira.Entities
{
    public class Status
    {
        private int _id;

        private string _description;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public Status(int id, string description)
        {
            Id = id;
            Description = description;
        }

        public Status()
        {

        }

    }
}