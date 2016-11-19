using System;
using System.Threading;
using wUnit.Util.SyncContext;

namespace wUnit.Util {
  public abstract class AbstractSynchronizationClass: IDisposable {
    private SynchronizationContext _context;
    private readonly object _contextLock = new object();
    private readonly object _threadLock = new object();

    protected void Fire( SendOrPostCallback sop, object userState = null ) {
      EnsureCreatedContext();
      lock ( _contextLock ) {
        _context.Send( sop, userState );
      }
    }

    protected void FireDelegate<T>( EventHandler<T> handler, T args ) {
      var local = handler;
      if ( local != null ) {
        lock ( _threadLock ) {
          Fire( (s ) => {
            local.Invoke( this, args );
          } );
        }
      }
    }

    private void EnsureCreatedContext() {
      if ( _context != null ) {
        return;
      }
      lock ( _contextLock ) {
        if ( _context != null ) {
          return;
        }
        _context = SynchronizationContext.Current ?? new ConsoleSynchronizationContext();
      }
    }

    protected void FireAndWait( SendOrPostCallback sop, object userState = null ) {
      EnsureCreatedContext();
      lock ( _contextLock ) {
        _context.Send( sop, userState );
      }
    }

    protected void SetContext( SynchronizationContext context ) {
      lock ( _contextLock ) {
        _context = context;
      }
    }

    protected virtual void Dispose( bool disposing ) {
      if ( disposing ) {
        if ( _context is IDisposable ) {
          ( (IDisposable)_context ).Dispose();
        }
        _context = null;
      }
    }

    public void Dispose() {
      Dispose( true );
    }
  }
}

