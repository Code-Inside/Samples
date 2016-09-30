:: https://www.jayway.com/2014/09/03/creating-self-signed-certificates-with-makecert-exe-for-development/
:: https://msdn.microsoft.com/en-us/windows/uwp/porting/desktop-to-uwp-manual-conversion?f=255&MSPPError=-2147217396
:: http://stackoverflow.com/questions/6307886/how-to-create-pfx-file-from-certificate-and-private-key
"C:\Program Files (x86)\Windows Kits\10\bin\x64\makecert.exe" -r -h 0 -n "CN=Robert" -eku 1.3.6.1.5.5.7.3.3 -pe -sv App.pvk App.cer 

"C:\Program Files (x86)\Windows Kits\10\bin\x64\pvk2pfx.exe" -pvk App.pvk -spc App.cer -pfx App.pfx -po apptest
