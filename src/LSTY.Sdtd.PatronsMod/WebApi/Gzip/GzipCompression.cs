﻿using Nancy;
using Nancy.Bootstrapper;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.WebApi.Gzip
{
    public static class GzipCompression
    {
        private static GzipCompressionSettings _settings;

        public static void EnableGzipCompression(this IPipelines pipelines, GzipCompressionSettings settings)
        {
            _settings = settings;
            pipelines.AfterRequest += CheckForCompression;
        }

        public static void EnableGzipCompression(this IPipelines pipelines)
        {
            EnableGzipCompression(pipelines, new GzipCompressionSettings());
        }

        private static void CheckForCompression(NancyContext context)
        {
            if (RequestIsGzipCompatible(context.Request) == false)
            {
                return;
            }

            Response response = context.Response;

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return;
            }

            if (ResponseIsCompatibleMimeType(response) == false)
            {
                return;
            }

            if (ContentLengthIsTooSmall(response))
            {
                return;
            }

            CompressResponse(response);
        }

        private static void CompressResponse(Response response)
        {
            response.Headers["Content-Encoding"] = "gzip";

            var contents = response.Contents;
            response.Contents = responseStream =>
            {
                using (var compression = new GZipStream(responseStream, CompressionMode.Compress))
                {
                    contents(compression);
                }
            };
        }

        private static bool ContentLengthIsTooSmall(Response response)
        {
            string contentLength;
            if (response.Headers.TryGetValue("Content-Length", out contentLength))
            {
                var length = int.Parse(contentLength);
                if (length < _settings.MinimumBytes)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ResponseIsCompatibleMimeType(Response response)
        {
            return _settings.MimeTypes.Any(x => x == response.ContentType || response.ContentType.StartsWith($"{x};"));
        }

        private static bool RequestIsGzipCompatible(Request request)
        {
            return request.Headers.AcceptEncoding.Any(x => x.Contains("gzip"));
        }
    }
}
