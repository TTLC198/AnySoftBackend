﻿using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace AnySoftBackend.Helpers;

public static class ImageUriHelper
{
    public static string GetImagePathAsUri(string? absolutePath) => 
        absolutePath is null or {Length: 0} 
            ? "" 
            : HttpUtility.UrlPathEncode("/resources/image/" + string.Join(@"/", absolutePath
                .Split('\\')
                .SkipWhile(s => s != "wwwroot")
                .Skip(1)));
}