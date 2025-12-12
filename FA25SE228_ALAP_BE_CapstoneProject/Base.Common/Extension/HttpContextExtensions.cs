using Microsoft.AspNetCore.Http;

namespace Base.Common.Extension
{
    public static class HttpContextExtensions
    {
        public static string? GetEventCode(this HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("X-Event-Code", out var value))
            {
                return value.ToString();
            }

            return null;
        }

        public static long GetEventId(this HttpContext context)
        {
            if (context.Items.TryGetValue("EventId", out var value) && value != null)
            {
                if (long.TryParse(value.ToString(), out var result))
                {
                    return result;
                }
            }

            return 0;
        }

    }
}
