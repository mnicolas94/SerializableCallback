namespace SerializableCallback.Samples
{
    public static class TextExtensions
    { 
        public static string ExtensionMethod(this Test t, int integer)
        {
            return $"this is a extension method with argument: {t.ParamFunction(integer)}";
        }
    }
}