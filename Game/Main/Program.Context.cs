
using System.Collections.Immutable;
using System.Linq;
using Evil.Util;

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
            Log.I.Info($"add map pvid: {pvid}");
        }

        internal void RemoveMapPvid(ushort pvid)
        {
            m_MapPvids = m_MapPvids.Remove(pvid);
            Log.I.Info($"remove map pvid: {pvid}");
        }

        internal ushort RandomMapPvid()
        {
            if (m_MapPvids.IsEmpty)
            {
                return 0;
            }

            return m_MapPvids.ToArray()[Edb.Edb.I.Random.Next(0, m_MapPvids.Count)];
        }
    }
}