﻿using System;
using Object = UnityEngine.Object;

namespace SerializableCallback
{
    [Serializable]
    public struct Arg {
        public enum ArgType { Unsupported, Bool, Int, Float, String, Object }
        public bool boolValue;
        public int intValue;
        public float floatValue;
        public string stringValue;
        public Object objectValue;
        public ArgType argType;
        public string _typeName;

        public object GetValue() {
            return GetValue(argType);
        }

        public object GetValue(ArgType type) {
            switch (type) {
                case ArgType.Bool:
                    return boolValue;
                case ArgType.Int:
                    return intValue;
                case ArgType.Float:
                    return floatValue;
                case ArgType.String:
                    return stringValue;
                case ArgType.Object:
                    return objectValue;
                default:
                    return null;
            }
        }

        public void SetValue(object value)
        {
            var type = value.GetType();
            var argumentType = FromRealType(type);
            
            _typeName = type.AssemblyQualifiedName;
            argType = argumentType;
            switch (argumentType) {
                case ArgType.Bool:
                    boolValue = (bool) value;
                    break;
                case ArgType.Int:
                    intValue = (int) value;
                    break;
                case ArgType.Float:
                    floatValue = (float) value;
                    break;
                case ArgType.String:
                    stringValue = (string) value;
                    break;
                case ArgType.Object:
                    objectValue = (Object) value;
                    break;
                case ArgType.Unsupported:
                default:
                    break;
            }
        }

        public static Arg FromValue(object value)
        {
            var arg = new Arg();
            arg.SetValue(value);
            return arg;
        }

        public static Type RealType(ArgType type) {
            switch (type) {
                case ArgType.Bool:
                    return typeof(bool);
                case ArgType.Int:
                    return typeof(int);
                case ArgType.Float:
                    return typeof(float);
                case ArgType.String:
                    return typeof(string);
                case ArgType.Object:
                    return typeof(Object);
                default:
                    return null;
            }
        }

        public static ArgType FromRealType(Type type) {
            if (type == typeof(bool)) return ArgType.Bool;
            if (type == typeof(int)) return ArgType.Int;
            if (type == typeof(float)) return ArgType.Float;
            if (type == typeof(String)) return ArgType.String;
            if (typeof(Object).IsAssignableFrom(type)) return ArgType.Object;
            return ArgType.Unsupported;
        }

        public static bool IsSupported(Type type) {
            return FromRealType(type) != ArgType.Unsupported;
        }
    }
}