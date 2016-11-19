using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace wUnit.Util.SyncContext {
  public class ConsoleSynchronizationContext : SynchronizationContext, IDisposable {
    private static int _started = 0;
    private static readonly object _contextLock = new object();
    private static readonly Thread _internalThread;
    private static IList<Action> _actionCollection = new List<Action>();
    private static bool _running = false;

    [STAThread]
    protected static void ActionReader() {
      lock ( _contextLock ) {
        if ( _running ) {
          return;
        }
        Monitor.Pulse( _contextLock );
      }
      _running = true;
      while ( _running ) {
        lock ( _contextLock ) {
          if ( _actionCollection.Count > 0 && _running ) {
            Action action = null;
            action = _actionCollection[0];
            _actionCollection.RemoveAt( 0 );
            if ( action != null ) {
              action();
            }
            Monitor.Pulse( _contextLock );
          }
        }
      }
    }

    public override void Post( SendOrPostCallback d, object state ) {
      lock ( _contextLock ) {
        _actionCollection.Add( () => {
          base.Post( d, state );
        } );
        Monitor.Pulse( _contextLock );
      }
    }

    public override void Send( SendOrPostCallback d, object state ) {
      lock ( _contextLock ) {
        _actionCollection.Add( () => {
          base.Send( d, state );
        } );
        Monitor.Pulse( _contextLock );
      }
    }

    static ConsoleSynchronizationContext() {
      _internalThread = new Thread( ActionReader );
      _internalThread.Start();
    }

    public ConsoleSynchronizationContext() {
      if ( _started == 0 ) {
        SynchronizationContext.SetSynchronizationContext( this );
      }
      lock ( _contextLock ) {
        _started++;
        Monitor.Pulse( _contextLock );
      }
    }

    protected virtual void Dispose( bool disposing ) {
      lock ( _contextLock ) {
        _started--;
        Monitor.Pulse( _contextLock );
      }
      if ( _started == 0 ) {
        _running = false;
        if ( disposing ) {
          _internalThread.Abort();
          _internalThread.Join();
          lock ( _contextLock ) {
            int count = _actionCollection.Count;
            for (int i = 0; i < count; i++) {
              _actionCollection[i]();
            }
            _actionCollection.Clear();
            Monitor.Pulse( _contextLock );
          }
          SynchronizationContext.SetSynchronizationContext( null );
        }
      }
    }

    public void Dispose() {
      Dispose( true );
    }

    ~ConsoleSynchronizationContext () {
      Dispose( false );
      GC.SuppressFinalize( true );
    }
  }
}

