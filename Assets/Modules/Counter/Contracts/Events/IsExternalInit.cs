namespace System.Runtime.CompilerServices {
    // Unity does not have this type, and the compiler needs it for records and init.
    // Each assembly with a record has its own copy.
    static class IsExternalInit { }
}
