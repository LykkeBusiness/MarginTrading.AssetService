using Microsoft.Extensions.Logging;

namespace Kathe.LogEnrichment.Outbound
{
    public class UrlQueryStringHider
    {
        private readonly ILogger _logger;

        public UrlQueryStringHider(ILogger logger)
        {
            _logger = logger;
        }

        public string HideQuerystringValues(Uri uri)
        {
            return HideQuerystringValues(uri?.ToString());
        }

        public string HideQuerystringValues(string url)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                return url;
            }

            if (url == null)
                return null;
            try
            {
                // Hiding the querystring by manually splitting the URL.
                // I've tried using Uri class but it's risky as it might throw 
                // if the uri is not 100% valid
                // Also tried to use httputility but working with NameValuecollection 
                // is not nice. So manually splitting here. 

                var parts = url.Split("?");


                var uriWithoutQueryString = parts[0];
                if (parts.Length == 1)
                {
                    // Url doesn't have a querystring
                    return url;
                }

                // in theory, there should only be a single ? character
                // in a querystring, but if you have more, let's keep them in. 
                var query = string.Join('?', parts.Skip(1));

                if (string.IsNullOrWhiteSpace(query))
                {
                    // No querystring found, keep original
                    return url;
                }

                // get all querystring parts
                var kvps = query.Split("&");

                // 
                var kvpsWithProtectedValue = kvps
                    .Select(x => x.Split('='))
                    .Select(ProtectKvp);

                return uriWithoutQueryString + "?" + string.Join('&', kvpsWithProtectedValue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to hide values in querystring for url {url}", url);
                return url;
            }

        }

        private static string ProtectKvp(string[] x)
        {
            if (x.Length == 1)
            {
                // the querystring parameter doesn't have a '='. So just leave it
                return x[0];
            }
            else
            {
                //
                return x[0] + "=" + ProtectVAlue(x[1]);
            }
        }

        /// <summary>
        /// prints out the length of the kvp
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string ProtectVAlue(string input)
        {
            return "(str:" + input.Length + ")";
        }
    }
}
