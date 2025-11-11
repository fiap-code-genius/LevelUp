namespace LevelUp.Domain.Errors
{
    public class EmailNotFoundException : Exception
    {
        public EmailNotFoundException()
        {
            
        }

        public EmailNotFoundException(string? message) : base(message)
        {
            Console.WriteLine($"Erro - {message} - {DateTime.UtcNow.ToString()}");
        }
    }
}
