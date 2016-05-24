using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    public class DisposableObject : IDisposable
    {
        private bool disposed = false;

        ~DisposableObject()
        {
            Dispose();
        }

        public DisposableObject()
        {

        }

        public void Dispose()
        {
            if(!disposed)
            {
                disposed = true;
                Disposed(disposed);
            }
        }

        public virtual void Disposed(bool dispose)
        {
            if(dispose)
            {
                GC.SuppressFinalize(this);
            }
        }
    }
}
