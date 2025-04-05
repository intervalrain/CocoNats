namespace CocoNats.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class SubjectAttribute(string subject) : Attribute
{
    public string Subject { get; } = subject;
}