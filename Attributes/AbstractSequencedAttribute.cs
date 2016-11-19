using System;

namespace wUnit.Attributes {
  /// <summary>
  /// Adds the possilibity to indicate in which order any other marked attributes must run
  /// This can help to indicate which test should run before the other and have some more control
  /// over the fully wUnit framework / unit test cycle
  /// </summary>
  public abstract class AbstractSequencedAttribute : AbstractActivatorAttribute {
    /// <summary>
    /// An integer indicating it's sequence nummer in a chain of similar attributes
    /// Lower SequenceNumber run before any other sequences
    /// Default is default(int)
    /// </summary>
    /// <value>The sequence number.</value>
    public int SequenceNumber { get; protected set; }
  }
}

