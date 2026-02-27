using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;

namespace Orbita.Infrastructure.Persistence.Configurations.Converters;

public static class IdConverters
{
    public static ValueConverter<TId, Guid> GuidId<TId>(
        Expression<Func<TId, Guid>> toProvider,
        Expression<Func<Guid, TId>> fromProvider)
        where TId : notnull
        => new(toProvider, fromProvider);

    public static ValueConverter<TId?, Guid?> NullableGuidId<TId>(
        Expression<Func<TId?, Guid?>> toProvider,
        Expression<Func<Guid?, TId?>> fromProvider)
        where TId : class
        => new(toProvider, fromProvider);

    public static readonly ValueConverter<DateOnly, DateTime> DateOnlyToDateTime =
        new(
            d => d.ToDateTime(TimeOnly.MinValue),
            dt => DateOnly.FromDateTime(dt));

    public static readonly ValueConverter<DateOnly?, DateTime?> NullableDateOnlyToDateTime =
        new(
            d => d.HasValue ? d.Value.ToDateTime(TimeOnly.MinValue) : null,
            dt => dt.HasValue ? DateOnly.FromDateTime(dt.Value) : null);

    public static readonly ValueConverter<TimeOnly, TimeSpan> TimeOnlyToTimeSpan =
        new(
            t => t.ToTimeSpan(),
            ts => TimeOnly.FromTimeSpan(ts));

    public static readonly ValueConverter<TimeOnly?, TimeSpan?> NullableTimeOnlyToTimeSpan =
        new(
            t => t.HasValue ? t.Value.ToTimeSpan() : null,
            ts => ts.HasValue ? TimeOnly.FromTimeSpan(ts.Value) : null);
}
