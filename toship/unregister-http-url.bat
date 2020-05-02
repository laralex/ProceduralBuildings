@echo off
netsh http delete urlacl url="http://+:64046/"
echo Done!
pause