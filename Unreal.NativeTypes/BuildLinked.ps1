# Set Working Directory
Split-Path $MyInvocation.MyCommand.Path | Push-Location
[Environment]::CurrentDirectory = $PWD

Remove-Item "$env:RELOADEDIIMODS/Unreal.NativeTypes/*" -Force -Recurse
dotnet publish "./Unreal.NativeTypes.csproj" -c Release -o "$env:RELOADEDIIMODS/Unreal.NativeTypes" /p:OutputPath="./bin/Release" /p:ReloadedILLink="true"

# Restore Working Directory
Pop-Location