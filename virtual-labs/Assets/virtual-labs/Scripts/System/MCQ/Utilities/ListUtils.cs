using System.Collections.Generic;

public static class ListUtils
{
    public static void AddDerivedToBaseList<TBase, TDerived>(List<TBase> baseList, List<TDerived> derivedList) where TDerived : TBase
    {
        foreach (var item in derivedList)
        {
            baseList.Add(item);
        }
    }

    public static void AddBaseToDerivedList<TBase, TDerived>(List<TDerived> derivedList, List<TBase> baseList) where TDerived : TBase
    {
        foreach (var item in baseList)
        {
            if (item is TDerived derivedItem)
            {
                derivedList.Add(derivedItem);
            }
        }
    }
}
