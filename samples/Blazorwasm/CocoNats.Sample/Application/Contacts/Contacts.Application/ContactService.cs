using Application.Contacts.Domain;

using CocoNats.Core.Services;

using CocoNats.Sample.Shared.Contacts;

using NATS.Client.Core;

namespace Application.Contacts.Contacts.Application;

public static class ContactEntityExtensions
{
    public static ContactDto ToDto(this Contact contact)
    {
        return new ContactDto
        {
            Id = contact.Id,
            Sex = contact.Sex,
            Name = contact.Name,
            Address = contact.Address,
            Phone = contact.Phone,
        };
    }

    public static Contact ToEntity(this ContactDto contact)
    {
        return new Contact(contact.Id, contact.Sex, contact.Name, contact.Address, contact.Phone);
    }
}

public class ContactService(IContactRepository contactRepository) : NatsService
{
    private readonly IContactRepository _repository = contactRepository;

    // [Subject("contact.post.*")]
    public async Task<ContactDto> CreateContact(NatsMsg<CreateUpdateContactDto> message)
    {
        var request = message.Data ?? throw new Exception("Error processing message");
        var contact = Contact.Create(request.Sex, request.Name, request.Address, request.Phone);
        var result = await _repository.CreateContactAsync(contact) ?? throw new Exception();

        return result.ToDto();
    }

    // [Subject("contact.get.*")]
    public async Task<ContactDto> GetContact(NatsMsg<object> message)
    {
        var parts = message.Subject.Split('.');
        if (parts.Length != 3 || !Guid.TryParse(parts[2], out var contactId))
        {
            throw new Exception("Invalid contact Id in subject");
        }

        var contact = await _repository.GetContactAsync(contactId) ?? throw new Exception();
        return contact.ToDto();
    }

    // [Subject("contact.get")]
    public async Task<List<ContactDto>> GetContacts(NatsMsg<object> message)
    {
        var contacts = await _repository.GetContactsAsync();
        return contacts.ConvertAll(contact => contact.ToDto());
    }

    // [Subject("contact.put.*")]
    public async Task<ContactDto> UpdateContact(NatsMsg<CreateUpdateContactDto> message)
    {
        var request = message.Data ?? throw new Exception("Error processing message");
        var parts = message.Subject.Split('.');
        if (parts.Length != 3 || !Guid.TryParse(parts[2], out var contactId))
        {
            throw new Exception("Invalid contact Id in subject");
        }

        var contact = await _repository.UpdateContactAsync(contactId, Contact.Create(request.Sex, request.Name, request.Address, request.Name)) ?? throw new Exception();
        return contact.ToDto();
    }

    // [Subject("contact.delete.*")]
    public async Task<bool> DeleteContact(NatsMsg<object> message)
    {
        var parts = message.Subject.Split('.');
        if (parts.Length != 3 || !Guid.TryParse(parts[2], out var contactId))
        {
            throw new Exception("Invalid contact Id in subject");
        }

        var success = await _repository.DeleteContactAsync(contactId);
        return success;
    }
}
