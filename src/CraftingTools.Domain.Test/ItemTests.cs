﻿using System;
using System.Runtime;
using CraftingTools.Common;
using NUnit.Framework;
using SleepingBearSystems.Railway;

namespace CraftingTools.Domain.Test;

/// <summary>
/// Tests for <see cref="Item"/> class.
/// </summary>
internal static class ItemTests
{
    /// <summary>
    /// Validates the behavior of the <c>FromParameters</c> factory
    /// method.
    /// </summary>
    [Test]
    public static void FromParameters_ValidatesBehavior()
    {
        // use case: valid construction
        {
            var id = new Guid(g: "5E226140-DF07-47A8-B290-21F5B7E581B6");
            var itemName = ItemName.FromParameter(value: "name").Unwrap();
            var result = Item.FromParameters(id, itemName, resultId: "resultId");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Success));
            Assert.That(result.Id, Is.EqualTo(expected: "resultId"));
            var item = result.Unwrap();
            Assert.That(item.Id, Is.EqualTo(id));
            Assert.That(item.Name.Value, Is.EqualTo(expected: "name"));
        }

        // use case: invalid ID
        {
            var itemName = ItemName.FromParameter(value: "name").Unwrap();
            var result = Item.FromParameters(Guid.Empty, itemName, resultId: "resultId");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Failure));
            Assert.That(result.Error.Message, Is.EqualTo(expected: "Unable to create item."));
            Assert.That(result.Id, Is.EqualTo(expected: "resultId"));
        }
    }

    [Test]
    public static void FromPoco_ValidatesBehavior()
    {
        // use case: valid poco
        {
            var id = new Guid(g: "B13BA385-5AED-4AE7-9FA8-69F3D6FD24A1");
            var poco = new ItemPoco()
            {
                Id = id,
                Name = "name"
            };
            var result = Item.FromPoco(poco, resultId: "poco");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Success));
            Assert.That(result.Id, Is.EqualTo(expected: "poco"));
            var item = result.Unwrap();
            Assert.That(item.Id, Is.EqualTo(id));
            Assert.That(item.Name.Value, Is.EqualTo(expected: "name"));
        }

        // use case: null poco
        {
            var result = Item.FromPoco(poco: default, resultId: "null");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Success));
            Assert.That(result.Id, Is.EqualTo(expected: "null"));
            var item = result.Unwrap();
            Assert.That(item, Is.EqualTo(Item.None));
        }

        // use case: invalid poco
        {
            var result = Item.FromPoco(new ItemPoco()
            {
                Id = Guid.Empty,
            });
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Failure));
            Assert.That(result.Error.Message, Is.EqualTo(expected: "Item name cannot be empty."));
        }
    }
}
