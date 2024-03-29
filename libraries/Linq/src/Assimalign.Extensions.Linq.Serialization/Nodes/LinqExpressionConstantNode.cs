﻿using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;


namespace Assimalign.Extensions.Linq.Serialization.Nodes;

using Assimalign.Extensions.Linq.Serialization.Exceptions;
using Assimalign.Extensions.Linq.Serialization.Internal;

[Serializable]
[DataContract(Name = "C")]   
public class LinqExpressionConstantNode : LinqExpressionNode<ConstantExpression>
{
    private object _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="LinqExpressionConstantNode"/> class.
    /// </summary>
    public LinqExpressionConstantNode() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinqExpressionConstantNode"/> class.
    /// </summary>
    /// <param name="factory">The factory.</param>
    /// <param name="value">The value.</param>
    public LinqExpressionConstantNode(ILinqExpressionNodeFactory factory, object value)
        : this(factory, value, null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinqExpressionConstantNode" /> class.
    /// </summary>
    /// <param name="factory">The factory.</param>
    /// <param name="value">The value.</param>
    /// <param name="type">The type.</param>
    public LinqExpressionConstantNode(ILinqExpressionNodeFactory factory, object value, Type type)
        : base(factory, ExpressionType.Constant)
    {
        Value = value;
        if (type != null)
            base.Type = factory.Create(type);

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinqExpressionConstantNode"/> class.
    /// </summary>
    /// <param name="factory">The factory.</param>
    /// <param name="expression">The expression.</param>
    public LinqExpressionConstantNode(ILinqExpressionNodeFactory factory, ConstantExpression expression)
        : base(factory, expression) { }

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    /// <value>
    /// The type.
    /// </value>
    /// <exception cref="InvalidTypeException"></exception>
    public override TypeNode Type
    {
        get => base.Type;
        set
        {
            if (Value != null)
            {
                if (value == null)
                {
                    value = Factory.Create(Value.GetType());
                }
                else
                {
                    var context = new LinqExpressionContext();
                    if (!value.ToType(context).IsInstanceOfType(Value))
                        throw new InvalidTypeException(
                            $"Type '{value.ToType(context)}' is not an instance of the current value type '{Value.GetType()}'.");
                }
            }
            base.Type = value;
        }
    }


    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    /// <exception cref="System.ArgumentException">Expression not allowed.;value</exception>
    [DataMember(EmitDefaultValue = false, Name = "V")]
    public object Value
    {
        get { return _value; }
        set
        {
            if (value is Expression)
                throw new ArgumentException("Expression not allowed.", "value");

            _value = value is Type valueType ? Factory.Create(valueType) : value;

            if (_value == null || _value is TypeNode) 
                return;

            var type = base.Type != null ? base.Type.ToType(new LinqExpressionContext()) : null;
            if (type == null)
            {
                if (Factory != null)
                    base.Type = Factory.Create(_value.GetType());
                return;
            }
            _value = ValueConverter.Convert(_value, type);
        }
    }

    /// <summary>
    /// Initializes the specified expression.
    /// </summary>
    /// <param name="expression">The expression.</param>
    protected override void Initialize(ConstantExpression expression)
    {
        Value = expression.Value;
    }

    /// <summary>
    /// Converts this instance to an expression.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns></returns>
    public override Expression ToExpression(ILinqExpressionContext context)
    {
        if (Value is TypeNode typeNode)
            return Expression.Constant(typeNode.ToType(context), Type.ToType(context));

        return Type != null 
            ? Expression.Constant(Value, Type.ToType(context)) 
            : Expression.Constant(Value);
    }
}
