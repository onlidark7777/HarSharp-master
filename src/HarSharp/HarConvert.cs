﻿using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace HarSharp
{
    /// <summary>
    /// Provides methods for converting between HTTP Archive Format (HAR) and HAR entities.
    /// </summary>
    public static class HarConvert
    {
        /// <summary>
        /// Deserialize HAR content to a HAR entity.
        /// </summary>
        /// <param name="har">The HAR content to be deserialized.</param>
        /// <returns>The HAR entity.</returns>
        public static Har Deserialize(string harJson)
        {
            if (string.IsNullOrWhiteSpace(harJson))
            {
                throw new ArgumentNullException(nameof(harJson));
            }

            var result = JsonConvert.DeserializeObject<Har>(harJson);

            TransformPartialRedirectUrlToFull(result);

            return result;
        }

        /// <summary>
        /// Deserialize a HAR file to a HAR entity.
        /// </summary>
        /// <param name="fileName">The HAR file name to be deserialized.</param>
        /// <returns>The HAR entity.</returns>
        public static Har DeserializeFromFile(string fileName)
        {
            return Deserialize(File.ReadAllText(fileName));
        }

        /// <summary>
        /// Transform the partial redirect URL to a full one.
        /// </summary>
        /// <param name="har">The HAR.</param>
        private static void TransformPartialRedirectUrlToFull(Har har)
        {
            var responsesWithPartialRedirectUrl = har.Log.Entries
                .Where(e => e.Response.RedirectUrl != null && e.Response.RedirectUrl.OriginalString.StartsWith("/", StringComparison.OrdinalIgnoreCase));

            foreach (var entry in responsesWithPartialRedirectUrl)
            {
                var requestUrl = entry.Request.Url;
                entry.Response.RedirectUrl = new Uri($"{requestUrl.GetLeftPart(UriPartial.Authority)}{(entry.Response.RedirectUrl.IsAbsoluteUri ? entry.Response.RedirectUrl.AbsolutePath : entry.Response.RedirectUrl.OriginalString)}");
            }
        }        
    }
}
