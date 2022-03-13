﻿using System.Collections.Immutable;
using CraftingTools.Shared;

namespace CraftingTools.Domain;

/// <summary>
/// Extension methods for the <see cref="Recipe"/> class.
/// </summary>
public static class RecipeExtensions
{
    /// <summary>
    /// Checks if <see cref="Recipe"/> instance is not null and not the <see cref="Recipe.None"/> instance
    /// and wraps the instance in a <see cref="RailwayResult{TValue}"/>.
    /// </summary>
    public static RailwayResult<Recipe> ToValidResult(this Recipe recipe, string? resultId = default)
    {
        return recipe
            .ToResultIsNotNull(failureMessage: "Recipe cannot be null.", resultId ?? nameof(recipe))
            .Check(value => value != Recipe.None, failureMessage: "Recipe cannot be none.");
    }

    public static RailwayResult<Recipe> SetOutput(
        this Recipe recipe,
        RecipeOutput output,
        string? resultId = default)
    {
        var failures = ImmutableList<RailwayResultBase>.Empty;

        var validRecipe = recipe
            .ToValidResult(nameof(recipe))
            .UnwrapOrAddToFailuresImmutable(ref failures);

        var validOutput = output
            .ToValidResult(nameof(output))
            .UnwrapOrAddToFailuresImmutable(ref failures);

        return validRecipe.Output.Item == validOutput.Item && validRecipe.Output.Count == validOutput.Count
            ? RailwayResult<Recipe>.Success(validRecipe, resultId)
            : Recipe.FromParameters(validRecipe.Id, validRecipe.Profession, output, validRecipe.Inputs, resultId);
    }

    public static RailwayResult<Recipe> AddInput(
        this Recipe recipe,
        RecipeInput input,
        string? resultId = default)
    {
        var failures = ImmutableList<RailwayResultBase>.Empty;

        var validRecipe = recipe
            .ToValidResult(nameof(recipe))
            .UnwrapOrAddToFailuresImmutable(ref failures);

        var validInput = input
            .ToValidResult(nameof(input))
            .UnwrapOrAddToFailuresImmutable(ref failures);

        return Recipe.FromParameters(
            validRecipe.Id,
            validRecipe.Profession,
            validRecipe.Output,
            validRecipe.Inputs.Add(validInput),
            resultId);
    }

    public static RailwayResult<Recipe> AddInput(
        this Recipe recipe,
        Item item,
        int count,
        string? resultId = default)
    {
        return RecipeInput
            .FromParameters(item, count, resultId)
            .OnSuccess(input => recipe.AddInput(input, resultId));
    }

    public static RailwayResult<Recipe> DeleteInput(
        this Recipe recipe,
        Item item,
        string? resultId = default)
    {
        var failures = ImmutableList<RailwayResultBase>.Empty;

        var validRecipe = recipe
            .ToValidResult(nameof(recipe))
            .UnwrapOrAddToFailuresImmutable(ref failures);

        var validItem = item
            .ToValidResult(nameof(item))
            .UnwrapOrAddToFailuresImmutable(ref failures);

        if (!failures.IsEmpty)
        {
            return RailwayResult<Recipe>.Failure(failures.ToError("Unable to delete input"));
        }

        var updatedInputs = validRecipe.Inputs.RemoveAll(i => i.Item == validItem);

        return updatedInputs.Count == validRecipe.Inputs.Count
            ? RailwayResult<Recipe>.Success(validRecipe, resultId)
            : Recipe.FromParameters(
                validRecipe.Id,
                validRecipe.Profession,
                validRecipe.Output,
                updatedInputs,
                resultId);
    }

    public static RailwayResult<Recipe> SetInput(
        this Recipe recipe,
        RecipeInput input,
        string? resultId = default)
    {
        var failures = ImmutableList<RailwayResultBase>.Empty;

        var validRecipe = recipe
            .ToValidResult(nameof(recipe))
            .UnwrapOrAddToFailuresImmutable(ref failures);

        var validInput = input
            .ToValidResult(nameof(input))
            .UnwrapOrAddToFailuresImmutable(ref failures);

        if (!failures.IsEmpty)
        {
            return RailwayResult<Recipe>.Failure(failures.ToError("Unable to set input."), resultId);
        }

        var updatedInputs = validRecipe.Inputs.Remove(validInput);

        return updatedInputs.Count == validInput.Count
            ? RailwayResult<Recipe>.Success(validRecipe, resultId)
            : Recipe.FromParameters(
                validRecipe.Id,
                validRecipe.Profession,
                validRecipe.Output,
                updatedInputs.Add(validInput),
                resultId);
    }
}