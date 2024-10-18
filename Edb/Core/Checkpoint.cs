using Evil.Util;

namespace Edb
{
    public class Checkpoint
    {
        private readonly int m_SchedPeriod;
        private readonly Tables m_Tables;

        private long m_NextMarshalTime;
        private long m_NextCheckpointTime;
        private volatile bool m_CheckpointNow;
        
        private readonly Elapse m_Elapse = new();

        #region Mertics

        private long m_MarshalNCount;
        private long m_Marshal0Count;
        private long m_SnapshotCount;
        private long m_FlushCount;
        private long m_CheckpointCount;
        
        private long m_MarshalNTotalTime;
        private long m_SnapshotTotalTime;
        private long m_FlushTotalTime;
        private long m_CheckpointTotalTime;

        #endregion
        
        internal Checkpoint(Tables tables)
        {
            var period = Environment.GetEnvironmentVariable("edb.bucketShift");
            m_SchedPeriod = period == null ? 100 : int.Parse(period);
            m_Tables = tables;
            
            var now = Time.Now;
            m_NextMarshalTime = now + Edb.I.Config.MarshalPeriod;
            m_NextCheckpointTime = now + Edb.I.Config.CheckpointPeriod;
            Edb.I.Executor.Tick(() => Checkpoint0(Time.Now, Edb.I.Config), 0, m_SchedPeriod);
        }

        public void CheckpointNow()
        {
            m_CheckpointNow = true;
            // 用edb的任务接口执行，保证任务不会丢失
            Edb.I.Executor.Execute(() => Checkpoint0(Time.Now, Edb.I.Config));
        }
        
        private void Checkpoint0(long now, Config config)
        {
            lock (this)
            {
                try
                {
                    if (config.MarshalPeriod >= 0 && m_NextMarshalTime <= now)
                    {
                        m_NextMarshalTime = now + config.MarshalPeriod;
                        var start = Time.Now;
                        long countMarshalN = 0;
                        foreach (var storage in m_Tables.Storages)
                        {
                            countMarshalN += storage.MarshalN();
                        }
                        Interlocked.Add(ref m_MarshalNCount, countMarshalN);
                        Interlocked.Add(ref m_MarshalNTotalTime, Time.Now - start);
                        Log.I.Info($"marshalN=*/{countMarshalN}");
                    }
                    var checkpointPeriod = config.CheckpointPeriod;
                    if (checkpointPeriod >= 0 && (m_CheckpointNow || m_NextCheckpointTime <= now))
                    {
                        m_CheckpointNow = false;
                        m_NextCheckpointTime = now + checkpointPeriod;
                        Checkpoint0(config);
                    }
                }
                catch (Exception e)
                {
                    Log.I.Fatal(e);
                    Environment.Exit(-1);
                }   
            }
        }

        private void Checkpoint0(Config config)
        {
            lock (this)
            {
                Log.I.Info("--------------- begin checkpoint ---------------");
                var storages = m_Tables.Storages;
                if (config.MarshalN < 1)
                    Log.I.Warn("marshalN disabled");
            
                // marshalN
                m_Elapse.Reset();
                for (var i = 1; i <= config.MarshalN; i++)
                {
                    long countMarshalN = 0;
                    foreach (var storage in storages)
                    {
                        countMarshalN += storage.MarshalN();
                    }
                    Interlocked.Add(ref m_MarshalNCount, countMarshalN);
                    Log.I.Info($"marshalN={i}/{countMarshalN}");
                }
                Interlocked.Add(ref m_MarshalNTotalTime, m_Elapse.ElapsedAndReset());
                
                // marshal0 + snapshot
                long countSnapshot = 0;
                long countMarshal0 = 0;
                m_Tables.FlushLock.WLock();
                m_Elapse.Reset();
                try
                {
                    foreach (var storage in storages)
                    {
                        countMarshal0 += storage.Marshal0();
                        countSnapshot += storage.Snapshot();
                    }
                }
                finally
                {
                    m_Tables.FlushLock.WUnlock();
                }
                var snapshotTime = m_Elapse.ElapsedAndReset();
                if (snapshotTime > config.SnapshotFatalTime)
                    Log.I.Fatal($"snapshot time={snapshotTime} snapshot={countSnapshot} marshal0={countMarshal0}");
                Interlocked.Add(ref m_SnapshotCount, countSnapshot);
                Interlocked.Add(ref m_Marshal0Count, countMarshal0);
                Interlocked.Add(ref m_SnapshotTotalTime, snapshotTime);
                Log.I.Info($"snapshot={countSnapshot} marshal0={countMarshal0}");
                
                // flush
                long countFlush = 0;
                while (true)
                {
                    var success = false;
                    m_Tables.Logger!.BeforeFlush();
                    try
                    {
                        foreach (var storage in storages)
                            countFlush += storage.FlushAsync();
                        success = true;
                        if (countFlush > 0)
                        {
                            m_Tables.Logger!.AfterFlush(success);
                            foreach (var storage in storages)
                                storage.Cleanup();
                        }
                    } finally
                    {
                        m_Tables.Logger!.AfterFlush(success);
                    }

                    break;
                }
                
                Interlocked.Add(ref m_FlushTotalTime, m_Elapse.ElapsedAndReset());
                Log.I.Info($"flush={countFlush}");
                if (countFlush > 0)
                {
                    Interlocked.Add(ref m_FlushCount, countFlush);
                    Interlocked.Add(ref m_CheckpointTotalTime, m_Elapse.ElapsedAndReset());
                    Log.I.Info("checkpoint");
                }

                Interlocked.Increment(ref m_CheckpointCount);
                Log.I.Info("--------------- end checkpoint ---------------");
            }
        }

        internal void Cleanup()
        {
            Log.I.Info("final checkpoint begin");
            Checkpoint0(Edb.I.Config);
            Log.I.Info("final checkpoint end");
        }
    }
}