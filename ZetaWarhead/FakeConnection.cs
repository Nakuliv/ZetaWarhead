using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZetaWarhead
{
    public sealed class FakeConnection : NetworkConnectionToClient
    {
        public override void Send(ArraySegment<byte> segment, int channelId = 0) { }

        public override string address => "localhost";

        public FakeConnection(int networkConnectionId) : base(networkConnectionId) { }
    }
}
