using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFLua
{
    public class LuaRef: IDisposable
    {
        private bool isdisposed;
        public int reference;
        protected Lua _lua;

        protected bool TryGet(out Lua lua)
        {
            if (_lua.L == 0)
            {
                lua = null;
                return false;
            }

            lua = _lua;
            return true;
        }

        protected LuaRef(int r, Lua lua)
        {
            _lua = lua;
            reference = r;
        }

        ~LuaRef()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void DisposeLuaReference(bool finalized)
        {
            if (_lua == null)
                return;
            Lua lua;
            if (!TryGet(out lua))
                return;

            lua.DisposeRef(reference, finalized);
        }
        public virtual void Dispose(bool disposeManagedResources)
        {
            if (isdisposed)
                return;

            bool finalized = !disposeManagedResources;

            if (reference != 0)
            {
                DisposeLuaReference(finalized);
            }

            _lua = null;
            isdisposed = true;
        }

        public override bool Equals(object o)
        {
            var r = o as LuaRef;
            if (r == null)
                return false;

            Lua lua;
            if (!TryGet(out lua))
                return false;

            return lua.CompareRef(r.reference, reference);
        }

        public override int GetHashCode()
        {
            return reference;
        }
    }
}
