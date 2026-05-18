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

public static class FinanceTransactionHelper
{
    public static string ResolveTransactionType(FinanceTransaction transaction, Guid currentUserId)
    {
        if (transaction.IsFromBalance)
            return TransactionTypes.Shared;

        return transaction.CreatorId == new UserId(currentUserId)
            ? TransactionTypes.Personal
            : TransactionTypes.Team;
    }
}

public static class TransactionTypes
{
    public const string Personal = "personal";
    public const string Shared = "shared";
    public const string Team = "team";
}
