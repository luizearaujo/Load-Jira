## 1. Visão Geral da Aplicação

[Voltar](README.md)

LoadJira é uma aplicação console desenvolvida em C# (.NET) projetada para extrair dados de issues (tarefas, bugs, histórias, etc.) do Jira Cloud via API REST. Após a extração, os dados são processados e persistidos em um banco de dados SQL Server. A aplicação pode ser executada sob demanda para carregar dados históricos e calcular métricas ágeis essenciais, como Lead Time e Cycle Time, além de gerenciar Story Points.

## 2. Arquitetura da Aplicação

A arquitetura do LoadJira é modular, organizada em camadas lógicas para garantir a separação de responsabilidades, manutenibilidade e testabilidade. As principais camadas são:

### 2.1. LoadJira.Console (Camada de Execução)

- **Responsabilidade:** Atua como o ponto de entrada da aplicação. É responsável por interpretar os argumentos de linha de comando, inicializar o sistema de logging (Serilog) e orquestrar a execução dos serviços de negócio.
- **Tecnologias:** C#, Serilog.

### 2.2. LoadJira.Domain (Camada de Negócio)

- **Responsabilidade:** Contém a lógica de negócio central da aplicação. Os serviços nesta camada coordenam as operações, utilizando os repositórios para acesso a dados e os serviços de API para comunicação externa. É aqui que as regras de negócio para processamento de issues, cálculo de Story Points e tempos são aplicadas.
- **Tecnologias:** C#, Serilog.

### 2.3. LoadJira.Infra (Camada de Infraestrutura)

- **Responsabilidade:** Abstrai os detalhes de implementação de acesso a dados (banco de dados SQL Server) e comunicação com APIs externas (Jira REST API). Inclui repositórios para persistência de entidades e serviços para interação com APIs de terceiros.
- **Tecnologias:** C#, Dapper (ORM), HttpClient, Newtonsoft.Json, Serilog.

### 2.4. LoadJira.Entities (Camada de Entidades)

- **Responsabilidade:** Define os modelos de dados (POCOs - Plain Old C# Objects) que representam as entidades de negócio da aplicação (e.g., Issue, Person, Project, Status, Detail). Essas entidades são usadas em todas as camadas para garantir a consistência dos dados.
- **Tecnologias:** C#.

### 2.5. LoadJira.Config (Camada de Configuração)

- **Responsabilidade:** Armazena configurações globais da aplicação, como credenciais de acesso ao Jira (usuário, token), URL da API do Jira e string de conexão com o banco de dados SQL Server.
- **Tecnologias:** C#.

## 3. Estrutura de Classes e Responsabilidades

### 3.1. Classes de Serviço (LoadJira.Domain)

- **`BaseService.cs`:** Classe abstrata base para os serviços de negócio, fornecendo injeção de `ILogger` e repositórios comuns.
- **`IssueService.cs`:** Orquestra a carga e o processamento de issues do Jira, interagindo com a API do Jira e diversos repositórios para persistência e atualização de dados.
- **`StoryPointService.cs`:** Calcula e persiste os Story Points (primeiro e último valor) das issues, analisando o histórico de detalhes.
- **`TimeService.cs`:** Calcula e persiste o Lead Time e o Cycle Time das issues, com base nas mudanças de status registradas.

### 3.2. Classes de Infraestrutura (LoadJira.Infra)

- **`JiraWebApiService.cs`:** Gerencia a comunicação assíncrona com a API REST do Jira para buscar issues, detalhes e changelogs, incluindo autenticação e desserialização JSON.
- **`BaseRepository.cs`:** Classe abstrata genérica para acesso a dados, responsável por gerenciar a abertura e o fechamento seguro de conexões SQL e fornecer métodos abstratos para operações de salvamento.
- **Repositórios Específicos (`IssueRepository.cs`, `DetailRepository.cs`, `PersonRepository.cs`, `ProjectRepository.cs`, `StatusRepository.cs`, `TypeRepository.cs`):** Herdam de `BaseRepository<T>` e implementam a lógica CRUD (Create, Read, Update, Delete) para suas respectivas entidades usando Dapper.

### 3.3. Entidades (LoadJira.Entities)

- **`Issue.cs`:** Representa uma issue do Jira, incluindo metadados, status de processamento e campos para Story Points e tempos calculados.
- **`Detail.cs`:** Representa um item do changelog de uma issue, detalhando mudanças de campos como status ou Story Points.
- **`Person.cs`:** Representa um usuário do Jira (e.g., reportador, autor de mudanças).
- **`Project.cs`:** Representa um projeto do Jira.
- **`Status.cs`:** Representa um status de fluxo de trabalho do Jira.
- **`Type.cs`:** Representa um tipo de issue do Jira (e.g., Story, Bug, Task).

### 3.4. Mapeamento (LoadJira.Infra.Mapping)

- **`IssueMapping.cs`:** Mapeia objetos da API do Jira para as entidades de domínio `Issue`.
- **`IssueDetailMapping.cs`:** Mapeia dados de changelog da API do Jira para as entidades de domínio `Detail`.

## 4. Banco de Dados

A aplicação LoadJira utiliza um banco de dados SQL Server para persistir os dados extraídos do Jira. A estrutura é relacional, otimizada para armazenamento e consulta de dados de issues e suas métricas.

### 4.1. Tabelas Principais

As tabelas são criadas e gerenciadas por scripts SQL localizados em `LoadJira.Infra/Repository/sql/`:

- **`Type`:** Tipos de issues (Id, Name).
- **`Project`:** Projetos do Jira (Key, Name).
- **`Status`:** Status de issues (Id, Name).
- **`Person`:** Pessoas (Id, Name).
- **`Issue`:** Issues do Jira (Key, Summary, Description, Created, Processed, StoryPointProcessed, TimeProcessed, FirstStoryPoint, LastStoryPoint, LeadTime, CycleTime, e chaves estrangeiras para Type, Status, Project, Person).
- **`Detail`:** Detalhes do changelog (Id, Created, Type, From, To, IssueKey (FK), AuthorId (FK)).

### 4.2. Acesso a Dados

O acesso ao banco de dados é realizado pelos repositórios na camada `LoadJira.Infra`. O Dapper é empregado como Micro-ORM para executar comandos SQL e mapear resultados para objetos C#. As operações de banco de dados são encapsuladas nos métodos dos repositórios, que utilizam conexões gerenciadas pela `BaseRepository`.

## 5. Processos e Transformações

A aplicação LoadJira executa diferentes processos com base no comando fornecido na linha de comando (`issue`, `sp`, `time`).

### 5.1. Processo `issue`

Responsável pela extração e carga inicial dos dados das issues do Jira:

1.  **Carga de Chaves de Issues:** Opcionalmente, busca novas chaves de issues na API do Jira (`JiraWebApiService.GetKeysAsync`) até uma data final especificada.
2.  **Persistência Inicial:** As novas chaves de issues são salvas no banco de dados local (`IssueRepository.Save(issuesKey)`).
3.  **Seleção para Processamento:** Identifica issues no banco de dados local que ainda não foram totalmente processadas.
4.  **Processamento Individual:** Para cada issue não processada:
    - Obtém detalhes completos da issue e seu changelog da API do Jira (`JiraWebApiService.GetIssueAsync`, `GetDetailsAsync`).
    - Salva ou atualiza entidades relacionadas (Tipo, Projeto, Status, Reportador) e os detalhes do changelog no banco de dados.
    - Marca a issue como processada no banco de dados após o sucesso.

### 5.2. Processo `sp` (Story Points)

Calcula e persiste os Story Points das issues:

1.  **Seleção de Issues:** Busca issues não processadas para Story Points no banco de dados.
2.  **Obtenção de Detalhes:** Recupera o changelog da issue do banco de dados local.
3.  **Cálculo:** Analisa o changelog para identificar o primeiro e o último valor de Story Points.
4.  **Persistência:** Atualiza a issue no banco de dados com os Story Points calculados e a marca como processada para SP.

### 5.3. Processo `time` (Lead Time e Cycle Time)

Calcula e persiste o Lead Time e o Cycle Time das issues:

1.  **Seleção de Issues:** Busca issues não processadas para tempo no banco de dados.
2.  **Obtenção de Detalhes:** Recupera o changelog da issue do banco de dados local.
3.  **Cálculo de Lead Time:** Diferença em dias entre a data de criação da issue e a data da última transição para um status "Done" ou "Concluído".
4.  **Cálculo de Cycle Time:** Diferença em dias entre a primeira transição para um status "In Progress" (ou similar) e a última transição para "Done" ou "Concluído".
5.  **Persistência:** Atualiza a issue no banco de dados com os tempos calculados e a marca como processada para tempo.

## 6. Configuração da Aplicação

O arquivo `LoadJira.Config/Config.cs` é crucial para o funcionamento da aplicação e **deve ser revisado e atualizado antes da execução**. Ele armazena configurações sensíveis e globais.

### 6.1. Parâmetros Essenciais

É necessário ajustar os seguintes valores:

-   **`_jiraUser`:** Nome de usuário da conta Jira (geralmente e-mail).
-   **`_jiraToken`:** Token da API para autenticação com o Jira Cloud.
-   **`_jiraUrl`:** URL base da instância do Jira Cloud (ex.: `https://seu-dominio.atlassian.net`).
-   **`_sqlServerConn`:** String de conexão com o SQL Server.

**Exemplo de Configuração (`Config.cs`):**

```csharp
private static string _jiraUser => "seu-email@exemplo.com";
private static string _jiraToken => "seu-token-da-api-jira";
private static string _jiraUrl => "https://seu-dominio.atlassian.net";
private static string _sqlServerConn => @"Server=seu-servidor-sql;Database=LoadJira;User Id=seu-usuario;Password=sua-senha;";
```

### 6.2. Observações Importantes sobre a Configuração

-   **Atualização Obrigatória:** Os valores padrão são placeholders e **devem ser substituídos** pelos seus dados reais.
-   **Segurança:** Em ambientes de produção, considere carregar credenciais de variáveis de ambiente ou cofres de segredos para maior segurança.

## 7. Criação do Schema e Objetos de Banco de Dados

Antes de executar a aplicação, o banco de dados deve ser criado e configurado. Scripts SQL para definir o schema e os objetos necessários estão disponíveis em `LoadJira.Infra/Repository/sql/`.

### 7.1. Ordem de Execução dos Scripts

Os scripts devem ser executados na seguinte sequência:

1.  `01_type_create_table.sql`
2.  `02_project_create_table.sql`
3.  `03_status_create_table.sql`
4.  `04_person_create_table.sql`
5.  `05_issue_create_table.sql`
6.  `06_detail_create_table.sql`
7.  `07_issue_alter_table_sp.sql`
8.  `08_issue_alter_table_time.sql`

### 7.2. Observações Importantes sobre o Banco de Dados

-   **Banco de Dados:** Os scripts assumem o uso do banco `jira_database`. Certifique-se de que este banco exista no seu servidor SQL Server.
-   **Integridade:** As tabelas são criadas com chaves primárias e estrangeiras para garantir a integridade referencial.
-   **Execução:** Recomenda-se executar os scripts em uma ferramenta como SQL Server Management Studio (SSMS), verificando o sucesso de cada etapa.

---
[Voltar](README.md)