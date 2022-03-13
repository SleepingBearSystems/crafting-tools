﻿using System.Collections.Immutable;
using CraftingTools.Shared;

namespace CraftingTools.Domain;

public sealed class RecipeInput : ValueObject<RecipeInput>
{
    private RecipeInput(Item item, int count)
    {
        this.Item = item;
        this.Count = count;
    }

    /// <inheritdoc cref="ValueObject{T}"/>
    protected override bool EqualsCore(RecipeInput other)
    {
        return this.Item == other.Item && this.Count == other.Count;
    }

    /// <inheritdoc cref="ValueObject{T}"/>
    protected override int GetHashCodeCore()
    {
        return (this.Item, this.Count).GetHashCode();
    }

    /// <summary>
    /// The item being used.
    /// </summary>
    public Item Item { get; }

    /// <summary>
    /// The number of items being used.
    /// </summary>
    public int Count { get; }

    public static readonly RecipeInput None = new(Item.None, count: 0);

    /// <summary>
    /// Factory method for creating a <see cref="RecipeInput"/> from the
    /// supplied parameters.
    /// </summary>
    public static RailwayResult<RecipeInput> FromParameters(Item item, int count, string? resultId = default)
    {
        var failures = ImmutableList<RailwayResultBase>.Empty;

        var validItem = item
            .ToResultIsNotNull(failureMessage: "Item cannot be null.", nameof(item))
            .Check(value => !object.ReferenceEquals(value, Item.None), failureMessage: "Item cannot be none.")
            .UnwrapOrAddToFailuresImmutable(ref failures);

        var validCount = count
            .ToResult(nameof(count))
            .Check(value => value > 0, failureMessage: "Range must be positive.")
            .UnwrapOrAddToFailuresImmutable(ref failures);

        return failures.IsEmpty
            ? RailwayResult<RecipeInput>.Success(new RecipeInput(validItem, validCount), resultId)
            : RailwayResult<RecipeInput>.Failure(failures.ToError("Unable to create recipe output."), resultId);
    }
}