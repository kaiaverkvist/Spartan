using System;
using System.Net;
using Spartan.Core.Http;

namespace Spartan.Dev
{
    public class RequestLogger
    {
        public RequestLogger(HttpHandler handler)
        {
            handler.onHttpRequest += OnHttpRequest;
        }

        private void OnHttpRequest(object source, HttpListenerContext context)
        {
            Console.WriteLine($"Request from IP {context.Request.RemoteEndPoint} to {context.Request.Url}");
        }
    }
}
