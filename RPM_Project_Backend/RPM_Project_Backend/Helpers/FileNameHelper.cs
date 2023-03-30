﻿namespace RPM_Project_Backend.Helpers;

public static class FileNameHelper
{
    public static string GetUniqueFileName(string fileName)
    {
        fileName = Path.GetFileName(fileName);
        
        return string.Concat(Path.GetFileNameWithoutExtension(fileName),
            "_",
            Guid.NewGuid().ToString().AsSpan(0, 4),
            Path.GetExtension(fileName));
    }
}