using System;
using System.Collections.Generic;

namespace LoadJira.Infra.Entities.WebApiIssueDetails
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class AvatarUrls
    {
        public string _48x48 { get; set; }
        public string _24x24 { get; set; }
        public string _16x16 { get; set; }
        public string _32x32 { get; set; }
    }

    public class Author
    {
        public string self { get; set; }
        public string accountId { get; set; }
        public AvatarUrls avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
        public string accountType { get; set; }
        public string emailAddress { get; set; }
    }

    public class Item
    {
        public string field { get; set; }
        public string fieldtype { get; set; }
        public string fieldId { get; set; }
        public string from { get; set; }
        public string fromString { get; set; }
        public string to { get; set; }
        public string toString { get; set; }
        public string tmpFromAccountId { get; set; }
        public string tmpToAccountId { get; set; }
    }

    public class Value
    {
        public int id { get; set; }
        public Author author { get; set; }
        public DateTime created { get; set; }
        public List<Item> items { get; set; }
    }

    public class RootWebApiIssueDetails
    {
        public string self { get; set; }
        public int maxResults { get; set; }
        public int startAt { get; set; }
        public int total { get; set; }
        public bool isLast { get; set; }
        public List<Value> values { get; set; }
    }


}
