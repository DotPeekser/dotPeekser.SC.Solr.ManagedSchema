param(
  [Parameter(Mandatory=$true)]
  [string]$version,
  [Parameter(Mandatory=$true)]
  [string]$apiKey)

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
$nuget = "$repoRoot\tools\nuget.exe"
$projectName = "dotPeekser.SC.Solr.ManagedSchema"
$nuspecFilePath = "$repoRoot\nuspec-packages"
$packagePath = "$nuspecFilePath\$projectName.$version.nupkg"

Show-AsteriskBanner "Pushing package '$projectName' in version $version"

&$nuget push "$packagePath" -ApiKey $apiKey -ConfigFile "$repoRoot\nuget.config" -Source "https://api.nuget.org/v3/index.json"