﻿namespace AnySoftBackend.Library.Misc;

public class ErrorModel
{
    public string Message { get; set; }

    public ErrorModel(string message)
    {
        Message = message;
    }
}