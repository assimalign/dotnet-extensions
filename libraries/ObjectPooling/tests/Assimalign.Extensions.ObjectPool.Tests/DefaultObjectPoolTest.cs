﻿using Xunit;
using System.Collections.Generic;

namespace Assimalign.Extensions.ObjectPool.Tests;

public class DefaultObjectPoolTest
{

    [Fact]
    public void DefaultObjectPoolWithDefaultPolicy_GetAnd_ReturnObject_SameInstance()
    {
        // Arrange
        var pool = new ObjectPoolDefault<object>(new ObjectPoolPolicyDefault<object>());

        var obj1 = pool.Get();
        pool.Return(obj1);

        // Act
        var obj2 = pool.Get();

        // Assert
        Assert.Same(obj1, obj2);
    }

    [Fact]
    public void DefaultObjectPool_GetAndReturnObject_SameInstance()
    {
        // Arrange
        var pool = new ObjectPoolDefault<List<int>>(new ListPolicy());

        var list1 = pool.Get();
        pool.Return(list1);

        // Act
        var list2 = pool.Get();

        // Assert
        Assert.Same(list1, list2);
    }

    [Fact]
    public void DefaultObjectPool_CreatedByPolicy()
    {
        // Arrange
        var pool = new ObjectPoolDefault<List<int>>(new ListPolicy());

        // Act
        var list = pool.Get();

        // Assert
        Assert.Equal(17, list.Capacity);
    }

    [Fact]
    public void DefaultObjectPool_Return_RejectedByPolicy()
    {
        // Arrange
        var pool = new ObjectPoolDefault<List<int>>(new ListPolicy());
        var list1 = pool.Get();
        list1.Capacity = 20;

        // Act
        pool.Return(list1);
        var list2 = pool.Get();

        // Assert
        Assert.NotSame(list1, list2);
    }

    private class ListPolicy : IObjectPoolPolicy<List<int>>
    {
        public List<int> Create()
        {
            return new List<int>(17);
        }

        public bool Return(List<int> obj)
        {
            return obj.Capacity == 17;
        }
    }
}
