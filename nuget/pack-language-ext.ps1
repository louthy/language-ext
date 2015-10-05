$lib   = (split-path -parent $MyInvocation.MyCommand.Definition) + '\..\lib'
$nuget = (split-path -parent $MyInvocation.MyCommand.Definition)

$version = [System.Reflection.Assembly]::LoadFile("$lib\LanguageExt.dll").GetName().Version
$versionStr = "{0}.{1}.{2}" -f ($version.Major, $version.Minor, $version.Build)

Write-Host "Setting .nuspec version tag to $versionStr"

$content = (Get-Content $nuget\LanguageExt.nuspec) 
$content = $content -replace '\$version\$',$versionStr

$content | Out-File $nuget\LanguageExt.compiled.nuspec

& $nuget\nuget.exe pack $nuget\LanguageExt.compiled.nuspec
