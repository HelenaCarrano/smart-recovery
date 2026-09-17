# Smart Recovery

Sistema de **recuperação inteligente de pagamentos recorrentes**, desenvolvido com .NET 8, React e PostgreSQL.

O sistema simula cobranças, identifica pagamentos recusados, calcula um **Recovery Score** e define automaticamente a melhor estratégia de recuperação com base no histórico do cliente.

## Demo

**Frontend:** https://smart-recovery-8b538.web.app (primeira vez rodando pode demorar)
**API / Swagger:** https://smart-recovery-api.onrender.com

Todos os dados e pagamentos são simulados.

## Stack

**Backend**

* C# / .NET 8
* ASP.NET Core
* Entity Framework Core
* PostgreSQL
* Serilog

**Frontend**

* React
* TypeScript
* Vite
* Tailwind CSS
* Recharts

**Testes**

* xUnit
* Moq
* Testes de integração com PostgreSQL

## Arquitetura

O projeto utiliza **Clean Architecture**, separando regras de negócio, aplicação, infraestrutura e API.

```text
Domain
  ↓
Application
  ↓
Infrastructure
  ↓
API
```

O frontend se comunica exclusivamente com a API, que concentra as regras de negócio.

## Como funciona

```text
Pagamento
    ↓
Processamento
    ↓
Aprovado → Ciclo encerrado

Recusado
    ↓
Recovery Score
    ↓
Estratégia de recuperação
    ↓
Nova tentativa / Atualização do pagamento /
Revisão manual / Cancelamento
```

O score considera:

* motivo da recusa;
* histórico de pagamentos;
* recuperações anteriores;
* recusas recentes;
* quantidade de tentativas.

As decisões são determinísticas e explicáveis.

## Principais funcionalidades

* Processamento de pagamentos
* Recuperação automática de pagamentos recusados
* Recovery Score de 0–100
* Agendamento de novas tentativas
* Background Worker
* Webhooks com idempotência
* Dashboard com métricas
* Histórico de pagamentos e recuperações
* API protegida por API Key

## Testes

O projeto possui **58 testes unitários e 9 testes de integração**, cobrindo regras de negócio, serviços, processamento de pagamentos e webhooks.

```bash
dotnet test
```

## Executar localmente

### Pré-requisitos

* .NET 8
* PostgreSQL
* Node.js

### Backend

```bash
dotnet run --project src/SmartRecovery.API
```

### Frontend

```bash
cd frontend
npm install
npm run dev
```

## Estrutura

```text
SmartRecovery/
├── src/
│   ├── SmartRecovery.Domain/
│   ├── SmartRecovery.Application/
│   ├── SmartRecovery.Infrastructure/
│   └── SmartRecovery.API/
│
├── tests/
│   └── SmartRecovery.Tests/
│
└── frontend/
```

## Limitações

* Pagamentos são simulados.
* Autenticação utiliza API Key para fins demonstrativos.
* Dados são fictícios.

## Status

**Projeto de portfólio — versão funcional.**

