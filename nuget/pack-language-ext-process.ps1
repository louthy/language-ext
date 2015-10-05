$version = [System.Reflection.Assembly]::LoadFile(".\lib\LanguageExt.Process.dll").GetName().Version
$versionStr = "{0}.{1}.{2}" -f ($version.Major, $version.Minor, $version.Build)

Write-Host "Setting .nuspec version tag to $versionStr"

$content = (Get-Content .\nuget\LanguageExt.Process.nuspec) 
$content = $content -replace '\$version\$',$versionStr

$content | Out-File .\nuget\LanguageExt.Process.compiled.nuspec

& $root\NuGet\NuGet.exe pack .\nuget\LanguageExt.Process.compiled.nuspec
