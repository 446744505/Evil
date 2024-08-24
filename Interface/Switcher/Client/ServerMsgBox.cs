﻿using Attributes;

namespace Evil.Switcher.Client
{
    [Protocol(Node.Switcher|Node.Client)]
    public class ServerMsgBox
    {
        [ProtocolField(1)]
        private uint messageId;
        [ProtocolField(2)]
        private byte[] data;
    }
}