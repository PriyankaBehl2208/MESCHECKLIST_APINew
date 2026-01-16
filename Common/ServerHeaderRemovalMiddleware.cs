namespace MESCHECKLIST.Common
{
    public class ServerHeaderRemovalMiddleware
    {
        private readonly RequestDelegate _next;
        public ServerHeaderRemovalMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.OnStarting(() =>
            {
                context.Response.Headers.Remove("Server");
                return Task.CompletedTask;
            });
            await _next(context);
        }
    }
}
