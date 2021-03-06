﻿using System.IO;
using Nancy;

namespace GroupMessage.Server.Module
{
    public static class ResponseExtensions
    {
        public static Response SetBody(this Response response, string body)
        {
            response.Contents = s =>
                {
                    var streamWriter = new StreamWriter(s);
                    streamWriter.Write(body);
                    streamWriter.Flush();
                };

            return response;
        }

        public static Response SetContentType(this Response response, string contentType)
        {
            response.ContentType = contentType;
            return response;
        }

        public static Response Create(this Response response, HttpStatusCode statusCode, string body)
        {
            return response.Create(statusCode, body, "text/plain");
        }

        public static Response Create(this Response response, HttpStatusCode statusCode, string body, string contentType)
        {
            response.StatusCode = statusCode;
            response.ContentType = contentType;
            return response.SetBody(body);
        }

    }
}