
param(
  [Parameter(Mandatory=$true)]
  [string]$version)

Function Show-AsteriskBanner
{
  param($text)

  $outerStars = "*";
  $spaceBetweenTextAndStars = 10

  #($spaceBetweenTextAndStars * 2) = both sides
  for($i = 0; $i -lt $text.Length +($spaceBetweenTextAndStars * 2); $i++){
    $outerStars += "*"
  }
  $outerStars += "*";
  
  Write-Host $outerStars

  $innerStars = "*"
  
  #($spaceBetweenTextAndStars * 2) = both sides
  for($i = 0; $i -lt $text.Length + ($spaceBetweenTextAndStars * 2); $i++){
    $innerStars += " "
  }
  $innerStars += "*"

  Write-Host "$innerStars"
  
  $finalText = "*";
  
  for($i = 0; $i -lt $spaceBetweenTextAndStars; $i++){
    $finalText += " "
  }

  $finalText += $text
  
  for($i = 0; $i -lt $spaceBetweenTextAndStars; $i++){
    $finalText += " "
  }
  $finalText += "*"

  Write-Host "$finalText"
  
  Write-Host "$innerStars"
  Write-Host $outerStars
}

$repoRoot = "$PSScriptRoot\.."
$projectName = "dotPeekser.Solr.ManagedSchema"
$managedSchemaProj = "$repoRoot\src\dotPeekser.Solr.ManagedSchema\$projectName.csproj"
$nuspecFilePath = "$repoRoot\nuspec-packages"
$packagePath = "$nuspecFilePath\$projectName.$version.nupkg"

Show-AsteriskBanner "Build project '$projectName'"

$buildResult = msbuild.exe -t:build -restore $managedSchemaProj -p:Configuration=Release

if (! $?) {
  Write-Error "MS Build failed."
  Write-Host $buildResult
  return
}

Write-Host ""
Write-Host "> Build successful"
Write-Host ""
Write-Host ""

Show-AsteriskBanner "Pack project '$projectName'"

$packResult = msbuild.exe -t:pack $managedSchemaProj -p:Configuration=Release -p:NoBuild=true -p:Version=$version -p:PackageOutputPath=$nuspecFilePath

if (! $?) {
  Write-Error "MS Pack failed."
  Write-Host $packResult
  return
}

Write-Host ""
Write-Host "> Packing successful. Path: $packagePath"
Write-Host ""
Write-Host ""

Show-AsteriskBanner "Pushing package '$projectName'"
