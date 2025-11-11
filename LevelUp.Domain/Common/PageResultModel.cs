namespace LevelUp.Domain.Common
{
    public class PageResultModel<T>
    {
        public required T Data { get; set; }
        public int Offset { get; set; }
        public int Take { get; set; }
        public int Total { get; set; }
    }
}
