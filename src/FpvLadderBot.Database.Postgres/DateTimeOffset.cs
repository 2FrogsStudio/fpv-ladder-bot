using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FpvLadderBot;

/// <summary>
/// a value converter that ensures you can run TMDS (integration tests) in timezones with UTC offset != 0
/// </summary>
/// </summary>
/// <remarks>
/// Copied from https://github.com/npgsql/npgsql/issues/4176#issuecomment-1064313552
/// </remarks>
internal class DateTimeOffsetConverter() : ValueConverter<DateTimeOffset, DateTimeOffset>(d => d.ToUniversalTime(),
    d => d.ToUniversalTime());

/// <summary>
/// <see cref="DateTimeOffsetConverter"/> but for nullable
/// </summary>
internal class NullableDateTimeOffsetConverter() : ValueConverter<DateTimeOffset?, DateTimeOffset?>(
    d => d == null ? null : d.Value.ToUniversalTime(),
    d => d == null ? null : d.Value.ToUniversalTime());
