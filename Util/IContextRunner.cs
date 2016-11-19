using System;
using System.ComponentModel;
using System.Reflection;
using wUnit.Model;
using System.Threading;

namespace wUnit.Util {
  public interface IContextRunner : IDisposable {
    event EventHandler<ProgressChangedEventArgs> ProgressChanged;
    event EventHandler<TestErrorOccuredEventArgs> ErrorOccured;
    event EventHandler<RunWorkerCompletedEventArgs> Completed;

    void RunAsync( IContextRunnerConfiguration configuration, ManualResetEvent mre );
  }
}

