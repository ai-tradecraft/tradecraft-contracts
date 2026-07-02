# Tradecraft Contracts

Shared cross-system contracts for the Tradecraft/Lamplighter runtime ecosystem.

This repository is the canonical home for contracts that are consumed by more
than one subsystem. Implementations may generate client libraries or load
schemas from here, but should not keep independent copies of these contracts in
their own repositories.

## Layout

- `contracts/agent-runtime/v1/` — normative JSON Schemas, examples, and
  validation tooling for the orchestrator/controller and controller/adapter
  protocol surfaces.
- `contracts/lamplighter-opencode/schemas/` — OpenCode model-accessor payload
  schemas used by the Lamplighter OpenCode adapter.
- `src/Tradecraft.Contracts/` — C# DTOs and JSON options for the v1
  orchestrator/controller protocol.
- `tests/Tradecraft.Contracts.Tests/` — compatibility tests for the C# protocol
  surface.

## Validate

```sh
uv run python contracts/agent-runtime/v1/validate_contracts.py
dotnet test Tradecraft.Contracts.slnx
```
