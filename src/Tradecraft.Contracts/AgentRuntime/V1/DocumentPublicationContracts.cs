using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tradecraft.Contracts.AgentRuntime.V1;

/// <summary>Identifies the bounded source content for explicit document publication.</summary>
/// <param name="WorkspacePath">The source path relative to the authorized runtime workspace.</param>
/// <param name="ContentRef">The already materialized source content reference.</param>
public sealed record DocumentPublicationSource(
    [property: JsonPropertyName("workspace_path")] string? WorkspacePath = null,
    [property: JsonPropertyName("content_ref")] ContentReference? ContentRef = null);

/// <summary>Requests explicit publication of an agent-authored document.</summary>
/// <param name="DocumentType">The resource document type.</param>
/// <param name="PublicationId">The publication identifier.</param>
/// <param name="Source">The bounded source content.</param>
/// <param name="LogicalPath">The logical document path.</param>
/// <param name="Title">The document title.</param>
/// <param name="MediaType">The document media type.</param>
/// <param name="VersionIntent">The requested versioning intent.</param>
/// <param name="IdempotencyKey">The publication idempotency key.</param>
/// <param name="Target">The runtime target and provenance resources.</param>
/// <param name="Correlation">The workflow and trace correlation.</param>
/// <param name="ExpectedVersion">The optional expected semantic version.</param>
/// <param name="Extensions">The optional namespaced extension data.</param>
public sealed record DocumentPublicationRequest(
    [property: JsonPropertyName("document_type")] string DocumentType,
    [property: JsonPropertyName("publication_id")] string PublicationId,
    [property: JsonPropertyName("source")] DocumentPublicationSource Source,
    [property: JsonPropertyName("logical_path")] string LogicalPath,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("media_type")] string MediaType,
    [property: JsonPropertyName("version_intent")] string VersionIntent,
    [property: JsonPropertyName("idempotency_key")] string IdempotencyKey,
    [property: JsonPropertyName("target")] ResourceTarget Target,
    [property: JsonPropertyName("correlation")] ProtocolCorrelation Correlation,
    [property: JsonPropertyName("expected_version")] string? ExpectedVersion = null,
    [property: JsonPropertyName("extensions")] ImmutableDictionary<string, JsonElement>? Extensions = null);

/// <summary>Reports the content reference and logical references produced for a publication request.</summary>
/// <param name="DocumentType">The resource document type.</param>
/// <param name="PublicationId">The publication identifier.</param>
/// <param name="DocumentRef">The logical document reference.</param>
/// <param name="VersionRef">The immutable document-version reference.</param>
/// <param name="ContentRef">The immutable source content reference.</param>
/// <param name="Created">Whether the operation created a new logical document.</param>
/// <param name="PublishedAt">The publication time.</param>
/// <param name="Extensions">The optional namespaced extension data.</param>
public sealed record DocumentPublicationResult(
    [property: JsonPropertyName("document_type")] string DocumentType,
    [property: JsonPropertyName("publication_id")] string PublicationId,
    [property: JsonPropertyName("document_ref")] string DocumentRef,
    [property: JsonPropertyName("version_ref")] string VersionRef,
    [property: JsonPropertyName("content_ref")] ContentReference ContentRef,
    [property: JsonPropertyName("created")] bool Created,
    [property: JsonPropertyName("published_at")] DateTimeOffset PublishedAt,
    [property: JsonPropertyName("extensions")] ImmutableDictionary<string, JsonElement>? Extensions = null);
