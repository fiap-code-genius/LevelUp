namespace LevelUp.Domain.Errors
{
    public class NoContentException : Exception
    {
        public NoContentException()
        {
        }

        public NoContentException(string? message) : base(message)
        {
            Console.WriteLine($"Erro - {message} - {DateTime.UtcNow.ToString()}");
        }
    }
}
