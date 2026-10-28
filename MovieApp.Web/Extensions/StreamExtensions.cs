namespace MovieApp.Web.Extensions
{
    public static class StreamExtensions
    {
        extension(Stream? self)
        {
            public bool IsEmptyOrDefault()
            {
                if (self is null) return false;
                if (!self.CanSeek)
                    return false;
                return self.Length == 0;
            }
        }
    }
}
