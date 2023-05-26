using System;
using AnySoftBackend.Library.Converters;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;

namespace AnySoftBackend.Library.Misc;

public class MinMaxObject<T> where T: struct
{
    [ValidateNever]
    public T? Min { get; set; }
    [ValidateNever]
    public T? Max { get; set; }
}

public class MinMaxDateTimeObject : MinMaxObject<DateTime>
{
    [ValidateNever]
    [JsonConverter(typeof(JsonDateTimeConverter))]
    public new DateTime? Min { get; set; }
    [ValidateNever]
    [JsonConverter(typeof(JsonDateTimeConverter))]
    public new DateTime? Max { get; set; }
}