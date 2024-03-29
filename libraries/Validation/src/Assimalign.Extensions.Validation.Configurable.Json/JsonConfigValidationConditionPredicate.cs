﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Assimalign.Extensions.Validation.Configurable;

using Assimalign.Extensions.Validation.Configurable.Serialization;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class JsonConfigValidationConditionPredicate<T>
    where T : class
{
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("$and")]
    public IEnumerable<JsonConfigValidationCondition<T>> And { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("$or")]
    public IEnumerable<JsonConfigValidationCondition<T>> Or { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("$member")]
    public string Member { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("$operator")]
    public JsonConfigOperatorType Operator { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("$value")]
    [JsonConverter(typeof(ObjectConverter))]
    public object Value { get; set; }
}