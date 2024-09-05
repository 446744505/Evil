
using System.Collections.Immutable;

namespace Game
{
    public static partial class Program
    {
        internal static readonly Context Ctx = new();
    }

    internal class Context
    {
        private ImmutableHashSet<ushort> m_MapPvids = ImmutableHashSet.Create<ushort>();

        internal void AddMapPvid(ushort pvid)
        {
            m_MapPvids = m_MapPvids.Add(pvid);
        }
    }
}