using System;
using System.Net.Cache;
using wUnit.Model;
using System.Collections.Generic;
using wUnit.Util.Cache;
using System.Reflection;
using wUnit.Attributes;
using System.Runtime.Remoting;

namespace wUnit.Util.Factory
{
	/// <summary>
	/// The entity factory used to construct new types during testing
	/// </summary>
	public class EntityFactory : IEntityFactory
	{
		private readonly ICacheHelper<Type, object> _cache;

		protected bool CacheEnabled { 
			get { 
				return CacheSettingsHelper<Type, object>.Current.IsEnabled;
			} 
		}

		public object[] FillParamInfo(ParameterInfo[] paramInfo, IDefineTypes typeProvider) {
			if (paramInfo == null || paramInfo.Length == 0) {
				return null;
			}
			var args = new object[paramInfo.Length];

			for (int index = 0, len = paramInfo.Length; index < len; index++) {
				var param = paramInfo [index];
				var valueInject = param.GetCustomAttribute<InjectValueParameterAttribute> ();
				if (valueInject != null && valueInject.IsEnabled) {
					args [index] = valueInject.Value;
					continue;
				}
				var inject = param.GetCustomAttribute<InjectTypeParameterAttribute> ();
				if (inject == null || !inject.IsEnabled) {
					Type definedParameter;
					if (typeProvider != null && typeProvider.DefinedTypes.TryGetValue (param.ParameterType, out definedParameter)) {
						if (definedParameter is Type) {
							args [index] = ConstructType (definedParameter as Type, typeProvider);
						} else {
							args [index] = definedParameter;
						}
						continue;
					}
					if (param.ParameterType.IsInterface) {
						args [index] = null;
					} else if (!param.ParameterType.IsValueType) {
						args[index] = ConstructType(param.ParameterType, typeProvider);
					} else if (param.DefaultValue != null && !(param.DefaultValue is System.DBNull)) {
						args [index] = param.DefaultValue;
					} else if (param.ParameterType.IsEnum) {
						var enumValues = Enum.GetValues (param.ParameterType);
						foreach (var value in enumValues) {
							args [index] = value;
							break;
						}
					} else {
						args [index] = null;
					}
					continue;
				}
				args [index] = ConstructType (inject.Type, typeProvider);
			}
			return args;
		}

		protected virtual object InitializeType(Type type, IDefineTypes typeProvider) {
			var constructor = type.GetConstructor (Type.EmptyTypes);
			if (constructor == null) {
				var constructors = type.GetConstructors (BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
				if (constructors == null || constructors.Length == 0) {
					throw new NotImplementedException (string.Format("No constructor found for {0}", type.FullName));
				}
				constructor = constructors [0];
			}
			//return typeProvider.ContextRunnerConfiguration.ContextAppDomain.CreateInstance(type.Assembly.FullName, type.FullName, false, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, FillParamInfo(constructor.GetParameters(), typeProvider), null, null);
			return Activator.CreateInstance (type, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, FillParamInfo(constructor.GetParameters(), typeProvider), null);
		}

		public object ConstructType (Type t, IDefineTypes typeProvider)
		{
			object result;
			if (CacheEnabled && _cache.TryGetValue(t, out result)) {
				return result;
			}
			result = InitializeType (t, typeProvider);
			if (result is ObjectHandle) {
				result = Convert.ChangeType (((ObjectHandle)result).Unwrap (), t);
			}

			if (CacheEnabled) {
				_cache.Set (t, result);
			}
			return result;
		}

		public T ConstructType<T> (IDefineTypes typeProvider)
		{
			return (T)ConstructType (typeof(T), typeProvider);
		}

		private static Lazy<EntityFactory> _factory = new Lazy<EntityFactory>(()=> new EntityFactory());
		public static IEntityFactory Factory {
			get {
				return _factory.Value;
			}
		}

		private EntityFactory() {
			// intended blank
			_cache = CacheSettingsHelper<Type, object>.Current;
		}
	}
}

