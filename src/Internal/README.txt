Theese classes and structs are only used by the engine internally and aren't part of the user
interface of the libary.

You may notice that a few structs ends with "Data". These structs represents unmanaged data blocks
that is managed by it's respective classes. I chose to design it like it is because I want to be
able to share all data structure with unmanaged native code segments (PInvoking dlls) which can
synthesise or post-process faster than original code in CLR.