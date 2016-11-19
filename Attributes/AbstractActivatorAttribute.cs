using System;

namespace wUnit.Attributes {
  /// <summary>
  /// A class ment as a base for any of the available options for the wUnit framework
  /// It offers the possiblity to get/set an IsEnabled state
  /// </summary>
  public abstract class AbstractActivatorAttribute : Attribute {
    private bool enabled = true;

    /// <summary>
    /// Indicates wether or not this Attribute should be treated as enabled or not, default is enabled
    /// </summary>
    /// <value><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</value>
    public bool IsEnabled { 
      get {
        return enabled;
      }
      protected set {
        enabled = value;
      }
    }
  }
}

