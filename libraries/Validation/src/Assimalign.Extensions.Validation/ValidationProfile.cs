﻿using System;
using System.Collections.Generic;


namespace Assimalign.Extensions.Validation;

using Assimalign.Extensions.Validation.Internal;

/// <summary>
/// A validation profile is used to describe the rules of <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ValidationProfile<T> : IValidationProfile<T>
{
    private readonly Type validationType;
    private readonly IValidationItemStack validationItems;

    /// <summary>
    /// 
    /// </summary>
    public ValidationProfile()
    {
        this.validationType = typeof(T);
        this.validationItems = new ValidationItemStack();
    }

    /// <summary>
    /// A collection validation rules to apply to the instance of <typeparamref name="T"/>
    /// for a given context.
    /// </summary>
    public IValidationItemStack ValidationItems => this.validationItems;

    /// <summary>
    /// The type of <typeparamref name="T"/> being validated.
    /// </summary>
    public Type ValidationType => this.validationType;

    /// <summary>
    /// 
    /// </summary>
    void IValidationProfile.Configure(IValidationRuleDescriptor descriptor)
    {
        if (descriptor is ValidationRuleDescriptor<T> td)
        {
            this.Configure(td);
        }
    }

    /// <summary>
    /// Configures the validation rules to be applied on the type <typeparamref name="T"/>
    /// </summary>
    /// <param name="descriptor"></param>
    public abstract void Configure(IValidationRuleDescriptor<T> descriptor);
}


/// <summary>
/// 
/// </summary>
public abstract class ValidationProfile : IValidationProfile
{
    private readonly Type validationType;
    private readonly IValidationItemStack validationItems;

    /// <summary>
    /// 
    /// </summary>
    public ValidationProfile(Type type)
    {
        this.validationType = type;
        this.validationItems = new ValidationItemStack();
    }

    /// <summary>
    /// 
    /// </summary>
    public Type ValidationType => this.validationType;

    /// <summary>
    /// 
    /// </summary>
    public IValidationItemStack ValidationItems => this.validationItems;

    /// <summary>
    /// 
    /// </summary>
    public abstract void Configure(IValidationRuleDescriptor descriptor);
}