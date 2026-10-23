# SPDX-License-Identifier: MPL-2.0
param([Parameter(Mandatory=$true)][string]$ManagedDirectory)
$ErrorActionPreference='Stop'
# Offline research only: synthetic native Items, no Harmony, live WGO, save or game process.
# Method checkpoint: source/IL establishes the seams; actual native GetBodySkulls is the
# independent oracle for the proposed small decay leaf, especially the 90% threshold.
$hostPath=Join-Path $ManagedDirectory 'Assembly-CSharp.dll'
$expected='e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8'
if((Get-FileHash -LiteralPath $hostPath).Hash.ToLowerInvariant() -ne $expected){throw 'Unverified host'}
$asm=[Reflection.Assembly]::LoadFrom($hostPath)
[Reflection.Assembly]::LoadFrom((Join-Path $ManagedDirectory 'UnityEngine.CoreModule.dll')) | Out-Null
$itemType=$asm.GetType('Item',$true)
$defType=$asm.GetType('ItemDefinition',$true)
$resType=$asm.GetType('GameRes',$true)
$binding=[Reflection.BindingFlags]'Public,NonPublic,Instance'
function Field($object,[string]$name,$value){$object.GetType().GetField($name,$binding).SetValue($object,$value)}
function New-Body([int]$red,[int]$white,[single]$durability){
 $body=[Runtime.Serialization.FormatterServices]::GetUninitializedObject($itemType)
 $def=[Runtime.Serialization.FormatterServices]::GetUninitializedObject($defType)
 Field $def 'id' 'body'
 Field $def 'q_minus' $red
 Field $def 'q_plus' $white
 Field $body 'id' 'body'
 Field $body 'value' ([int]1)
 Field $body '_definition' $def
 Field $body '_definition_set' $true
 Field $body '_definition_set_id' 'body'
 Field $body '_params' ([Activator]::CreateInstance($resType))
 foreach($name in @('inventory','secondary_inventory')){
  $f=$itemType.GetField($name,$binding)
  $f.SetValue($body,[Activator]::CreateInstance($f.FieldType))
 }
 $body.SetParam('durability',$durability)
 return $body
}
function Native-Skulls($body){
 $arguments=[object[]]@([int]0,[int]0,[int]0,$false)
 $itemType.GetMethod('GetBodySkulls').Invoke($body,$arguments) | Out-Null
 return [pscustomobject]@{red=[int]$arguments[0];white=[int]$arguments[1];available=[int]$arguments[2]}
}
function Assert-Equal($actual,$expected,[string]$name){if($actual -ne $expected){throw "$name expected=$expected actual=$actual"}}
$cases=0
foreach($durability in @([single]1,[single]0.97,[single]0.9001,[single]0.9,[single]0.8999,[single]0.8,[single]0.01,[single]0)){
 foreach($pair in @(@(3,3),@(0,6),@(7,0),@(1,11),@(-2,-1))){
  $body=New-Body $pair[0] $pair[1] $durability
  $before=Native-Skulls $body
  foreach($capacity in @(0,1,3,20)){
   $converted=[math]::Min($capacity,$before.red)
   $effectiveRed=$before.red-$converted
   $effectiveWhite=$before.white+$converted
   # Original proposed derived leaf, compared with a separately constructed native
   # reference body. This reference is a test oracle, never a production shadow body.
   $percent=[int][math]::Ceiling([single]($durability*[single]100))
   if($percent -gt 90){$percent=100}
   $available=[int][math]::Floor([single]([single]($effectiveWhite*$percent)/[single]100))
   $reference=Native-Skulls (New-Body $effectiveRed $effectiveWhite $durability)
   Assert-Equal $reference.red $effectiveRed 'effective red'
   Assert-Equal $reference.white $effectiveWhite 'effective white'
   Assert-Equal $reference.available $available 'native available-white oracle'
   if($before.red -eq 0){Assert-Equal $effectiveWhite $before.white 'unused capacity creates no white'}
   $after=Native-Skulls $body
   Assert-Equal $after.red $before.red 'original red unchanged'
   Assert-Equal $after.white $before.white 'original white unchanged'
   Assert-Equal $after.available $before.available 'original available unchanged'
   Assert-Equal ($body.GetParam('durability',[single]0)) $durability 'original durability unchanged'
   $cases++
  }
 }
}
Assert-Equal (Native-Skulls (New-Body 0 6 ([single]0.8))).available 4 'six-white 80-percent boundary'
Assert-Equal (Native-Skulls (New-Body 0 6 ([single]0.9))).available 5 'six-white 90-percent boundary'
Assert-Equal (Native-Skulls (New-Body 0 6 ([single]0.9001))).available 6 'ceil-percent above90 becomes100'
Write-Output "PASS native skull oracle: $cases cases; decay thresholds, clamped totals, red-bound conversion, unchanged synthetic original bodies."
Write-Output 'LIMIT no live organ/body edits, WGO quality patch, native GUI, zone totals, coverage, payment, exhumation or save lifecycle were executed. This is research arithmetic evidence, not production acceptance.'
