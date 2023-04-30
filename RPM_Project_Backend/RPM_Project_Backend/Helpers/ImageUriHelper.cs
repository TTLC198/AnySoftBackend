using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace RPM_Project_Backend.Helpers;

public static class ImageUriHelper
{
    public static string GetImagePathAsUri(string absolutePath)
    {
        return HttpUtility.UrlPathEncode("/resources/image/" + string.Join(@"/", absolutePath.Split('\\').SkipWhile(s => s != "wwwroot").Skip(1)));
    }
}