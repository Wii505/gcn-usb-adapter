echo off
cls

cd /d %programfiles%\vjoy

echo Removing Controller Ports...
vJoyConfig.exe -d 1 2 3 4