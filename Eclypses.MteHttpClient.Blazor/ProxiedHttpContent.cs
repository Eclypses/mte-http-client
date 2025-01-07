using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Eclypses.MteHttpClient
{
    /// <summary>
    /// A custom content that allows for the outgoing
    /// request stream to be manipulated.
    /// </summary>
    /// <param name="streamWriter">A delegated function to allow the request stream to be processed.</param>
    /// <param name="getLength">A delegated function required for HttpContent.</param>
    public class ProxiedHttpContent(Func<Stream, CancellationToken, Task> streamWriter, Func<long?>? getLength = null) : HttpContent
    {
        private readonly Func<Stream, CancellationToken, Task> _streamWriter = streamWriter;
        private readonly Func<long?>? _getLength = getLength;

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context) =>
         _streamWriter(stream, default);

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context, CancellationToken cancellationToken) =>
            _streamWriter(stream, cancellationToken);

        protected override bool TryComputeLength(out long length)
        {
            var l = _getLength?.Invoke();
            length = l.GetValueOrDefault();
            return l.HasValue;
        }
    }
}
