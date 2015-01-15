﻿using System.Net;
using EasyGelf.Core;
using EasyGelf.Core.Encoders;
using EasyGelf.Core.Udp;
using JetBrains.Annotations;
using System.Linq;

namespace EasyGelf.Log4Net
{
    public sealed class GelfUdpAppender : GelfAppenderBase
    {
        public GelfUdpAppender()
        {
            RemoteAddress = IPAddress.Loopback.ToString();
            RemotePort = 12201;
            MessageSize = 1024;
        }

        [UsedImplicitly]
        public int MessageSize { get; set; }

        [UsedImplicitly]
        public string RemoteAddress { get; set; }

        [UsedImplicitly]
        public int RemotePort { get; set; }

        protected override ITransport InitializeTransport()
        {
            var remoteIpAddress = Dns.GetHostAddresses(RemoteAddress)
                .Shuffle()
                .FirstOrDefault() ?? IPAddress.Loopback;
            var encoder = new CompositeEncoder(new GZipEncoder(), new ChunkingEncoder(new MessageBasedIdGenerator(), MessageSize.UdpMessageSize()));
            var configuration = new UdpTransportConfiguration
                {
                    Host = new IPEndPoint(remoteIpAddress, RemotePort),
                };
            return new UdpTransport(configuration, encoder, new GelfMessageSerializer());
        }
    }
}