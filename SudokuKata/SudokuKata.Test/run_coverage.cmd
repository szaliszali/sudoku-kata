@echo off
pushd %~dp0
rmdir /q /s TestResults
rmdir /q /s coveragereport
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:TestResults\*\* -targetdir:coveragereport -reporttypes:html
start coveragereport\index.html
popd
