namespace CocoNats.Sample.Shared.Contacts;

public class ContactDto
{
    public Guid Id { get; set; }

    public Sex Sex { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;
}
