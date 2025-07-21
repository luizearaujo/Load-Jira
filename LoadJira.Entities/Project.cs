namespace LoadJira.Entities
{
    public class Project
    {
        private string _key;

        private string _name;

        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }


        public Project(string key, string name)
        {
            Key = key;
            Name = name;
        }

        public Project()
        {

        }
    }
}