# Tftp.Net
This is a .NET/C# library that allows you to easily integrate a TFTP Client or TFTP Server in your own C# applications. 
If you're looking for a fully-fledged GUI client, you should probably look into other projects. However, if you're looking for code that allows you to implement your own TFTP client/server in only a few lines of C# code, you've come to the right place.

### Download and Building:
Visual Studio users can simply obtain the library from NuGet.
Alternatively, downloading the source and building it in Visual Studio 2013/2015 should work without problems. Contact me if you're having any issues.

### Features:
At the moment the library features:
- Complete TFTP protocol implementation (as defined in RFC 1350, RFC 2347 and RFC 2349)
- TFTP client components 
- TFTP server components 
- Unit-Tested code using NUnit
- Sample TFTP server
- Sample TFTP client
- *New:* Now supports TFTP timeout interval option (RFC 2349).
- *New:* Now supports TFTP transfer size option (RFC 2349).
- *New:* Now supports TFTP option extension (RFC 2347).
- *New:* Now supports TFTP block size option (RFC 2348).
- *New:* Now supports configurable block counter wrap around to zero/one.
