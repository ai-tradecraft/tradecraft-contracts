#!/usr/bin/env python3
"""Validate Agent Runtime JSON Schemas and their representative examples."""

from __future__ import annotations

import json
import sys
from pathlib import Path
from typing import Any

from jsonschema import Draft202012Validator
from referencing import Registry, Resource

ROOT = Path(__file__).resolve().parent
SCHEMAS = ROOT / "schemas"
EXAMPLES = ROOT / "examples"
DRAFT_2020_12 = "https://json-schema.org/draft/2020-12/schema"


def load_json(path: Path) -> dict[str, Any]:
    with path.open(encoding="utf-8") as stream:
        value = json.load(stream)
    if not isinstance(value, dict):
        raise ValueError(f"{path} must contain a JSON object")
    return value


def iter_refs(value: Any) -> list[str]:
    refs: list[str] = []
    if isinstance(value, dict):
        ref = value.get("$ref")
        if isinstance(ref, str):
            refs.append(ref)
        for child in value.values():
            refs.extend(iter_refs(child))
    elif isinstance(value, list):
        for child in value:
            refs.extend(iter_refs(child))
    return refs


def main() -> int:
    failures: list[str] = []
    schemas: dict[str, dict[str, Any]] = {}
    schema_files: dict[str, Path] = {}

    for path in sorted(SCHEMAS.glob("*.schema.json")):
        try:
            schema = load_json(path)
            Draft202012Validator.check_schema(schema)
            schema_id = schema.get("$id")
            if not isinstance(schema_id, str) or not schema_id:
                raise ValueError("schema does not declare a non-empty $id")
            if schema_id in schemas:
                raise ValueError(f"duplicate schema $id: {schema_id}")
            schemas[schema_id] = schema
            schema_files[path.name] = path
        except Exception as exc:  # noqa: BLE001 - report every contract failure together
            failures.append(f"{path.relative_to(ROOT)}: {exc}")

    registry = Registry()
    for schema_id, schema in schemas.items():
        registry = registry.with_resource(schema_id, Resource.from_contents(schema))

    for schema_id, schema in schemas.items():
        resolver = registry.resolver(schema_id)
        for ref in iter_refs(schema):
            try:
                resolver.lookup(ref)
            except Exception as exc:  # noqa: BLE001
                failures.append(f"{schema_id}: unresolved $ref {ref}: {exc}")

    manifest_path = EXAMPLES / "manifest.json"
    try:
        manifest = load_json(manifest_path)
        entries = manifest["examples"]
        if not isinstance(entries, list):
            raise ValueError("examples must be an array")
    except Exception as exc:  # noqa: BLE001
        failures.append(f"{manifest_path.relative_to(ROOT)}: {exc}")
        entries = []

    listed_examples: set[str] = set()
    for entry in entries:
        if not isinstance(entry, dict):
            failures.append("examples/manifest.json: each entry must be an object")
            continue

        example_name = entry.get("file")
        schema_name = entry.get("schema")
        definition = entry.get("definition")
        expected_valid = entry.get("valid", True)
        if not isinstance(example_name, str) or not isinstance(schema_name, str):
            failures.append(
                "examples/manifest.json: each entry requires string file and schema"
            )
            continue
        if not isinstance(expected_valid, bool):
            failures.append(f"{example_name}: valid must be a boolean when present")
            continue
        if example_name in listed_examples:
            failures.append(f"examples/manifest.json: duplicate example {example_name}")
            continue
        listed_examples.add(example_name)

        example_path = EXAMPLES / example_name
        schema_path = schema_files.get(schema_name)
        if schema_path is None:
            failures.append(f"{example_name}: unknown schema {schema_name}")
            continue

        try:
            example = load_json(example_path)
            schema = load_json(schema_path)
            schema_id = schema["$id"]
            if definition is None:
                validation_schema = schema
            elif isinstance(definition, str) and definition:
                validation_schema = {
                    "$schema": DRAFT_2020_12,
                    "$id": f"https://schemas.tradecraft.dev/agent-runtime/v1/example-validation/{example_name}",
                    "$ref": f"{schema_id}#/$defs/{definition}",
                }
            else:
                raise ValueError("definition must be a non-empty string when present")

            validator = Draft202012Validator(
                validation_schema,
                registry=registry,
                format_checker=Draft202012Validator.FORMAT_CHECKER,
            )
            errors = sorted(
                validator.iter_errors(example),
                key=lambda error: [str(part) for part in error.absolute_path],
            )
            if expected_valid:
                for error in errors:
                    location = (
                        ".".join(str(part) for part in error.absolute_path) or "<root>"
                    )
                    failures.append(
                        f"examples/{example_name}:{location}: {error.message}"
                    )
            elif not errors:
                failures.append(
                    f"examples/{example_name}: expected validation failure but document was valid"
                )
        except Exception as exc:  # noqa: BLE001
            failures.append(f"examples/{example_name}: {exc}")

    unlisted_examples = {
        path.name for path in EXAMPLES.glob("*.json") if path.name != manifest_path.name
    } - listed_examples
    for example_name in sorted(unlisted_examples):
        failures.append(
            f"examples/{example_name}: example is not listed in manifest.json"
        )

    if failures:
        print("Agent Runtime contract validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print(f"Validated {len(schemas)} schemas and {len(entries)} examples.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
