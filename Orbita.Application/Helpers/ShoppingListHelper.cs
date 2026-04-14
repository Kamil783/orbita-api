using Orbita.Domain.Entities;
using Orbita.Domain.ValueObjects;

namespace Orbita.Application.Helpers;

public static class ShoppingListHelper
{
    public static string ResolveListType(ShoppingList shoppingList, Guid currentUserId)
    {
        if (shoppingList.IsFromBalance)
            return ShoppingListTypes.Shared;

        return shoppingList.CreatorId == new UserId(currentUserId)
            ? ShoppingListTypes.Personal
            : ShoppingListTypes.Team;
    }
}

public static class ShoppingListTypes
{
    public const string Personal = "personal";
    public const string Shared = "shared";
    public const string Team = "team";
}
