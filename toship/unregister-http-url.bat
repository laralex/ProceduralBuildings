@echo off
netsh http delete urlacl url="http://+:64046/"
netsh http delete urlacl url="http://+:64047/"
echo Done!
pause