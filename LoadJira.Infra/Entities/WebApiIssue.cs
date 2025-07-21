using System;
using System.Collections.Generic;

namespace LoadJira.Infra.Entities.WebApiIssue
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class StatusCategory
    {
        public string self { get; set; }
        public int id { get; set; }
        public string key { get; set; }
        public string colorName { get; set; }
        public string name { get; set; }
    }

    public class Status
    {
        public string self { get; set; }
        public string description { get; set; }
        public string iconUrl { get; set; }
        public string name { get; set; }
        //public string id { get; set; }
        public int id { get; set; }
        public StatusCategory statusCategory { get; set; }
    }

    public class Priority
    {
        public string self { get; set; }
        public string iconUrl { get; set; }
        public string name { get; set; }
        public string id { get; set; }
    }

    public class Issuetype
    {
        public string self { get; set; }
        //public string id { get; set; }
        public int id { get; set; }
        public string description { get; set; }
        public string iconUrl { get; set; }
        public string name { get; set; }
        public bool subtask { get; set; }
        public int avatarId { get; set; }
        public int hierarchyLevel { get; set; }
    }

    public class Fields2
    {
        public string summary { get; set; }
        public Status status { get; set; }
        public Priority priority { get; set; }
        public Issuetype issuetype { get; set; }
        public object resolution { get; set; }
        public object lastViewed { get; set; }
        public object aggregatetimeoriginalestimate { get; set; }
        public List<object> issuelinks { get; set; }
        public object assignee { get; set; }
        public List<Subtask> subtasks { get; set; }
        public Votes votes { get; set; }
        public Worklog worklog { get; set; }
        public Timetracking timetracking { get; set; }
        public object environment { get; set; }
        public object duedate { get; set; }
        public Customfield11320 customfield_11320 { get; set; }
        public Customfield11321 customfield_11321 { get; set; }
        public Customfield11322 customfield_11322 { get; set; }
        public Customfield11323 customfield_11323 { get; set; }
        public Customfield10113 customfield_10113 { get; set; }
        public Customfield11324 customfield_11324 { get; set; }
        public Customfield11319 customfield_11319 { get; set; }
        public DateTime? customfield_10100 { get; set; }
        public object timeestimate { get; set; }
        public Customfield11302 customfield_11302 { get; set; }
        public Customfield11414 customfield_11414 { get; set; }
        public Customfield11413 customfield_11413 { get; set; }
        public Customfield11416 customfield_11416 { get; set; }
        public Customfield11417 customfield_11417 { get; set; }
        public Creator creator { get; set; }
        public Customfield11412 customfield_11412 { get; set; }
        public Customfield11402 customfield_11402 { get; set; }
        public object timespent { get; set; }
        public long workratio { get; set; }
        public Customfield10301 customfield_10301 { get; set; }
        public Customfield11391 customfield_11391 { get; set; }
        public List<string> labels { get; set; }
        public List<object> components { get; set; }
        public Reporter reporter { get; set; }
        public Customfield11379 customfield_11379 { get; set; }
        public Progress progress { get; set; }
        public Project project { get; set; }
        public object resolutiondate { get; set; }
        public Watches watches { get; set; }
        public Customfield11350 customfield_11350 { get; set; }
        public Customfield11352 customfield_11352 { get; set; }
        public Customfield11347 customfield_11347 { get; set; }
        public DateTime updated { get; set; }
        public object timeoriginalestimate { get; set; }
        public Description description { get; set; }
        public Customfield11337 customfield_11337 { get; set; }
        public Customfield10122 customfield_10122 { get; set; }
        public Customfield11333 customfield_11333 { get; set; }
        public Customfield10123 customfield_10123 { get; set; }
        public Customfield11326 customfield_11326 { get; set; }
        public List<object> customfield_10115 { get; set; }
        public Customfield11325 customfield_11325 { get; set; }
        public Customfield11327 customfield_11327 { get; set; }
        public Comment comment { get; set; }
        public DateTime statuscategorychangedate { get; set; }
        public List<object> fixVersions { get; set; }
        public List<object> versions { get; set; }
        public Aggregateprogress aggregateprogress { get; set; }
        public Issuerestriction issuerestriction { get; set; }
        public DateTime created { get; set; }
        public object security { get; set; }
        public List<Attachment> attachment { get; set; }
    }

    public class Subtask
    {
        public string id { get; set; }
        public string key { get; set; }
        public string self { get; set; }
        public Fields2 fields { get; set; }
    }

    public class Votes
    {
        public string self { get; set; }
        public int votes { get; set; }
        public bool hasVoted { get; set; }
    }

    public class Worklog
    {
        public int startAt { get; set; }
        public int maxResults { get; set; }
        public int total { get; set; }
        public List<object> worklogs { get; set; }
    }

    public class Timetracking
    {
    }

    public class I18nErrorMessage
    {
        public string i18nKey { get; set; }
        public List<object> parameters { get; set; }
    }

    public class Customfield11320
    {
        public string errorMessage { get; set; }
        public I18nErrorMessage i18nErrorMessage { get; set; }
    }

    public class Customfield11321
    {
        public string errorMessage { get; set; }
        public I18nErrorMessage i18nErrorMessage { get; set; }
    }

    public class Customfield11322
    {
        public string errorMessage { get; set; }
        public I18nErrorMessage i18nErrorMessage { get; set; }
    }

    public class Customfield11323
    {
        public string errorMessage { get; set; }
        public I18nErrorMessage i18nErrorMessage { get; set; }
    }

    public class NonEditableReason
    {
        public string reason { get; set; }
        public string message { get; set; }
    }

    public class Customfield10113
    {
        public bool hasEpicLinkFieldDependency { get; set; }
        public bool showField { get; set; }
        public NonEditableReason nonEditableReason { get; set; }
    }

    public class Customfield11324
    {
        public string errorMessage { get; set; }
        public I18nErrorMessage i18nErrorMessage { get; set; }
    }

    public class Customfield11319
    {
        public string errorMessage { get; set; }
        public I18nErrorMessage i18nErrorMessage { get; set; }
    }

    public class Customfield11302
    {
        public string errorMessage { get; set; }
        public I18nErrorMessage i18nErrorMessage { get; set; }
    }

    public class Customfield11414
    {
        public string errorMessage { get; set; }
        public I18nErrorMessage i18nErrorMessage { get; set; }
    }

    public class Customfield11413
    {
        public string errorMessage { get; set; }
        public I18nErrorMessage i18nErrorMessage { get; set; }
    }

    public class Customfield11416
    {
        public string errorMessage { get; set; }
        public I18nErrorMessage i18nErrorMessage { get; set; }
    }

    public class Customfield11417
    {
        public string errorMessage { get; set; }
        public I18nErrorMessage i18nErrorMessage { get; set; }
    }

    public class AvatarUrls
    {
        public string _48x48 { get; set; }
        public string _24x24 { get; set; }
        public string _16x16 { get; set; }
        public string _32x32 { get; set; }
    }

    public class Creator
    {
        public string self { get; set; }
        public string accountId { get; set; }
        public AvatarUrls avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
        public string accountType { get; set; }
    }

    public class Customfield11412
    {
        public string errorMessage { get; set; }
        public I18nErrorMessage i18nErrorMessage { get; set; }
    }

    public class Customfield11402
    {
        public string errorMessage { get; set; }
        public I18nErrorMessage i18nErrorMessage { get; set; }
    }

    public class Child
    {
        public string self { get; set; }
        public string value { get; set; }
        public string id { get; set; }
    }

    public class Customfield10301
    {
        public string self { get; set; }
        public string value { get; set; }
        public string id { get; set; }
        public Child child { get; set; }
    }

    public class Customfield11391
    {
        public string errorMessage { get; set; }
        public I18nErrorMessage i18nErrorMessage { get; set; }
    }

    public class Reporter
    {
        public string self { get; set; }
        public string accountId { get; set; }
        public AvatarUrls avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
        public string accountType { get; set; }
    }

    public class Content2
    {
        public string type { get; set; }
        public string text { get; set; }
        public List<Content2> content { get; set; }
        public Attrs attrs { get; set; }
    }

    public class Customfield11379
    {
        public int version { get; set; }
        public string type { get; set; }
        public List<Content2> content { get; set; }
    }

    public class Progress
    {
        public int progress { get; set; }
        public int total { get; set; }
    }

    public class Project
    {
        public string self { get; set; }
        public string id { get; set; }
        public string key { get; set; }
        public string name { get; set; }
        public string projectTypeKey { get; set; }
        public bool simplified { get; set; }
        public AvatarUrls avatarUrls { get; set; }
    }

    public class Watches
    {
        public string self { get; set; }
        public int watchCount { get; set; }
        public bool isWatching { get; set; }
    }

    public class Customfield11350
    {
        public int version { get; set; }
        public string type { get; set; }
        public List<Content2> content { get; set; }
    }

    public class Customfield11352
    {
        public string self { get; set; }
        public string value { get; set; }
        public string id { get; set; }
    }

    public class Customfield11347
    {
        public string self { get; set; }
        public string value { get; set; }
        public string id { get; set; }
    }

    public class Description
    {
        public int version { get; set; }
        public string type { get; set; }
        public List<Content2> content { get; set; }
    }

    public class Customfield11337
    {
        public string self { get; set; }
        public string value { get; set; }
        public string id { get; set; }
    }

    public class Customfield10122
    {
        public string errorMessage { get; set; }
        public I18nErrorMessage i18nErrorMessage { get; set; }
    }

    public class Customfield11333
    {
        public string errorMessage { get; set; }
        public I18nErrorMessage i18nErrorMessage { get; set; }
    }

    public class Customfield10123
    {
        public string errorMessage { get; set; }
        public I18nErrorMessage i18nErrorMessage { get; set; }
    }

    public class Customfield11326
    {
        public string errorMessage { get; set; }
        public I18nErrorMessage i18nErrorMessage { get; set; }
    }

    public class Customfield11325
    {
        public string errorMessage { get; set; }
        public I18nErrorMessage i18nErrorMessage { get; set; }
    }

    public class Customfield11327
    {
        public string errorMessage { get; set; }
        public I18nErrorMessage i18nErrorMessage { get; set; }
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
    }

    public class Attrs
    {
        public string url { get; set; }
    }

    public class Body
    {
        public int version { get; set; }
        public string type { get; set; }
        public List<Content2> content { get; set; }
    }

    public class UpdateAuthor
    {
        public string self { get; set; }
        public string accountId { get; set; }
        public AvatarUrls avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
        public string accountType { get; set; }
    }

    public class Comment2
    {
        public string self { get; set; }
        public string id { get; set; }
        public Author author { get; set; }
        public Body body { get; set; }
        public UpdateAuthor updateAuthor { get; set; }
        public DateTime created { get; set; }
        public DateTime updated { get; set; }
        public bool jsdPublic { get; set; }
    }

    public class Comment
    {
        public List<Comment> comments { get; set; }
        public string self { get; set; }
        public int maxResults { get; set; }
        public int total { get; set; }
        public int startAt { get; set; }
    }

    public class Aggregateprogress
    {
        public int progress { get; set; }
        public int total { get; set; }
    }

    public class Issuerestrictions
    {
    }

    public class Issuerestriction
    {
        public Issuerestrictions issuerestrictions { get; set; }
        public bool shouldDisplay { get; set; }
    }

    public class Attachment
    {
        public string self { get; set; }
        public string id { get; set; }
        public string filename { get; set; }
        public Author author { get; set; }
        public DateTime created { get; set; }
        public int size { get; set; }
        public string mimeType { get; set; }
        public string content { get; set; }
    }

    public class RootWebApiIssue
    {
        public string expand { get; set; }
        public string id { get; set; }
        public string self { get; set; }
        public string key { get; set; }
        public Fields2 fields { get; set; }
    }

}
