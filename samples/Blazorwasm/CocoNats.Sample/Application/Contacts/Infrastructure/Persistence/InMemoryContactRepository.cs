using Application.Contacts.Domain;

using CocoNats.Sample.Shared.Contacts;

namespace Application.Contacts.Infrastructure.Persistence;

public class InMemoryContactRepository : IContactRepository
{
    private readonly List<Contact> _contacts;

    public InMemoryContactRepository()
    {
        _contacts =
        [
            Contact.Create(Sex.Male, "John Doe", "123 Main St", "123-456-7890"),
            Contact.Create(Sex.Female, "Jane Smith", "456 Oak Ave", "234-567-8901"),
            Contact.Create(Sex.Male, "Mike Johnson", "789 Pine Rd", "345-678-9012"),
            Contact.Create(Sex.Female, "Emily Davis", "321 Elm St", "456-789-0123"),
            Contact.Create(Sex.Male, "David Wilson", "654 Maple Dr", "567-890-1234"),
            Contact.Create(Sex.Female, "Sarah Brown", "987 Cedar Ln", "678-901-2345"),
            Contact.Create(Sex.Male, "Chris White", "159 Birch Ct", "789-012-3456"),
            Contact.Create(Sex.Female, "Laura Green", "753 Willow Way", "890-123-4567"),
            Contact.Create(Sex.Male, "James Black", "852 Ash Blvd", "901-234-5678"),
            Contact.Create(Sex.Female, "Olivia Moore", "951 Spruce Rd", "012-345-6789"),
            Contact.Create(Sex.Male, "Robert Hall", "147 Redwood St", "123-654-7890"),
            Contact.Create(Sex.Female, "Sophia Lewis", "369 Sequoia Pl", "234-765-8901"),
            Contact.Create(Sex.Male, "William Clark", "258 Fir Ave", "345-876-9012"),
            Contact.Create(Sex.Female, "Isabella Turner", "147 Cedar Dr", "456-987-0123"),
            Contact.Create(Sex.Male, "Ethan Martinez", "369 Aspen Ln", "567-098-1234")
        ];
    }

    public Task<Contact> CreateContactAsync(Contact contact)
    {
        _contacts.Add(contact);
        return Task.FromResult(contact);
    }

    public Task<Contact?> GetContactAsync(Guid id)
    {
        return Task.FromResult(_contacts.FirstOrDefault(c => c.Id == id));
    }

    public Task<List<Contact>> GetContactsAsync()
    {
        return Task.FromResult(_contacts.ToList());
    }

    public Task<Contact?> UpdateContactAsync(Guid id, Contact contact)
    {
        var existingContact = _contacts.FirstOrDefault(c => c.Id == id);
        if (existingContact == null)
        {
            return Task.FromResult<Contact?>(null);
        }

        existingContact.Sex = contact.Sex;
        existingContact.Name = contact.Name;
        existingContact.Address = contact.Address;
        existingContact.Phone = contact.Phone;

        return Task.FromResult(existingContact)!;
    }

    public Task<bool> DeleteContactAsync(Guid id)
    {
        var contact = _contacts.FirstOrDefault(c => c.Id == id);
        if (contact == null)
        {
            return Task.FromResult(false);
        }

        _contacts.Remove(contact);
        return Task.FromResult(true);
    }
}
