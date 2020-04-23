@echo off

call "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\VC\Auxiliary\Build\vcvars64.bat"

echo.
echo CLEARING "ship"

if not exist "ship" mkdir ship
del /Q ship\*.*

echo. 
echo BUILDING  
echo.

call aux-rebuild.bat

echo.
echo COPYING
echo.

call aux-copy.bat

echo.
echo Done!
echo.
pause
exit