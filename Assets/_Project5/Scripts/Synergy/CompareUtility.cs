public static class CompareUtility
{
    public static bool Compare(int left, CompareType compareType, int right)
    {
        if (compareType == CompareType.Equal)
        {
            return left == right;
        }

        if (compareType == CompareType.GreaterOrEqual)
        {
            return left >= right;
        }

        if (compareType == CompareType.LessOrEqual)
        {
            return left <= right;
        }

        if (compareType == CompareType.Greater)
        {
            return left > right;
        }

        if (compareType == CompareType.Less)
        {
            return left < right;
        }

        return false;
    }
}
