updateversioninfo -v=auto -n=.\Sources\ZPF_Basics.csproj

dotnet restore Sources\ZPF_Basics.csproj 

..\..\_Units_\_Tools_\Nuget pack .\Sources\ZPF_Basics.csproj -build -Properties Configuration=Release

pause 

Rem  *** publish all ***


