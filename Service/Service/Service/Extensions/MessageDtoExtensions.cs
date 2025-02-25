using Service.DTO;
using Service.Model;

namespace Service.Extensions;

public static class MessageDtoExtensions
{
    public static Message ToMessage(this SendMessageDTO sendMessageDto, DateTime dateTime)
    {
        return new Message
        {
            InternalId = sendMessageDto.InternalId,
            Text = sendMessageDto.Text,
            CreatedAt = dateTime
        };
    }
}