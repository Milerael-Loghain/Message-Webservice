using Npgsql;
using Service.Data.Abstract;
using Service.Model;

namespace Service.Data
{
    public class MessageDAL : IMessageDAL
    {
        private readonly string _connectionString;

        public MessageDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Task InsertMessage(int internalId, string text)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO messages (internal_id, text, created_at) VALUES (@internalId, @text, @created_at)";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("internalId", internalId);
                    command.Parameters.AddWithValue("text", text);
                    command.Parameters.AddWithValue("created_at", DateTime.Now);

                    command.ExecuteNonQuery();
                }
            }

            return Task.CompletedTask;
        }

        public async Task<List<Message>> GetMessagesInRangeAsync(DateTime from, DateTime to)
        {
            var messages = new List<Message>();

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
            SELECT internal_id, text, created_at
            FROM messages
            WHERE created_at BETWEEN @from AND @to
            ORDER BY created_at ASC;";

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@from", from);
            command.Parameters.AddWithValue("@to", to);

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                messages.Add(new Message
                {
                    InternalId = reader.GetInt32(0),
                    Text = reader.GetString(1),
                    CreatedAt = reader.GetDateTime(2)
                });
            }

            return messages;
        }
    }
}