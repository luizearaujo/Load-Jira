using LoadJira.Entities;
using LoadJira.Infra.Entities.WebApiKey;
using LoadJira.Infra.Entities.WebApiIssue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using LoadJira.Infra.Entities.WebApiIssueDetails;
using Serilog;
using System.Threading.Tasks;

namespace LoadJira.Infra.Service
{
    public class JiraWebApiService : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _log;
        private readonly string _jiraUrl;
        private readonly string _jiraUser;
        private readonly string _jiraToken;

        public JiraWebApiService(ILogger log)
        {
            _log = log.ForContext<JiraWebApiService>();
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            // Load configuration
            _jiraUrl = Config.Config.JiraUrl;
            _jiraUser = Config.Config.JiraUser;
            _jiraToken = Config.Config.JiraToken;

            var tokenByte = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_jiraUser}:{_jiraToken}"));
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {tokenByte}");
        }

        public async Task<IList<Issue>> GetKeysAsync(string endDate)
        {
            _log.Information($"Buscando chaves de issues na API do Jira até a data: {endDate ?? "atual"}.");

            var jiraIssues = new List<JiraKey>();
            var startAt = 0;
            var maxResults = 100;
            var total = 1; // Initial value to enter the loop

            while (startAt < total)
            {
                try
                {
                    // Hardcoded JQL - consider moving to configuration
                    var jiraQuery = $@"project+in+(NGBC,NGCAD,NGFDI,NGM,NGPHD,NGTIV,NGW,NGRF,NGRV,NGCC,NGPO)+and+issuetype+in+(Story,Task,Bug,\'Technical+Story\')+and+createdDate>=\'2019-01-01\'+
and+createdDate<=\'{endDate}\'+
and+status=Done+order+by+key";

                    var url = $"{_jiraUrl}/rest/api/3/search?&maxResults={maxResults}&startAt={startAt}&jql={jiraQuery}";

                    _log.Debug($"Chamando API para buscar chaves: {url}");

                    var response = await _httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode(); // Throws on non-success status

                    var responseBody = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeObject<RootWebApiKey>(responseBody);

                    if (responseData?.issues == null || !responseData.issues.Any())
                    {
                        _log.Information("Nenhuma issue retornada na chamada atual da API.");
                        break; // Exit loop if no issues are returned
                    }

                    jiraIssues.AddRange(responseData.issues);
                    total = responseData.total;
                    startAt += maxResults;

                    _log.Information($"Retornadas {responseData.issues.Count} issues. Total até agora: {jiraIssues.Count}. Próximo startAt: {startAt}. Total estimado: {total}.");
                }
                catch (HttpRequestException httpEx)
                {
                    _log.Error(httpEx, $"Erro HTTP ao buscar chaves de issues na API do Jira (startAt: {startAt}).");
                    throw; // Re-throw the exception after logging
                }
                catch (JsonException jsonEx)
                {
                    _log.Error(jsonEx, $"Erro de desserialização ao buscar chaves de issues na API do Jira (startAt: {startAt}).");
                    throw; // Re-throw the exception after logging
                }
                catch (Exception ex)
                {
                    _log.Error(ex, $"Erro inesperado ao buscar chaves de issues na API do Jira (startAt: {startAt}).");
                    throw; // Re-throw the exception after logging
                }
            }

            _log.Information($"Busca de chaves de issues finalizada. Total de chaves encontradas: {jiraIssues.Count}.");
            return Mapping.IssueMapping.Map(jiraIssues);
        }

        public async Task<Issue> GetIssueAsync(string key)
        {
            _log.Information($"Buscando detalhes da issue {key} na API do Jira.");
            try
            {
                var url = $"{_jiraUrl}/rest/api/3/issue/{key}";

                _log.Debug($"Chamando API para buscar issue: {url}");

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Throws on non-success status

                var responseBody = await response.Content.ReadAsStringAsync();
                var myDeserializedClass = JsonConvert.DeserializeObject<RootWebApiIssue>(responseBody);

                _log.Information($"Detalhes da issue {key} obtidos com sucesso.");
                return Mapping.IssueMapping.Map(myDeserializedClass);
            }
            catch (HttpRequestException httpEx)
            {
                _log.Error(httpEx, $"Erro HTTP ao buscar detalhes da issue {key} na API do Jira.");
                // Depending on requirements, might return null or re-throw
                return null; // Returning null for a single issue not found/accessible
            }
            catch (JsonException jsonEx)
            {
                _log.Error(jsonEx, $"Erro de desserialização ao buscar detalhes da issue {key} na API do Jira.");
                throw; // Re-throw the exception after logging
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Erro inesperado ao buscar detalhes da issue {key} na API do Jira.");
                throw; // Re-throw the exception after logging
            }
        }

        public async Task<IList<Detail>> GetDetailsAsync(string key)
        {
            _log.Information($"Buscando changelog para a issue {key} na API do Jira.");
            try
            {
                var url = $"{_jiraUrl}/rest/api/3/issue/{key}/changelog";

                _log.Debug($"Chamando API para buscar changelog: {url}");

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Throws on non-success status

                var responseBody = await response.Content.ReadAsStringAsync();
                var myDeserializedClass = JsonConvert.DeserializeObject<RootWebApiIssueDetails>(responseBody);

                _log.Information($"Changelog para a issue {key} obtido com sucesso.");
                return Mapping.IssueDetailMapping.Map(myDeserializedClass);
            }
            catch (HttpRequestException httpEx)
            {
                _log.Error(httpEx, $"Erro HTTP ao buscar changelog para a issue {key} na API do Jira.");
                // Depending on requirements, might return null or re-throw
                return null; // Returning null for changelog not found/accessible
            }
            catch (JsonException jsonEx)
            {
                _log.Error(jsonEx, $"Erro de desserialização ao buscar changelog para a issue {key} na API do Jira.");
                throw; // Re-throw the exception after logging
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Erro inesperado ao buscar changelog para a issue {key} na API do Jira.");
                throw; // Re-throw the exception after logging
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}

