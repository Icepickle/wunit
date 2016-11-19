using System;
using System.Collections.Generic;
using wUnit.Util;

namespace wUnit.Export {
  public interface IConfigurationResultExporter {
    int Export( IContextRunnerConfiguration context, string fileName );

    string Name { get; }

    string Description { get; }

    Version Version { get; }

    string DefaultFileNamePattern { get; }

    string FileExtension { get; }

    bool TriggerApplicationExitCode { get; }
  }
}

