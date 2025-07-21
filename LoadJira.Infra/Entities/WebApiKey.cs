using System;
using System.Collections.Generic;

namespace LoadJira.Infra.Entities.WebApiKey
{
    public class JiraKey
    {
        public string key { get; set; }
    }

    public class RootWebApiKey
    {
        public int total { get; set; }
        public List<JiraKey> issues { get; set; }
    }


}
