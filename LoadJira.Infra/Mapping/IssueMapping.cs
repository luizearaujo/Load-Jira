using LoadJira.Entities;
using LoadJira.Infra.Entities.WebApiKey;
using LoadJira.Infra.Entities.WebApiIssue;
using System;
using System.Collections.Generic;

namespace LoadJira.Infra.Mapping
{
    public static class IssueMapping
    {
        public static Issue Map(JiraKey jiraIssue)
        {
            return new Issue(jiraIssue.key);
        }

        public static IList<Issue> Map(IList<JiraKey> jiraIssues)
        {
            var result = new List<Issue>();

            foreach (var jiraIssue in jiraIssues)
                result.Add(new Issue(jiraIssue.key));

            return result;
        }

        public static Issue Map(RootWebApiIssue jiraIssue)
        {
            var data = new Issue();

            data.Processed = true;
            data.Key = jiraIssue.key;
            data.Created = jiraIssue.fields.created;
            data.Summary = jiraIssue.fields.summary;
            data.Reporter = new Person(jiraIssue.fields.reporter.accountId, jiraIssue.fields.reporter.displayName);
            data.Type = new LoadJira.Entities.Type(jiraIssue.fields.issuetype.id, jiraIssue.fields.issuetype.name);
            data.Project = new LoadJira.Entities.Project(jiraIssue.fields.project.key, jiraIssue.fields.project.name);
            data.Status = new LoadJira.Entities.Status(jiraIssue.fields.status.id, jiraIssue.fields.status.description);

            if (jiraIssue.fields.description != null)
            {
                foreach (var content1 in jiraIssue.fields.description.content)
                {
                    if (content1 != null)
                    {
                        if (!string.IsNullOrEmpty(content1.text))
                            data.Description += $"{content1.text}{Environment.NewLine}";

                        if (content1.content != null)
                        {
                            foreach (var content2 in content1.content)
                            {
                                if (content2 != null)
                                {
                                    if (!string.IsNullOrEmpty(content2.text))
                                        data.Description += $"{content2.text}{Environment.NewLine}";

                                    if (content2.content != null)
                                        foreach (var content3 in content2.content)
                                            if (content3 != null && !string.IsNullOrEmpty(content2.text))
                                                data.Description += $"{content2.text}{Environment.NewLine}";

                                }
                            }
                        }
                        
                    }
                }
            }
            

            return data;
        }
    }
}
