# SPDX-License-Identifier: MPL-2.0
param(
 [Parameter(Mandatory=$true)][string]$GameRoot,
 [Parameter(Mandatory=$true)][string]$DotnetRoot,
 [Parameter(Mandatory=$true)][string]$ExpectedSource,
 [switch]$CheckOnly
)
$ErrorActionPreference='Stop'
$repoRoot=(Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$head=(& git -C $repoRoot rev-parse HEAD).Trim()
if ($head -ne $ExpectedSource) {throw 'Unexpected source commit'}
if (!$CheckOnly -and (& git -C $repoRoot status --porcelain --untracked-files=normal)) {throw 'Handoff source tree must be clean'}
$managed=Join-Path $GameRoot 'Graveyard Keeper_Data\Managed'
$core=Join-Path $GameRoot 'BepInEx\core'
$compiler=Join-Path $DotnetRoot 'sdk\6.0.406\Roslyn\bincore\csc.dll'
$versionMatch=[regex]::Match([IO.File]::ReadAllText((Join-Path $PSScriptRoot 'Plugin.cs')), 'public const string Version = "([0-9]+\.[0-9]+\.[0-9]+)";')
if (!$versionMatch.Success) {throw 'Missing canonical version'}
$version=$versionMatch.Groups[1].Value
$outDir=Join-Path $repoRoot ('artifacts\FaithProbe-'+$version)
New-Item -ItemType Directory -Path $outDir -Force | Out-Null
$output=Join-Path $outDir $(if ($CheckOnly) {'compile-check.dll'} else {'BetterPrayerStationsFaithProbe-'+$version+'.dll'})
if (!$CheckOnly -and (Test-Path -LiteralPath $output)) {throw 'Immutable numbered artifact already exists'}
$refs=@('mscorlib.dll','System.dll','System.Core.dll','Assembly-CSharp.dll','Assembly-CSharp-firstpass.dll','UnityEngine.dll','UnityEngine.CoreModule.dll','UnityEngine.Physics2DModule.dll','UnityEngine.IMGUIModule.dll','UnityEngine.InputLegacyModule.dll') | ForEach-Object {Join-Path $managed $_}
$refs += @('BepInEx.dll','0Harmony.dll') | ForEach-Object {Join-Path $core $_}
$compilerArgs=@('/nologo','/target:library','/nostdlib+','/noconfig','/langversion:7.3','/optimize+','/deterministic+','/warnaserror+',('/out:'+$output))
$compilerArgs += $refs | ForEach-Object {if (!(Test-Path -LiteralPath $_)) {throw ('Missing reference: '+$_)}; '/reference:'+$_}
$compilerArgs += @('Plugin.cs','SavedFaithState.cs','PrayerCoverage.cs','FaithPurchase.cs','TwoGraves.cs','StationSet.cs','MovePreviewOwner.cs','RadiusOverlay.cs','StationMaterials.cs','GridFootprint.cs','PreviewFeedback.cs','PreviewBaseline.cs','ContributionMath.cs','PassiveQuality.cs') | ForEach-Object {Join-Path $PSScriptRoot $_}
& (Join-Path $DotnetRoot 'dotnet.exe') $compiler @compilerArgs
if ($LASTEXITCODE -ne 0) {throw 'Compilation failed'}
$asmVersion=[Reflection.AssemblyName]::GetAssemblyName($output).Version.ToString()
if ($asmVersion -ne ($version+'.0') -or [Diagnostics.FileVersionInfo]::GetVersionInfo($output).FileVersion -ne ($version+'.0')) {throw 'Version mismatch'}
$testOutput=Join-Path $outDir 'FaithTests.exe'
$testArgs=@('/nologo','/target:exe','/nostdlib+','/noconfig','/langversion:7.3','/optimize+','/warnaserror+',('/out:'+$testOutput))
$testArgs += $refs | ForEach-Object {'/reference:'+$_}
$testArgs += @('FaithTests.cs','LifecycleTests.cs','PurchaseTests.cs','PreviewTests.cs','PassiveTests.cs') | ForEach-Object {Join-Path $PSScriptRoot $_}
& (Join-Path $DotnetRoot 'dotnet.exe') $compiler @testArgs
if ($LASTEXITCODE -ne 0) {throw 'Test compilation failed'}
& $testOutput $managed $core $output
if ($LASTEXITCODE -ne 0) {throw 'Faith tests failed'}
if ($CheckOnly) {Write-Output 'Development compile/version/faith input/IL checks passed; not a handoff artifact'; return}
$manifest=[ordered]@{kind='research-only faith probe';version=$version;source=$head;compiler='SDK6.0.406 Roslyn';file=(Split-Path $output -Leaf);sha256=(Get-FileHash -LiteralPath $output).Hash.ToLowerInvariant();references=@($refs | ForEach-Object {[ordered]@{name=(Split-Path $_ -Leaf);sha256=(Get-FileHash -LiteralPath $_).Hash.ToLowerInvariant()}})}
$manifest | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $outDir 'build-identity.json') -Encoding utf8
Write-Output ('Built '+$output+' SHA256='+$manifest.sha256+' source='+$head)
