# Roadmap de Branches — Teste Técnico .NET (Sabemi)

> Regra: nenhuma branch é apagada. Cada uma nasce de `main`, é commitada, 
> faz merge em `main` (via PR, se quiser mostrar fluxo profissional) e permanece 
> viva no histórico como prova de trabalho incremental.

## Ordem de execução

### Dia 1 — Backend + Banco

1. **`main`**
Branch raiz. Criar `README.md` inicial (mesmo que só com o título do projeto)
e primeiro commit.
2. **`feat/database-setup`**

   * Docker Compose com PostgreSQL
   * Modelagem das tabelas `EventosBrutos` e `StatusContrato`
   * Merge em `main` ao final
3. **`feat/webhook-endpoint`**

   * Branch a partir de `main` (já com o banco mergeado)
   * `POST /webhooks/pagamento`
   * Validação de ApiKey/Signature no header
   * Idempotência via constraint UNIQUE em `id\_transacao`
   * Merge em `main`
4. **`feat/background-worker`**

   * Branch a partir de `main`
   * `BackgroundService` simulando processamento de 2s
   * Atualização de `StatusContrato`
   * Merge em `main`
5. **`feat/payments-api`**

   * Branch a partir de `main`
   * `GET` de listagem com filtros (status, id\_contrato)
   * Merge em `main`

### Dia 2 — Frontend + Deploy

6. **`feat/frontend-dashboard`**

   * Branch a partir de `main`
   * Tela React: tabela de pagamentos + filtros
   * Merge em `main`
7. **`feat/frontend-error-alert`**

   * Branch a partir de `main`
   * Alerta visual para eventos com status de erro
   * Merge em `main`
8. **`feat/docker-compose-full`**

   * Branch a partir de `main`
   * Dockerfile do backend, Dockerfile do frontend, `docker-compose.yml` unificado
(API + Frontend + Postgres)
   * Testar `docker-compose up` do zero, em pasta limpa
   * Merge em `main`
9. **`chore/deploy-vps`**

   * Branch a partir de `main`
   * Ajustes de ambiente para produção: variáveis de ambiente (`.env`),
porta exposta, configuração de proxy reverso (Nginx, se a VPS já tiver),
scripts de deploy (`deploy.sh` ou instruções manuais)
   * Deploy efetivo na VPS
   * Merge em `main`
10. **`docs/postman-collection`**

    * Branch a partir de `main` (depois que `feat/webhook-endpoint` e
`feat/payments-api` já estiverem mergeados, pra documentar endpoints reais)
    * Exportar a collection do Postman (`.json`) com:

      * `POST /webhooks/pagamento` (exemplo de payload válido + header de ApiKey)
      * Exemplo de reenvio do mesmo `id\_transacao` (pra provar idempotência)
      * `GET` de listagem com filtros (status, id\_contrato)
    * Salvar o arquivo `.json` na raiz do repo (ex: `postman/collection.json`)
ou usar o link público de "Run in Postman"
    * Merge em `main`
11. **`docs/readme`**

    * Branch a partir de `main`
    * README final: descrição do projeto, como rodar localmente (`docker-compose up`),
link/IP de acesso ao demonstrativo na VPS, exemplos de request (curl/JSON),
link ou referência pra collection do Postman, decisões de arquitetura
    * Merge em `main`

## Resumo visual

```
main
 ├── feat/database-setup
 ├── feat/webhook-endpoint
 ├── feat/background-worker
 ├── feat/payments-api
 ├── feat/frontend-dashboard
 ├── feat/frontend-error-alert
 ├── feat/docker-compose-full
 ├── chore/deploy-vps
 ├── docs/postman-collection
 └── docs/readme
```

## Dica sobre a VPS

Pensa na VPS como "a prateleira da loja" — o Docker Compose é a caixa organizada
que você leva pra lá. Se o `docker-compose up` já funciona local, o deploy na VPS
é basicamente: subir o Docker na VPS (se ainda não tiver), copiar o projeto
(`git clone` ou `scp`), rodar `docker-compose up -d`, e abrir a porta no firewall.
Não precisa de nada além disso pra um teste técnico.

