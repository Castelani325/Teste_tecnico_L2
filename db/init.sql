-- Tabela 1: log imutável de eventos recebidos (append-only)
CREATE TABLE eventos_brutos (
    id              SERIAL PRIMARY KEY,
    id_transacao    VARCHAR(100) NOT NULL UNIQUE,   
    id_contrato     VARCHAR(100) NOT NULL,
    valor           NUMERIC(15,2) NOT NULL,
    data_pagamento  TIMESTAMP NOT NULL,
    status          VARCHAR(50) NOT NULL,
    payload_bruto   JSONB,                          -- corpo cru do webhook, útil pra auditoria
    recebido_em     TIMESTAMP NOT NULL DEFAULT NOW()
);

-- Índice auxiliar: buscas futuras por contrato vão ser comuns (GET com filtro)
CREATE INDEX idx_eventos_brutos_id_contrato ON eventos_brutos (id_contrato);

CREATE TABLE status_contrato (
    id_contrato          VARCHAR(100) PRIMARY KEY,
    status_atual         VARCHAR(50) NOT NULL,
    ultima_atualizacao   TIMESTAMP NOT NULL DEFAULT NOW(),
    ultimo_id_transacao  VARCHAR(100)
);
