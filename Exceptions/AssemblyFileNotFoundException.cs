using System;
using System.IO;
using System.Reflection;

namespace wUnit.Exceptions {
  public class AssemblyFileNotFoundException : Exception {
    public Assembly RequestingAssembly { get; private set; }
    public string FileName { get; private set; }
    public string CurrentDirectory { get; private set; }

    public AssemblyFileNotFoundException(FileNotFoundException fne, Assembly assembly) {
      FileName = fne.FileName;
      RequestingAssembly = assembly;
      // todo: maybe move this to the constructor
      CurrentDirectory = Environment.CurrentDirectory;
    }
  }
}
