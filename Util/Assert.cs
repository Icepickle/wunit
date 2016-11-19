using System;

namespace wUnit.Util {

  public static class Assert {
    public static void IsNull<T>( T argument, string message = null ) {
      if ( argument != null && !object.Equals( argument, default(T) ) ) {
        throw new ArgumentNullException( message ?? typeof(T).FullName + " was not equal to null!" );
      }
    }

    public static void IsNotNull<T>( T argument, string message = "" ) {
      if ( argument == null || object.Equals( argument, default(T) ) ) {
        throw new ArgumentNullException( message ?? typeof(T).FullName + " was defined empty!" );
      }
    }

    public static void IsEqual<T>( T source, T destination, string message = "" ) {
      if ( !object.Equals( source, destination ) && !object.ReferenceEquals( source, destination ) ) {
        throw new Exception( message ?? "Types do not match!" );
      }
    }

    public static void IsOneOf<T>( T source, T[] values, string message = "" ) {
      foreach (var value in values) {
        if ( object.Equals( source, value ) ) {
          return;
        }
        if ( !( value is IComparable ) ) {
          continue;
        }
        if ( ( (IComparable)value ).CompareTo( source ) == 0 ) {
          return;
        }
      }
      throw new ArgumentOutOfRangeException( message ?? "No matching value found for " + source );
    }

    public static void IsNotEqual<T>( T source, T destination, string message = "" ) {
      if ( object.Equals( source, destination ) || object.ReferenceEquals( source, destination ) ) {
        throw new ArithmeticException( message ?? "Types match when they should not!" );
      }
    }

    public static void IsGreaterThan<T>( T source, T destination, string message = "" )
			where T: IComparable<T> {
      Assert.IsNotNull( source );
      Assert.IsNotNull( destination );
      Assert.IsOneOf( source.CompareTo( destination ), new [] { 1 }, message );
    }

    public static void IsGreaterThanOrEqual<T>( T source, T destination, string message = "" ) 			
			where T: IComparable<T> {
      Assert.IsNotNull( source );
      Assert.IsNotNull( destination );
      Assert.IsOneOf( source.CompareTo( destination ), new [] { 0, 1 }, message );
    }

    public static void IsLowerThan<T>( T source, T destination, string message = "" )
			where T: IComparable<T> {
      Assert.IsNotNull( source );
      Assert.IsNotNull( destination );
      Assert.IsOneOf( source.CompareTo( destination ), new [] { -1 }, message );
    }

    public static void IsLowerThanOrEqual<T>( T source, T destination, string message = "" ) 
			where T: IComparable<T> {
      Assert.IsNotNull( source );
      Assert.IsNotNull( destination );
      Assert.IsOneOf( source.CompareTo( destination ), new [] { -1, 0 }, message );
    }

    public static void IsOfType<T>( object obj, string message = "" ) {
      IsOfType( obj, typeof(T) );
    }

    public static void IsOfType( object obj, Type type, string message = "" ) {
      Assert.IsNotNull( obj, message );
      Assert.IsNotNull( type, message );
      if ( !type.IsInstanceOfType( obj ) ) {
        throw new InvalidCastException( message ?? "type of object " + obj.GetType().FullName + " is not the same as " + type.FullName );
      }
    }
  }
}

