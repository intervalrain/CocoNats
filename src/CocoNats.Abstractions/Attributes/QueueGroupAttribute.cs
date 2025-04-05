namespace CocoNats.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class QueueGroupAttribute(string group) : Attribute
{

    public string Group { get; } = group;
}