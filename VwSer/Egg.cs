using System;
using System.Collections.Generic;
using VwSyncSever;

namespace VwSer
{
    class Egg
    {
        internal bool wasChecked, isMapped;

        internal int lastExecTimeMs;

        internal string
            dir,
            usr,
            pas;

        internal Exception err; // ultima eroare intalnita pt acest ou

        internal Orchestrator orc;
    }

    class EggArray
    {
        private List<Egg> eggs;
        public EggArray()
        {
            eggs = new List<Egg>();
        }

        public Egg GetEgg(string path)
        {
            if (eggs != null)
            {
                foreach (Egg seg in eggs)
                {
                    if (seg.dir.Equals(path)) return seg;
                }
            }
            return null;
        }

        public void AddEgg(Egg ou)
        {
            eggs.Add(ou);
        }

        internal void Start()
        {
            foreach (Egg seg in eggs)
            {
                seg.wasChecked = false;
            }
            }

        internal void Stop()
        {
            foreach (Egg seg in eggs.ToArray())
            {
                if (!seg.wasChecked) eggs.Remove(seg);
            }
        }

        public int GetCount()
        {
            return eggs.Count;
        }

        //public void Clear()
        //{
        //    eggs.Clear();
        //}
    }
}
