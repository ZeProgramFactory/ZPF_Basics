
Rem  *** compiles ZPF_DBSQL *** 
dotnet build Sources\ZPF_Basics.csproj -c=Release
move Sources\bin\Release\*.nupkg .

pause
Rem  *** publish all ***
dotnet nuget push *.nupkg -k oy2bbli2ofzblzukwr4afcrcttpd2t7evkanccpnkawltu -s https://api.nuget.org/v3/index.json
rem dotnet nuget push *.nupkg MossIsTheBoss -s http://nugets.azurewebsites.net/nuget 
del *.nupkg
pause
