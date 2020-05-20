@echo off
netsh http add urlacl url="http://+:64046/" user=%USERNAME%
netsh http add urlacl url="http://+:64047/" user=%USERNAME%
echo Done!
pause