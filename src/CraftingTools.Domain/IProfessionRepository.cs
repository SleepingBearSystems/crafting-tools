﻿using System.Collections.Immutable;
using SleepingBearSystems.Common;

namespace CraftingTools.Domain;

public interface IProfessionRepository
{
    Maybe<Profession> GetProfessionById(Guid id);

    ImmutableList<Profession> GetProfessions();
}
