@echo off
netsh http add urlacl url="http://+:64046/" user=%USERNAME%
echo Done!
pause