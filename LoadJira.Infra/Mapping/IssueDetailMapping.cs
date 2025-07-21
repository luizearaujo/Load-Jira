using LoadJira.Entities;
using LoadJira.Infra.Entities.WebApiIssueDetails;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoadJira.Infra.Mapping
{
    public static class IssueDetailMapping
    {
        public static IList<Detail> Map(RootWebApiIssueDetails jiraIssueDetail)
        {
            var details = new List<Detail>();

            foreach (var jiraDetail in jiraIssueDetail.values)
            {
                foreach (var item in jiraDetail.items)
                {
                    var detail = new Detail();

                    detail.Id = jiraDetail.id;
                    detail.Created = jiraDetail.created;
                    detail.Author = new Person(jiraDetail.author.accountId, jiraDetail.author.displayName);
                    detail.Type = item.field;
                    detail.From = item.fromString;
                    detail.To = item.toString;

                    details.Add(detail);
                }

            }

            return details;
        }
    }
}
