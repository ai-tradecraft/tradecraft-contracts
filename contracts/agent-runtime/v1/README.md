# Tradecraft Agent Runtime Contracts v1

This directory is the canonical, provider-neutral contract source for the
Tradecraft Agent Runtime.

## Boundaries

The schemas define two independent protocol surfaces:

1. **Orchestrator to Lamplighter Controller**
   (`controller-message.schema.json`)
2. **Lamplighter Controller to Agent Runtime Adapter**
   (`runtime-adapter-message.schema.json`)

They also define workflow, resolved-agent, and immutable-context documents used
by those protocols.

Provider-specific configuration is referenced as an opaque, versioned document.
It must not be added to the controller protocol.

## Layout

```text
contracts/agent-runtime/v1/
├── schemas/
│   ├── common.schema.json
│   ├── context-package.schema.json
│   ├── controller-message.schema.json
│   ├── resolved-agent-spec.schema.json
│   ├── runtime-adapter-message.schema.json
│   ├── runtime-resources.schema.json
│   └── workflow-definition.schema.json
├── examples/
└── validate_contracts.py
```

## Versioning

- All schemas use JSON Schema Draft 2020-12.
- Canonical IDs use
  `https://schemas.tradecraft.dev/agent-runtime/v1/<schema-name>`.
- `protocol_version` identifies wire semantics.
- `schema_version` or `payload_schema_version` identifies document shape.
- Additive optional fields require a compatible minor schema revision.
- Removing fields, changing required fields, changing meanings, or narrowing
  accepted values requires a new protocol major version.
- Extension fields belong under `extensions` and use namespaced keys.
- Unknown top-level fields are rejected.

## POC Adoption

The Lamplighter chat-through-harness POC uses the v1 controller messages
directly. Its controller HTTP surface uses `controller_id`, independent runtime,
session, invocation, and interaction IDs, and canonical command and event
kinds. The former `runner_id`, `/api/runner/*`, and dot-separated wire messages
were deleted rather than retained as migration aliases because the POC has no
deployed legacy controller clients.

Mappings remain appropriate only at actual adapter boundaries: controller
inventory from local runtime observations, normalized events from provider
output, and UI projections from orchestrator state.

## Snapshots, Checkpoints, and Publications

The contracts intentionally separate:

- `snapshot_request`: a request or agent suggestion to capture state;
- `snapshot_manifest`: immutable component references;
- `snapshot_descriptor`: capture purpose, consistency, restoration mode,
  watermark, and retention;
- `checkpoint`: an orchestrator-committed, gate-verified workflow boundary;
- `document_publication_request` and `document_publication_result`: explicit
  promotion of selected workspace content into durable versioned documents.

`RequestSnapshot` is a controller command. Checkpoint evaluation and commitment
are orchestrator operations and are not delegated to a controller or runtime
adapter.

The normative ownership and transfer rules are documented in
`.agent-docs/snapshot-checkpoint-and-publication-spec.md`.

## Validation

Run:

```sh
uv run --project submodules/lamplighter-opencode \
  uv run python contracts/agent-runtime/v1/validate_contracts.py
```

The validator checks:

- every schema is valid Draft 2020-12 JSON Schema;
- canonical schema IDs are unique;
- every example names a schema;
- every example validates against that schema.
