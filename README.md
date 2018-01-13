Jazmine Blockchain

See https://jazmine.io/ for info.

This is JazminneGUI. The front-end GUI for Jazmined and JazmineWalletd, configured to build using Visual Studio C++

NOTE: mining is temporarily disabled in this version, until some code issues are resolved. If you 
want to mine jazmine, please use the Linux version.

Also multiple addresses are not yet supported in this version. If you want multiple addresses please use the Linux version.

This is not yet production quality, there are some issues. The software functions as intended but use with care.

This software starts three server processes listening on localhost (127.0.0.1). The ports are psuedo-randomized, 
the software will look for an open port to use for each service. This minimizes issues with security.


```
NOTE: This software will start an RPC/HTTP server listening on your computer on localhost (127.0.0.1). 
It is not accessible to the outside, HOWEVER PLEASE NOTE that a specially scripted javascript code 
on a remote web site could possibly manipulate the rpc/http server running on your machine. 
This means people could steal your coins. If you are running througn the JazmineGui it randomizes
the port used, but it is still a bad idea to have a wallet open while surfing the www.
```

At the moment do not "surf the web" while running this software, it's potentially an issue. 

Will have authorization added to future release.



To BUILD:

Open the solution file in Visual Studio.

This software requires CefSharp package and QRcode packages.

Example: 

```
Install-Package QRCoder -Version 1.3.2 
Install-Package CefSharp.WinForms -Version 57.0.0 
```

You will need to copy the jazmine-php directory to your exe/build directory.

You will also need a version of php for windows, extract the zip file and put in the exe/build directory. 

Copy the php.ini file in the jazmine-php directory to the php directory.




