namespace LoadJira.Entities
{
    public class Type
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


        public Type(int id, string description)
        {
            Id = id;
            Description = description;
        }

        public Type()
        {

        }
    }
}