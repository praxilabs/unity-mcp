using System;
using static XNode.Node;

[AttributeUsage(AttributeTargets.Field)]
public class GetAttribute : InputAttribute
{
    public GetAttribute(ShowBackingValue backingValue = ShowBackingValue.Always,
                        ConnectionType connectionType = ConnectionType.Override,
                        TypeConstraint typeConstraint = TypeConstraint.Strict,
                        bool dynamicPortList = false)
    {
        this.backingValue = backingValue;
        this.connectionType = connectionType;
        this.dynamicPortList = dynamicPortList;
        this.typeConstraint = typeConstraint;
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class SetAttribute : OutputAttribute
{
    public SetAttribute(ShowBackingValue backingValue = ShowBackingValue.Always,
                        ConnectionType connectionType = ConnectionType.Override,
                        TypeConstraint typeConstraint = TypeConstraint.Strict,
                        bool dynamicPortList = false)
    {
        this.backingValue = backingValue;
        this.connectionType = connectionType;
        this.dynamicPortList = dynamicPortList;
        this.typeConstraint = typeConstraint;
    }
}