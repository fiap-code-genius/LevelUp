namespace LevelUp.Domain.Errors
{
    public class IdNotFoundException : Exception
    {
        public IdNotFoundException() { }

        public IdNotFoundException(string? message) : base(message)
        {
            Console.WriteLine($"Erro - {message} - {DateTime.UtcNow.ToString()}");
        }
    }
}
