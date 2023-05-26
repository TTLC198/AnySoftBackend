using AnySoftBackend.Models;

namespace AnySoftBackend.Helpers;

public static class QueryParametersExtensions
{
    public static bool HasPrevious<T>(this QueryParameters<T> queryParameters)
    {
        return (queryParameters.Page > 1);
    }

    public static bool HasNext<T>(this QueryParameters<T> queryParameters, int totalCount)
    {
        if (queryParameters == null) throw new ArgumentNullException(nameof(queryParameters));
        return (queryParameters.Page < (int)GetTotalPages(queryParameters, totalCount));
    }

    public static double GetTotalPages<T>(this QueryParameters<T> queryParameters, int totalCount)
    {
        return Math.Ceiling(totalCount / (double)queryParameters.PageCount);
    }

    public static bool HasQuery<T>(this QueryParameters<T> queryParameters)
    {
        return !String.IsNullOrEmpty(queryParameters.Query);
    }

    public static bool IsDescending<T>(this QueryParameters<T> queryParameters)
    {
        if (!String.IsNullOrEmpty(queryParameters.OrderBy))
        {
            return queryParameters.OrderBy.Split(' ').Last().ToLowerInvariant().StartsWith("desc");
        }
        return false;
    }
}