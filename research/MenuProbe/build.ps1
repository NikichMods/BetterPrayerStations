# SPDX-License-Identifier: MPL-2.0
param([Parameter(Mandatory=$true)][string]$GameRoot,[Parameter(Mandatory=$true)][string]$DotnetRoot,[Parameter(Mandatory=$true)][string]$ExpectedSource,[switch]$CheckOnly)
$ErrorActionPreference='Stop'
$repo=(Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$head=(& git -C $repo rev-parse HEAD).Trim()
if($head -ne $ExpectedSource){throw 'Unexpected source'}
if(!$CheckOnly -and (& git -C $repo status --porcelain --untracked-files=normal)){throw 'Numbered build requires clean source'}
$version=[regex]::Match([IO.File]::ReadAllText((Join-Path $PSScriptRoot 'Plugin.cs')),'Version="([0-9]+\.[0-9]+\.[0-9]+)"').Groups[1].Value
if(!$version){throw 'Missing version owner'}
$managed=Join-Path $GameRoot 'Graveyard Keeper_Data\Managed'
$core=Join-Path $GameRoot 'BepInEx\core'
$refs=@('mscorlib.dll','System.dll','System.Core.dll','Assembly-CSharp.dll','Assembly-CSharp-firstpass.dll','UnityEngine.dll','UnityEngine.CoreModule.dll','UnityEngine.Physics2DModule.dll','UnityEngine.IMGUIModule.dll','UnityEngine.InputLegacyModule.dll') | ForEach-Object {Join-Path $managed $_}
$refs+=@('BepInEx.dll','0Harmony.dll') | ForEach-Object {Join-Path $core $_}
$compiler=Join-Path $DotnetRoot 'sdk\6.0.406\Roslyn\bincore\csc.dll'
$out=Join-Path $repo ('artifacts\MenuProbe-'+$version)
New-Item -ItemType Directory -Path $out -Force | Out-Null
$dll=Join-Path $out $(if($CheckOnly){'compile-check.dll'}else{'BetterPrayerStationsMenuProbe-'+$version+'.dll'})
if(!$CheckOnly -and (Test-Path -LiteralPath $dll)){throw 'Immutable artifact already exists'}
$common=@('/nologo','/nostdlib+','/noconfig','/langversion:7.3','/optimize+','/deterministic+','/warnaserror+')
$common+=@($refs | ForEach-Object {if(!(Test-Path -LiteralPath $_)){throw ('Missing reference '+$_)};'/reference:'+$_})
& (Join-Path $DotnetRoot 'dotnet.exe') $compiler @common '/target:library' ('/out:'+$dll) (Join-Path $PSScriptRoot 'Plugin.cs') (Join-Path $PSScriptRoot 'BeamState.cs') (Join-Path $PSScriptRoot 'SkullProjection.cs') (Join-Path $PSScriptRoot 'FieldGeometry.cs') (Join-Path $PSScriptRoot 'PhysicalSnapshot.cs') (Join-Path $PSScriptRoot '../FaithProbe/GridFootprint.cs')
if($LASTEXITCODE -ne 0){throw 'Probe compilation failed'}
if([Reflection.AssemblyName]::GetAssemblyName($dll).Version.ToString() -ne ($version+'.0') -or [Diagnostics.FileVersionInfo]::GetVersionInfo($dll).FileVersion -ne ($version+'.0')){throw 'Version mismatch'}
$test=Join-Path $out 'MenuTests.exe'
& (Join-Path $DotnetRoot 'dotnet.exe') $compiler @common '/target:exe' ('/out:'+$test) (Join-Path $PSScriptRoot 'MenuTests.cs') (Join-Path $PSScriptRoot 'EffectTests.cs')
if($LASTEXITCODE -ne 0){throw 'Test compilation failed'}
& $test $managed $core $dll
if($LASTEXITCODE -ne 0){throw 'Menu tests failed'}
if($CheckOnly){Write-Output 'Development compile and menu gates passed; not a handoff artifact';return}
$manifest=[ordered]@{kind='research-only session field and skull-effect probe';version=$version;source=$head;compiler='SDK6.0.406 Roslyn';file=(Split-Path $dll -Leaf);sha256=(Get-FileHash -LiteralPath $dll).Hash.ToLowerInvariant();references=@($refs | ForEach-Object {[ordered]@{name=(Split-Path $_ -Leaf);sha256=(Get-FileHash -LiteralPath $_).Hash.ToLowerInvariant()}})}
$manifest | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $out 'build-identity.json') -Encoding UTF8
Write-Output ('Built '+$dll+' SHA256='+$manifest.sha256+' source='+$head)
