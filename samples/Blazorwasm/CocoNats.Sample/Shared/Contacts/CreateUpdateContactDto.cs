namespace CocoNats.Sample.Shared.Contacts;

public class CreateUpdateContactDto
{
    public required Sex Sex { get; set; }

    public required string Name { get; set; }

    public required string Address { get; set; }

    public required string Phone { get; set; }
}
