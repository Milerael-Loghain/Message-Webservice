using Service.Model;

namespace Service.Data.Abstract
{
    public interface IMessageDAL
    {
        public Task InsertMessage(int internalId, string text);
        public Task<List<Message>> GetMessagesInRangeAsync(DateTime from, DateTime to);
    }
}