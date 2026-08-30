Contato do desenvolvedor :

Otávio Castelani
otavio.castelani@gmail.com



# Sabemi — Serviço de Webhooks de Pagamentos

Serviço que recebe notificações de pagamento de um banco parceiro via webhook, processa com
idempotência e resiliência, e exibe o status em um painel administrativo.

Desenvolvido como teste técnico para a vaga de Desenvolvedor(a) .NET — Sabemi Tec.

## Teste Rápido

### Opção 1 — Testar a demo já no ar (não precisa instalar nada)

- **Dashboard:** http://72.60.5.54:3000
- **API / Swagger:** http://72.60.5.54:5001/swagger

Pra ver um pagamento novo aparecer no dashboard, envie um webhook de teste:

```bash
curl -X POST http://72.60.5.54:5001/webhooks/pagamento \
  -H "Content-Type: application/json" \
  -H "X-Api-Key: sabemi-dev-super-secret-2026" \
  -d '{
    "id_transacao": "tx-avaliador-001",
    "id_contrato": "contrato-001",
    "valor": 150.00,
    "data_pagamento": "2026-08-30T10:00:00Z",
    "status": "sucesso"
  }'
```

Espera ~3 segundos (processamento em background) e atualiza o dashboard — o pagamento deve
aparecer na tabela.

**Ou pelo Postman:** importe [`postman/collection.json`](postman/collection.json) e troque a
variável `base_url` da collection para `http://72.60.5.54:5001`.

### Opção 2 — Rodar sua própria cópia localmente

Pré-requisito: [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado e aberto.

```bash
git clone https://github.com/Castelani325/Teste_tecnico_L2.git
cd Teste_tecnico_L2
docker-compose up -d --build
```

Aguarde o build (a primeira vez demora um pouco, baixa as imagens base). Depois:
- **Dashboard:** http://localhost:3000
- **API / Swagger:** http://localhost:5001/swagger

> **Se der erro de porta ocupada (5432):** provavelmente você já tem um PostgreSQL rodando
> localmente. Crie um arquivo `docker-compose.override.yml` na raiz do projeto com:
> ```yaml
> services:
>   db:
>     ports:
>       - "5433:5432"
> ```
> e ajuste a porta do banco em `src/Sabemi.WebhookApi/appsettings.Development.json` de acordo,
> ou pare o Postgres nativo temporariamente.

## Stack

- **Backend:** ASP.NET Core 8 (C#) + Entity Framework Core + Npgsql
- **Frontend:** React + TypeScript (Vite)
- **Banco:** PostgreSQL 16
- **Orquestração:** Docker Compose (backend + frontend + banco)
- **Documentação de API:** Swagger/OpenAPI + Postman Collection

## Exemplos de requisição

**Reenviar o mesmo `id_transacao`** (prova de idempotência): repete o `curl` da seção de
Teste Rápido — a resposta muda de `202 Accepted` para `200 OK` com uma mensagem informando
que o evento já foi recebido.

**Listar pagamentos com filtros:**
```bash
curl "http://localhost:5001/pagamentos?status=sucesso&id_contrato=contrato-001"
```

## Postman Collection

A collection completa está em [`postman/collection.json`](postman/collection.json), com:
- Envio de webhook novo
- Reenvio do mesmo `id_transacao` (prova de idempotência)
- Listagem com filtros por `status` e `id_contrato`

Importe pelo Postman via **File → Import**. A variável `base_url` já vem apontando para
`localhost:5001` — troque para `http://72.60.5.54:5001` pra testar contra a VPS.

## Decisões de arquitetura

- **Log imutável + projeção de estado:** cada webhook recebido é gravado em `eventos_brutos`
  (tabela *append-only*, nunca alterada), e um `BackgroundService` assíncrono atualiza
  `status_contrato` (o estado atual do contrato) a partir desse log. Essa separação é uma
  versão simplificada de *event sourcing* — o log é a fonte da verdade, o status é uma
  projeção derivada dele.
- **Idempotência garantida no banco, não só no código:** `id_transacao` tem constraint
  `UNIQUE` na tabela `eventos_brutos`. A API verifica duplicidade de forma otimista antes
  do insert, e também trata a exceção de violação de constraint como rede de segurança
  contra requisições concorrentes — a garantia real vem do banco.
- **Resiliência via fila em memória:** o endpoint `POST /webhooks/pagamento` grava o evento
  e responde `202 Accepted` imediatamente; o processamento "pesado" (simulado em 2s) ocorre
  de forma assíncrona em um `BackgroundService`, consumindo uma fila (`System.Threading.Channels`).
- **DTOs desacoplados das entidades:** as respostas da API (`PagamentoResponse`) são objetos
  próprios, não as entidades do Entity Framework — evita acoplar o contrato HTTP ao schema
  do banco.
- **Configuração via variáveis de ambiente:** segredos e endereços (senha do banco, ApiKey,
  URL da API, origem CORS) são parametrizados via `${VAR:-padrão}` no `docker-compose.yml`,
  permitindo que o mesmo arquivo funcione local e na VPS sem alterações — só um `.env`
  diferente em cada ambiente.

## Estrutura do repositório

```
├── db/init.sql                      # schema do banco (tabelas + constraints)
├── src/Sabemi.WebhookApi/           # backend .NET
├── frontend/                        # frontend React
├── postman/collection.json          # collection de testes da API
├── docker-compose.yml               # orquestração dos 3 serviços
└── deploy.sh                        # script de deploy/atualização na VPS
```


***Esse README foi feito com auxílio de IA, visando o melhor direcionamento, tanto para teste rápido quanto para o import e deploy de um container local (Esse aviso não foi feito por IA)***
