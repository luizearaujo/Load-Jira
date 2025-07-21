using LoadJira.Infra.Repository;
using Serilog;

namespace LoadJira.Domain
{
    public abstract class BaseService
    {
        protected readonly IssueRepository _issueRepository;
        protected readonly DetailRepository _detailRepository;
        protected readonly ILogger _log;

        public BaseService(ILogger log, IssueRepository issueRepository, DetailRepository detailRepository)
        {
            _log = log.ForContext(GetType());
            _issueRepository = issueRepository;
            _detailRepository = detailRepository;
        }
    }
}

