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
$refs=@('mscorlib.dll','System.dll','System.Core.dll','Assembly-CSharp.dll','Assembly-CSharp-firstpass.dll','UnityEngine.dll','UnityEngine.CoreModule.dll','UnityEngine.Physics2DModule.dll','UnityEngine.IMGUIModule.dll','UnityEngine.InputLegacyModule.dll','UnityEngine.ImageConversionModule.dll') | ForEach-Object {Join-Path $managed $_}
$refs+=@('BepInEx.dll','0Harmony.dll') | ForEach-Object {Join-Path $core $_}
$compiler=Join-Path $DotnetRoot 'sdk\6.0.406\Roslyn\bincore\csc.dll'
$out=Join-Path $repo ('artifacts\MenuProbe-'+$version)
New-Item -ItemType Directory -Path $out -Force | Out-Null
$dll=Join-Path $out $(if($CheckOnly){'compile-check.dll'}else{'BetterPrayerStationsMenuProbe-'+$version+'.dll'})
if(!$CheckOnly -and (Test-Path -LiteralPath $dll)){throw 'Immutable artifact already exists'}
$common=@('/nologo','/nostdlib+','/noconfig','/langversion:7.3','/optimize+','/deterministic+','/warnaserror+')
$common+=@($refs | ForEach-Object {if(!(Test-Path -LiteralPath $_)){throw ('Missing reference '+$_)};'/reference:'+$_})
$icons=@(
    @{file='prayer-open-rosary-large-cross-nearest100-v1.png';name='BPS.Prayer.png';sha256='0f9324e6b6c40607856e922b750963675723f0551ad9337a2706f37a106e06c7'},
    @{file='bless-place-nearest37-v1.png';name='BPS.Place.png';sha256='cdb90b9719f3efce7656b1b8403c4033eb407cf76dafa509b7a23919ff0adfbb'},
    @{file='remove-blessing-nearest37-v1.png';name='BPS.Remove.png';sha256='38c0dd4ec6a76170460e48227a60bb6a39631f6fd7db43b4276ba6401b46be4d'}
)
$resources=@($icons | ForEach-Object {
    $path=Join-Path $repo ('artifacts/IconConcepts/'+$_.file)
    if(!(Test-Path -LiteralPath $path) -or (Get-FileHash -LiteralPath $path).Hash.ToLowerInvariant() -ne $_.sha256){throw ('Missing or changed selected icon '+$_.file)}
    '/resource:'+$path+','+$_.name
})
& (Join-Path $DotnetRoot 'dotnet.exe') $compiler @common @resources '/target:library' ('/out:'+$dll) (Join-Path $PSScriptRoot 'Plugin.cs') (Join-Path $PSScriptRoot 'StationMenuView.cs') (Join-Path $PSScriptRoot 'BeamState.cs') (Join-Path $PSScriptRoot 'SavedBeamState.cs') (Join-Path $PSScriptRoot 'CapacityPurchase.cs') (Join-Path $PSScriptRoot 'TestResources.cs') (Join-Path $PSScriptRoot 'SkullProjection.cs') (Join-Path $PSScriptRoot 'FieldGeometry.cs') (Join-Path $PSScriptRoot 'PhysicalSnapshot.cs') (Join-Path $PSScriptRoot '../FaithProbe/GridFootprint.cs') (Join-Path $PSScriptRoot '../FaithProbe/StationMaterials.cs')
if($LASTEXITCODE -ne 0){throw 'Probe compilation failed'}
if([Reflection.AssemblyName]::GetAssemblyName($dll).Version.ToString() -ne ($version+'.0') -or [Diagnostics.FileVersionInfo]::GetVersionInfo($dll).FileVersion -ne ($version+'.0')){throw 'Version mismatch'}
$test=Join-Path $out 'MenuTests.exe'
& (Join-Path $DotnetRoot 'dotnet.exe') $compiler @common '/target:exe' ('/out:'+$test) (Join-Path $PSScriptRoot 'MenuTests.cs') (Join-Path $PSScriptRoot 'EffectTests.cs') (Join-Path $PSScriptRoot 'SavedBeamTests.cs') (Join-Path $PSScriptRoot 'CapacityPurchaseTests.cs') (Join-Path $PSScriptRoot 'PresentationTests.cs')
if($LASTEXITCODE -ne 0){throw 'Test compilation failed'}
& $test $managed $core $dll
if($LASTEXITCODE -ne 0){throw 'Menu tests failed'}
if($CheckOnly){Write-Output 'Development compile and menu gates passed; not a handoff artifact';return}
$manifest=[ordered]@{kind='research-only native station-menu presentation with existing payment/persistence/effect';version=$version;source=$head;compiler='SDK6.0.406 Roslyn';file=(Split-Path $dll -Leaf);sha256=(Get-FileHash -LiteralPath $dll).Hash.ToLowerInvariant();icons=$icons;references=@($refs | ForEach-Object {[ordered]@{name=(Split-Path $_ -Leaf);sha256=(Get-FileHash -LiteralPath $_).Hash.ToLowerInvariant()}})}
$manifest | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $out 'build-identity.json') -Encoding UTF8
Write-Output ('Built '+$dll+' SHA256='+$manifest.sha256+' source='+$head)
