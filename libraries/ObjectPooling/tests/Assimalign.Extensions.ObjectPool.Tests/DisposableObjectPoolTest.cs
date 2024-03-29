using System;
using System.Collections.Generic;
using Xunit;


namespace Assimalign.Extensions.ObjectPool.Tests;

public class DisposableObjectPoolTest
{
    [Fact]
    public void DisposableObjectPoolWithDefaultPolicy_GetAnd_ReturnObject_SameInstance()
    {
        // Arrange
        var pool = new ObjectPoolDisposable<object>(new ObjectPoolPolicyDefault<object>());

        var obj1 = pool.Get();
        pool.Return(obj1);

        // Act
        var obj2 = pool.Get();

        // Assert
        Assert.Same(obj1, obj2);
    }

    [Fact]
    public void DisposableObjectPool_GetAndReturnObject_SameInstance()
    {
        // Arrange
        var pool = new ObjectPoolDisposable<List<int>>(new ListPolicy());

        var list1 = pool.Get();
        pool.Return(list1);

        // Act
        var list2 = pool.Get();

        // Assert
        Assert.Same(list1, list2);
    }

    [Fact]
    public void DisposableObjectPool_Return_RejectedByPolicy()
    {
        // Arrange
        var pool = new ObjectPoolDisposable<List<int>>(new ListPolicy());
        var list1 = pool.Get();
        list1.Capacity = 20;

        // Act
        pool.Return(list1);
        var list2 = pool.Get();

        // Assert
        Assert.NotSame(list1, list2);
    }

    [Fact]
    public void DisposableObjectPoolWithOneElement_Dispose_ObjectDisposed()
    {
        // Arrange
        var pool = new ObjectPoolDisposable<DisposableObject>(new ObjectPoolPolicyDefault<DisposableObject>());
        var obj = pool.Get();
        pool.Return(obj);

        // Act
        pool.Dispose();

        // Assert
        Assert.True(obj.IsDisposed);
    }

    [Fact]
    public void DisposableObjectPoolWithTwoElements_Dispose_ObjectsDisposed()
    {
        // Arrange
        var pool = new ObjectPoolDisposable<DisposableObject>(new ObjectPoolPolicyDefault<DisposableObject>());
        var obj1 = pool.Get();
        var obj2 = pool.Get();
        pool.Return(obj1);
        pool.Return(obj2);

        // Act
        pool.Dispose();

        // Assert
        Assert.True(obj1.IsDisposed);
        Assert.True(obj2.IsDisposed);
    }

    [Fact]
    public void DisposableObjectPool_DisposeAndGet_ThrowsObjectDisposed()
    {
        // Arrange
        var pool = new ObjectPoolDisposable<DisposableObject>(new ObjectPoolPolicyDefault<DisposableObject>());
        var obj1 = pool.Get();
        var obj2 = pool.Get();
        pool.Return(obj1);
        pool.Return(obj2);

        // Act
        pool.Dispose();

        // Assert
        Assert.Throws<ObjectDisposedException>(() => pool.Get());
    }

    [Fact]
    public void DisposableObjectPool_DisposeAndReturn_DisposesObject()
    {
        // Arrange
        var pool = new ObjectPoolDisposable<DisposableObject>(new ObjectPoolPolicyDefault<DisposableObject>());
        var obj = pool.Get();

        // Act
        pool.Dispose();
        pool.Return(obj);

        // Assert
        Assert.True(obj.IsDisposed);
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

    private class DisposableObject : IDisposable
    {
        public bool IsDisposed { get; private set; }

        public void Dispose() => IsDisposed = true;
    }
}