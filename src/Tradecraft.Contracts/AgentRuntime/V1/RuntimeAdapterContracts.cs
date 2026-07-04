using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tradecraft.Contracts.AgentRuntime.V1;

/// <summary>Requests replay of normalized adapter events from an inclusive sequence.</summary>
/// <param name="DocumentType">The resource document type.</param>
/// <param name="FromSequence">The inclusive sequence to replay from.</param>
/// <param name="MaxEvents">The optional maximum number of events to return.</param>
/// <param name="Extensions">The optional namespaced extension data.</param>
public sealed record AdapterEventReplayRequest(
    [property: JsonPropertyName("document_type")] string DocumentType,
    [property: JsonPropertyName("from_sequence")] long FromSequence,
    [property: JsonPropertyName("max_events")] int? MaxEvents = null,
    [property: JsonPropertyName("extensions")] ImmutableDictionary<string, JsonElement>? Extensions = null);

/// <summary>Describes one normalized event emitted by an agent runtime adapter.</summary>
/// <param name="MessageType">The protocol message type.</param>
/// <param name="ProtocolVersion">The protocol version.</param>
/// <param name="SchemaVersion">The message schema version.</param>
/// <param name="EventId">The stable event identifier.</param>
/// <param name="AdapterKind">The adapter kind.</param>
/// <param name="AdapterVersion">The adapter implementation version.</param>
/// <param name="EventType">The normalized adapter event kind.</param>
/// <param name="Aggregate">The ordered event aggregate.</param>
/// <param name="Sequence">The monotonic event sequence.</param>
/// <param name="OccurredAt">The event time.</param>
/// <param name="PayloadSchemaVersion">The payload schema version.</param>
/// <param name="Target">The related runtime resources.</param>
/// <param name="Correlation">The workflow and trace correlation.</param>
/// <param name="PayloadRef">The optional claim-check payload.</param>
/// <param name="Payload">The optional inline payload.</param>
/// <param name="RawEventRef">The optional restricted provider event.</param>
/// <param name="Extensions">The optional namespaced extension data.</param>
public sealed record AdapterEvent(
    [property: JsonPropertyName("message_type")] string MessageType,
    [property: JsonPropertyName("protocol_version")] string ProtocolVersion,
    [property: JsonPropertyName("schema_version")] string SchemaVersion,
    [property: JsonPropertyName("event_id")] string EventId,
    [property: JsonPropertyName("adapter_kind")] string AdapterKind,
    [property: JsonPropertyName("adapter_version")] string AdapterVersion,
    [property: JsonPropertyName("event_type")] string EventType,
    [property: JsonPropertyName("aggregate")] AggregateReference Aggregate,
    [property: JsonPropertyName("sequence")] long Sequence,
    [property: JsonPropertyName("occurred_at")] DateTimeOffset OccurredAt,
    [property: JsonPropertyName("payload_schema_version")] string PayloadSchemaVersion,
    [property: JsonPropertyName("target")] ResourceTarget Target,
    [property: JsonPropertyName("correlation")] ProtocolCorrelation Correlation,
    [property: JsonPropertyName("payload_ref")] ContentReference? PayloadRef = null,
    [property: JsonPropertyName("payload")] JsonElement? Payload = null,
    [property: JsonPropertyName("raw_event_ref")] ContentReference? RawEventRef = null,
    [property: JsonPropertyName("extensions")] ImmutableDictionary<string, JsonElement>? Extensions = null);

/// <summary>Returns a replayable page of normalized adapter events.</summary>
/// <param name="DocumentType">The resource document type.</param>
/// <param name="AdapterKind">The adapter kind.</param>
/// <param name="AdapterVersion">The adapter implementation version.</param>
/// <param name="FromSequence">The inclusive sequence requested.</param>
/// <param name="ThroughSequence">The highest sequence included, or zero when no events are included.</param>
/// <param name="NextSequence">The next sequence to request.</param>
/// <param name="Events">The immutable normalized adapter events.</param>
/// <param name="Exhausted">Whether the adapter has returned all events currently available.</param>
/// <param name="GeneratedAt">The batch generation time.</param>
/// <param name="Extensions">The optional namespaced extension data.</param>
public sealed record AdapterEventBatch(
    [property: JsonPropertyName("document_type")] string DocumentType,
    [property: JsonPropertyName("adapter_kind")] string AdapterKind,
    [property: JsonPropertyName("adapter_version")] string AdapterVersion,
    [property: JsonPropertyName("from_sequence")] long FromSequence,
    [property: JsonPropertyName("through_sequence")] long ThroughSequence,
    [property: JsonPropertyName("next_sequence")] long NextSequence,
    [property: JsonPropertyName("events")] ImmutableArray<AdapterEvent> Events,
    [property: JsonPropertyName("exhausted")] bool Exhausted,
    [property: JsonPropertyName("generated_at")] DateTimeOffset GeneratedAt,
    [property: JsonPropertyName("extensions")] ImmutableDictionary<string, JsonElement>? Extensions = null);
