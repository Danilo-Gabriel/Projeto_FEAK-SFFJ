CREATE EXTENSION IF NOT EXISTS pgcrypto;

CREATE SCHEMA IF NOT EXISTS feak_sf;

SET search_path TO feak_sf;

CREATE TABLE IF NOT EXISTS produtos (
    id uuid NOT NULL DEFAULT gen_random_uuid(),
    codigo_barras text NOT NULL,
    descricao text NOT NULL,
    preco_custo numeric(18,2) NOT NULL DEFAULT 0,
    preco_venda numeric(18,2) NOT NULL DEFAULT 0,
    estoque_atual integer NOT NULL DEFAULT 0,
    dh_inclusao timestamp with time zone NOT NULL DEFAULT now(),
    dh_exclusao timestamp with time zone NULL,
    CONSTRAINT "PK_produtos" PRIMARY KEY (id)
);

ALTER TABLE produtos
    ADD COLUMN IF NOT EXISTS codigo_barras text;

ALTER TABLE produtos
    ALTER COLUMN codigo_barras SET DEFAULT '';

UPDATE produtos
SET codigo_barras = 'PDV-' || SUBSTRING(REPLACE(CAST(id AS text), '-', ''), 1, 10)
WHERE codigo_barras IS NULL OR codigo_barras = '';

ALTER TABLE produtos
    ALTER COLUMN codigo_barras SET NOT NULL;

CREATE INDEX IF NOT EXISTS "IX_produtos_descricao"
    ON produtos (descricao);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_produtos_codigo_barras"
    ON produtos (codigo_barras);

CREATE TABLE IF NOT EXISTS vendas (
    id uuid NOT NULL DEFAULT gen_random_uuid(),
    numero_venda text NOT NULL,
    operador text NOT NULL DEFAULT '',
    consumidor text NOT NULL,
    forma_pagamento text NOT NULL,
    subtotal numeric(18,2) NOT NULL DEFAULT 0,
    desconto_total numeric(18,2) NOT NULL DEFAULT 0,
    acrescimo numeric(18,2) NOT NULL DEFAULT 0,
    total numeric(18,2) NOT NULL DEFAULT 0,
    cancelada boolean NOT NULL DEFAULT false,
    dh_cancelamento timestamp with time zone NULL,
    dh_inclusao timestamp with time zone NOT NULL DEFAULT now(),
    dh_exclusao timestamp with time zone NULL,
    CONSTRAINT "PK_vendas" PRIMARY KEY (id)
);

ALTER TABLE vendas
    ADD COLUMN IF NOT EXISTS operador text NOT NULL DEFAULT '';

ALTER TABLE vendas
    ADD COLUMN IF NOT EXISTS cancelada boolean NOT NULL DEFAULT false;

ALTER TABLE vendas
    ADD COLUMN IF NOT EXISTS dh_cancelamento timestamp with time zone NULL;

CREATE UNIQUE INDEX IF NOT EXISTS "IX_vendas_numero_venda"
    ON vendas (numero_venda);

CREATE SEQUENCE IF NOT EXISTS numero_venda_seq;

SELECT setval(
    'feak_sf.numero_venda_seq',
    COALESCE((
        SELECT MAX(CAST(SUBSTRING(numero_venda FROM 3) AS bigint))
        FROM vendas
        WHERE numero_venda ~ '^VD[0-9]+$'
    ), 0) + 1,
    false
);

CREATE TABLE IF NOT EXISTS venda_itens (
    id uuid NOT NULL DEFAULT gen_random_uuid(),
    venda_id uuid NOT NULL,
    produto_id uuid NOT NULL,
    codigo_produto text NOT NULL,
    descricao_produto text NOT NULL,
    quantidade integer NOT NULL DEFAULT 1,
    preco_unitario numeric(18,2) NOT NULL DEFAULT 0,
    desconto_valor numeric(18,2) NOT NULL DEFAULT 0,
    desconto_percentual numeric(18,2) NOT NULL DEFAULT 0,
    total_item numeric(18,2) NOT NULL DEFAULT 0,
    dh_inclusao timestamp with time zone NOT NULL DEFAULT now(),
    dh_exclusao timestamp with time zone NULL,
    CONSTRAINT "PK_venda_itens" PRIMARY KEY (id),
    CONSTRAINT "FK_venda_itens_vendas" FOREIGN KEY (venda_id) REFERENCES vendas(id),
    CONSTRAINT "FK_venda_itens_produtos" FOREIGN KEY (produto_id) REFERENCES produtos(id)
);

INSERT INTO produtos (id, codigo_barras, descricao, preco_custo, preco_venda, estoque_atual, dh_inclusao, dh_exclusao)
VALUES
    ('9d1f52d7-5c99-4dd2-9a6c-2d31f3f65e01', '7891000100101', 'Arroz Tipo 1 5kg', 22.50, 31.90, 18, now(), NULL),
    ('9d1f52d7-5c99-4dd2-9a6c-2d31f3f65e02', '7891000100102', 'Feijao Carioca 1kg', 5.80, 8.90, 32, now(), NULL),
    ('9d1f52d7-5c99-4dd2-9a6c-2d31f3f65e03', '7891000100103', 'Oleo de Soja 900ml', 4.90, 7.50, 24, now(), NULL),
    ('9d1f52d7-5c99-4dd2-9a6c-2d31f3f65e04', '7891000100104', 'Cafe Torrado 500g', 11.50, 17.90, 14, now(), NULL),
    ('9d1f52d7-5c99-4dd2-9a6c-2d31f3f65e05', '7891000100105', 'Acucar Refinado 1kg', 3.20, 5.40, 26, now(), NULL),
    ('9d1f52d7-5c99-4dd2-9a6c-2d31f3f65e06', '7891000100106', 'Macarrao Espaguete 500g', 2.90, 4.99, 40, now(), NULL),
    ('9d1f52d7-5c99-4dd2-9a6c-2d31f3f65e07', '7891000100107', 'Sabonete 90g', 1.10, 2.49, 60, now(), NULL),
    ('9d1f52d7-5c99-4dd2-9a6c-2d31f3f65e08', '7891000100108', 'Detergente Neutro 500ml', 1.75, 3.29, 28, now(), NULL),
    ('9d1f52d7-5c99-4dd2-9a6c-2d31f3f65e09', '7891000100109', 'Leite Integral 1L', 3.95, 5.89, 36, now(), NULL),
    ('9d1f52d7-5c99-4dd2-9a6c-2d31f3f65e10', '7891000100110', 'Biscoito Recheado 120g', 1.65, 2.99, 48, now(), NULL),
    ('9d1f52d7-5c99-4dd2-9a6c-2d31f3f65e11', '7891000100111', 'Refrigerante Cola 2L', 5.10, 8.79, 21, now(), NULL),
    ('9d1f52d7-5c99-4dd2-9a6c-2d31f3f65e12', '7891000100112', 'Agua Mineral 500ml', 0.80, 1.99, 72, now(), NULL)
ON CONFLICT (id) DO NOTHING;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
SELECT '20260421000200_add_vendas_and_codigo_barras', '8.0.20'
WHERE NOT EXISTS (
    SELECT 1
    FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20260421000200_add_vendas_and_codigo_barras'
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
SELECT '20260421165849_add_venda_operador_cancelamento', '8.0.20'
WHERE NOT EXISTS (
    SELECT 1
    FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20260421165849_add_venda_operador_cancelamento'
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
SELECT '20260620091818_padronizar_schema_snake_case', '8.0.20'
WHERE NOT EXISTS (
    SELECT 1
    FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20260620091818_padronizar_schema_snake_case'
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
SELECT '20260620093232_adicionar_sequence_numero_venda', '8.0.20'
WHERE NOT EXISTS (
    SELECT 1
    FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20260620093232_adicionar_sequence_numero_venda'
);
