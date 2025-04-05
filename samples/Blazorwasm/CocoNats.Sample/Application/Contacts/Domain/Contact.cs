using CocoNats.Sample.Shared.Contacts;

namespace Application.Contacts.Domain;

public class Contact
{
    internal Contact(Guid id, Sex sex, string name, string address, string phone)
    {
        Id = id;
        Sex = sex;
        Name = name;
        Address = address;
        Phone = phone;
    }

    private Contact()
    {
    }

    public Guid Id { get; set; }

    public Sex Sex { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public static Contact Create(Sex sex, string name, string address, string phone)
    {
        return new Contact(Guid.NewGuid(), sex, name, address, phone);
    }
}
