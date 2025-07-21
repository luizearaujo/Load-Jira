namespace LoadJira.Entities
{
    using System;

    public class Issue
    {
        public string Key { get; set; }
        public bool Processed { get; set; }
        public string Summary { get; set; }
        public Status Status { get; set; }
        public Project Project { get; set; }
        public Person Reporter { get; set; }
        public Type Type { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public int? FirstStoryPoint { get; set; }
        public int? LastStoryPoint { get; set; }
        public bool StoryPointProcessed { get; set; }
        public double LeadTime { get; set; }
        public double CycleTime { get; set; }
        public bool TimeProcessed { get; set; }

        public Issue(string key, bool processed = false)
        {
            Key = key;
            Processed = processed;
        }

        public Issue()
        {

        }
    }
}
