namespace Application.Contacts.Domain;

public interface IContactRepository
{
    Task<Contact> CreateContactAsync(Contact contact);

    Task<List<Contact>> GetContactsAsync();

    Task<Contact?> GetContactAsync(Guid id);

    Task<Contact?> UpdateContactAsync(Guid id, Contact contact);

    Task<bool> DeleteContactAsync(Guid id);
}
