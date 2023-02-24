
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

function Resolve-MsBuild {
	$msb2022 = Resolve-Path "${env:ProgramFiles}\Microsoft Visual Studio\*\*\MSBuild\*\bin\msbuild.exe" -ErrorAction SilentlyContinue
	
	if ($msb2022) {
		Write-Host "Found MSBuild 2022 (or later)."
		Write-Host $msb2022
		
		return $msb2022
	}
	
	$msb2017 = Resolve-Path "${env:ProgramFiles(x86)}\Microsoft Visual Studio\*\*\MSBuild\*\bin\msbuild.exe" -ErrorAction SilentlyContinue
	
	if ($msb2017) {
		Write-Host "Found MSBuild 2017 (or later)."
		Write-Host $msb2017
		
		return $msb2017
	}
	
	$msBuild2015 = "${env:ProgramFiles(x86)}\MSBuild\14.0\bin\msbuild.exe"
	
	if (-not (Test-Path $msBuild2015)) {
		throw 'Could not find MSBuild 2015 or later.'
	}
	
	Write-Host "Found MSBuild 2015."
	Write-Host $msBuild2015
	
	return $msBuild2015
}

$msbuild = Resolve-MsBuild

$repoRoot = "$PSScriptRoot\.."
$projectName = "dotPeekser.SC.Solr.ManagedSchema"
$managedSchemaProj = "$repoRoot\src\$projectName\$projectName.csproj"
$nuspecFilePath = "$repoRoot\nuspec-packages"
$packagePath = "$nuspecFilePath\$projectName.$version.nupkg"

Show-AsteriskBanner "Build project '$projectName'"

$buildResult = & $msbuild -t:build -restore $managedSchemaProj -p:Configuration=Release

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
