# Smart Recovery

Smart Recovery é uma plataforma de recuperação de pagamentos recorrentes recusados. O sistema analisa
o histórico de cobranças do cliente, calcula um score de recuperabilidade e recomenda automaticamente
a estratégia de recuperação mais adequada (retry, atualização de forma de pagamento, revisão manual ou
cancelamento da assinatura).

**Stack:** C# · .NET 8 · PostgreSQL · Entity Framework Core · React · TypeScript

- 🔗 **Demo:** [smart-recovery-8b538.web.app](https://smart-recovery-8b538.web.app)
- ⚙️ **API:** [smart-recovery-api.onrender.com](https://smart-recovery-api.onrender.com) (Swagger em `/`)

Dados simulados e sem gateway de pagamento real — ver [Limitações conhecidas](#limitações-conhecidas).

## Objetivo

O projeto foi desenvolvido para demonstrar conceitos de backend que vão além de operações CRUD:

- modelagem de regras de negócio explícitas e testáveis;
- Clean Architecture, com o Domain isolado de infraestrutura;
- processamento assíncrono com background workers;
- integração com banco relacional (EF Core + PostgreSQL);
- idempotência de webhooks;
- testes unitários e de integração;
- decisões automatizadas e explicáveis (o Recovery Score nunca é uma caixa-preta).

## Arquitetura

Clean Architecture em 4 camadas, com o Domain isolado de qualquer dependência de infraestrutura:

```
SmartRecovery.Domain          regras de negócio puras (RecoveryScoreCalculator, RecoveryDecisionEngine,
                               BillingCycleCalculator, RecoveryActionScheduler) + entidades + interfaces
        ↑
SmartRecovery.Application     casos de uso (Services), DTOs, orquestra Domain + Infrastructure
        ↑
SmartRecovery.Infrastructure  EF Core + PostgreSQL, repositórios, seed, background workers
        ↑
SmartRecovery.API             Controllers, Swagger, tratamento de erro global, logging
```

O React consome a API somente por HTTP — nenhuma regra de negócio (cálculo de score, decisão de ação) é
duplicada no frontend. O backend é a única fonte da verdade.

## Tecnologias

**Backend:** .NET 8, ASP.NET Core Web API, Entity Framework Core 8 + Npgsql, PostgreSQL, Serilog, xUnit + Moq, Respawn (testes de integração).

**Frontend:** React 19, TypeScript, Vite, Tailwind CSS v4, React Router, Axios, Recharts.

## Funcionalidades

- **Dashboard**: KPIs (recovery rate, receita recuperada, ações pendentes), distribuição de pagamentos, tendência diária de aprovados/recusados, motivos de recusa mais comuns.
- **Pagamentos**: listagem paginada com filtros (status, motivo, cliente, período), detalhe com linha do tempo completa e breakdown do Recovery Score.
- **Recuperação**: fila de oportunidades pendentes ordenável por impacto financeiro, score ou ação, com resumo de receita em risco.
- **Clientes**: listagem paginada com busca, detalhe com histórico de pagamentos (Recovery Score e ação inline para os que já foram recusados).
- **Assinaturas**: listagem paginada filtrável por status, com próxima data de cobrança.
- **Webhooks**: endpoint idempotente para receber resultado de cobranças de um provedor externo (simulado).

## Fluxo de pagamento

```
Assinatura vence
      ↓
PaymentRetryWorker cria Payment (Pending)
      ↓
PaymentGatewaySimulator processa a cobrança (75% aprovação / 25% recusa)
      ↓
   Aprovado ──────────────────────────────► fim do ciclo
      │
   Recusado
      ↓
RecoveryService.AnalyzeAsync (RecoveryScoreCalculator + RecoveryDecisionEngine)
      ↓
Ação recomendada: RetryIn2/24/72Hours | RequestPaymentMethodUpdate | SendPaymentReminderEmail |
                  ManualReview | CancelSubscription
      ↓
Retry agendado?  → PaymentRetryWorker retenta automaticamente na data marcada
```

O mesmo pipeline de negócio é utilizado pela API, pelo seed e pelos testes de integração, sem uma
implementação de regras exclusiva para demonstração.

## Recovery Score

`RecoveryScoreCalculator.CalculateBreakdown` (`SmartRecovery.Domain/BusinessRules/`) soma cinco fatores nomeados, cada um com o sinal já aplicado:

| Fator | O que mede |
|---|---|
| Base score | Perfil de recuperabilidade do motivo da recusa (ex.: erro temporário recupera bem; cartão bloqueado, mal) |
| Ajuste de histórico | Taxa de sucesso histórica do cliente |
| Bônus de track record | Quantas vezes o cliente já foi recuperado antes |
| Ajuste de recusas recentes | Recusas nos últimos 30 dias |
| Ajuste de tentativas recentes | Volume de tentativas recentes (fadiga de retry) |

`RecoveryDecisionEngine.Decide(score, motivo, tentativas)` traduz o score final em uma ação, seguindo esta tabela (`SmartRecovery.Domain/BusinessRules/RecoveryDecisionEngine.cs`):

| Score | Ação |
|---|---|
| 80–100 | Retry em 2h (erro temporário / emissor indisponível) ou em 24h (demais motivos) |
| 50–79 | Retry em 72h |
| 30–49 | Solicitar atualização da forma de pagamento |
| 0–29 | Cancelar assinatura |

Duas exceções têm prioridade sobre a tabela de score: **cartão bloqueado ou suspeita de fraude** vão
sempre para revisão manual, independente do score. E, na **3ª tentativa consecutiva**, se o score
estiver abaixo de 80, o sistema interrompe novos retries automáticos e envia um e-mail de regularização
ao cliente em vez de continuar retentando às cegas.

A API expõe o breakdown completo (`GET /api/recovery/payments/{id}`) — o frontend só renderiza os números que o backend calculou.

## API

Principais endpoints (Swagger disponível em `/` ao rodar em Development):

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/payments` | Lista paginada, filtrável por status/motivo/cliente/período |
| GET | `/api/payments/{id}` | Detalhe + `/attempts` para o histórico de tentativas |
| POST | `/api/payments/subscriptions/{id}` | Cria cobrança pendente para uma assinatura |
| POST | `/api/payments/{id}/process` | Processa via gateway; aciona recuperação se recusado |
| GET | `/api/customers` | Lista paginada com busca por nome/email |
| GET | `/api/subscriptions` | Lista paginada filtrável por status |
| GET | `/api/recovery/payments/{id}` | Análise de recuperação de um pagamento |
| GET | `/api/recovery/pending` | Ações de recuperação ainda não executadas |
| GET | `/api/dashboard/summary` | KPIs do Dashboard |
| GET | `/api/dashboard/decline-reasons` | Distribuição dos motivos de recusa |
| GET | `/api/dashboard/trends` | Tendência diária de pagamentos |
| POST | `/api/webhooks/payments` | Recebe resultado de cobrança externa (idempotente) |

Erros seguem `ProblemDetails` (RFC 7807) em toda a API.

## Banco de dados

PostgreSQL, sem dados fabricados à mão: o seed (`SmartRecoveryDataSeeder`) simula ~9 meses de cobrança
para 80 clientes, reaproveitando o mesmo pipeline de produção (`BillingCycleCalculator`,
`RecoveryScoreCalculator`, `RecoveryDecisionEngine`), com uma seed fixa (`Random(42)`) para o resultado
ser reprodutível. Os motivos de recusa seguem uma distribuição realista (`DeclineReasonSimulator`) —
`InsufficientFunds`/`ExpiredCard` dominam, `SuspectedFraud` é raro — em vez de uniforme entre os 10
valores de `DeclineReason`, e todas as 7 `RecoveryAction` aparecem no histórico de análises.

## Testes

- **58 testes unitários** (`tests/SmartRecovery.Tests/Domain`, `/Application`) cobrindo as regras de negócio e todos os Services de Application isoladamente (Moq nos repositórios).
- **9 testes de integração** (`tests/SmartRecovery.Tests/Integration`) rodando HTTP → Controller → Application → EF Core → PostgreSQL de verdade, contra um banco separado (`smart_recovery_test`, criado e migrado automaticamente): criar pagamento, processar pagamento (aprovado/recusado, com verificação de que a análise de recuperação é gerada), consultar recuperação, webhook válido, duplicado (idempotência) e inválido (validação).

```bash
dotnet test
```

## Como executar

Requer PostgreSQL rodando localmente (este projeto foi desenvolvido sem Docker).

```bash
# 1. Criar o banco e o usuário (uma vez) — dono só do próprio banco, sem SUPERUSER
psql -U postgres -c "CREATE USER smart_recovery WITH PASSWORD 'smart_recovery';"
psql -U postgres -c "CREATE DATABASE smart_recovery OWNER smart_recovery;"

# 2. Backend — aplica migrations e popula o seed automaticamente no primeiro start
dotnet run --project src/SmartRecovery.API

# 3. Frontend
cd frontend
npm install
npm run dev
```

As credenciais acima são exclusivamente para desenvolvimento local e não devem ser utilizadas em
ambientes reais. Essa connection string é a usada em desenvolvimento (`appsettings.Development.json`);
em produção ela vem de variável de ambiente (`ConnectionStrings__DefaultConnection` ou `DATABASE_URL`),
nunca hardcoded.

API em `http://localhost:5000` (Swagger em `/`), frontend em `http://localhost:5173`.

## Limitações conhecidas

- **Sem autenticação de usuário** — existe uma API key simples (`X-Api-Key`) protegendo a API, mais para demonstrar a preocupação com segurança do que como controle de acesso real (a chave, quando configurada, vai embutida no bundle do frontend). Um próximo passo natural seria Login → JWT → Authorization → API.
- **Gateway de pagamento simulado**: `IPaymentGatewaySimulator` já isola a interface da implementação (`PaymentGatewaySimulator`), pronta para receber uma implementação real (ex.: modo sandbox de um provedor) sem alterar o resto do sistema.
