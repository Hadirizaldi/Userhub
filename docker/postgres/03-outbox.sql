CREATE TABLE outbox_messages (
    id           uuid PRIMARY KEY,
    type         text NOT NULL,
    payload      jsonb NOT NULL,
    created_at   timestamptz NOT NULL DEFAULT now(),
    processed_at timestamptz NULL,
    attempts     int NOT NULL DEFAULT 0,
    error        text NULL
);
CREATE INDEX ix_outbox_unprocessed ON outbox_messages (created_at) WHERE processed_at IS NULL;

CREATE TABLE processed_messages (
    id           uuid PRIMARY KEY,          -- = MessageId yang dibawa dari outbox
    processed_at timestamptz NOT NULL DEFAULT now()
);