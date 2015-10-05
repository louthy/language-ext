$root = (split-path -parent $MyInvocation.MyCommand.Definition) + '\..'
$version = [System.Reflection.Assembly]::LoadFile("$root\lib\LanguageExt.Process.Redis.dll").GetName().Version
$versionStr = "{0}.{1}.{2}" -f ($version.Major, $version.Minor, $version.Build)

Write-Host "Setting .nuspec version tag to $versionStr"

$content = (Get-Content $root\nuget\LanguageExt.Process.Redis.nuspec) 
$content = $content -replace '\$version\$',$versionStr

$content | Out-File $root\nuget\LanguageExt.Process.Redis.compiled.nuspec

& $root\NuGet\NuGet.exe pack $root\nuget\LanguageExt.Process.Redis.compiled.nuspec
