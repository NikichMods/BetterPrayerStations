# SPDX-License-Identifier: MPL-2.0
param([Parameter(Mandatory=$true)][string]$ManagedDirectory)
$ErrorActionPreference='Stop'
$hostAssembly=[Reflection.Assembly]::LoadFrom((Join-Path $ManagedDirectory 'Assembly-CSharp.dll'))
$serializationAssembly=[Reflection.Assembly]::LoadFrom((Join-Path $ManagedDirectory 'Assembly-CSharp-firstpass.dll'))
$itemType=$hostAssembly.GetType('Item',$true)
$resType=$hostAssembly.GetType('GameRes',$true)
$wgoType=$hostAssembly.GetType('SerializableWGO',$true)
$mapType=$hostAssembly.GetType('SerializableGameMap',$true)
$serializerType=$serializationAssembly.GetType('SmartSerializer',$true)
$binding=[Reflection.BindingFlags]'Public,NonPublic,Instance'
$serializeMethod=$serializerType.GetMethods() | Where-Object { $_.Name -eq 'Serialize' -and $_.GetParameters().Count -eq 1 }
$deserializeMethod=$serializerType.GetMethods() | Where-Object { $_.Name -eq 'Deserialize' -and $_.GetParameters().Count -eq 1 -and $_.GetParameters()[0].ParameterType -eq [byte[]] }
function New-FixtureItem([string]$id) {
 # Avoid constructors that consult Unity Resources. No Harmony or game process is used.
 $instance=[Runtime.Serialization.FormatterServices]::GetUninitializedObject($itemType)
 $itemType.GetField('id',$binding).SetValue($instance,$id)
 $itemType.GetField('value',$binding).SetValue($instance,[int]1)
 $itemType.GetField('_params',$binding).SetValue($instance,[Activator]::CreateInstance($resType))
 foreach ($field in 'inventory','secondary_inventory') {
  $inventoryField=$itemType.GetField($field,$binding)
  $inventoryField.SetValue($instance,[Activator]::CreateInstance($inventoryField.FieldType))
 }
 return $instance
}
function Set-Value($item,[string]$key,[single]$value) {
 $itemType.GetMethod('SetParam',[type[]]@([string],[single])).Invoke($item,[object[]]@($key,$value)) | Out-Null
}
function Read-Value($item,[string]$key) {
 return $itemType.GetMethod('GetParam',[type[]]@([string],[single])).Invoke($item,[object[]]@($key,[single]0))
}
function Roundtrip($type,$value) {
 $bytes=$serializeMethod.MakeGenericMethod($type).Invoke($null,[object[]]@($value))
 return $deserializeMethod.MakeGenericMethod($type).Invoke($null,[object[]]@(,$bytes))
}
function Assert-Value($actual,$expected,[string]$label) {
 if ($actual -ne $expected) {throw ($label+': expected '+$expected+', got '+$actual)}
}
$grave=New-FixtureItem 'grave_ground'
Set-Value $grave 'nikichmods.bps.schema' 1
Set-Value $grave 'nikichmods.bps.conversions' 3
Set-Value $grave 'nikichmods.bps.wreaths' 2
Set-Value $grave 'nikichmods.bps.actions' 5
Set-Value $grave 'nikichmods.bps.look.stone' 7
Set-Value $grave 'nikichmods.bps.look.fence' 11
$decoration=New-FixtureItem 'grave_top_stn_plate_2'
Set-Value $decoration 'durability' 0.625
$grave.inventory.Add($decoration)
$body=New-FixtureItem 'body'
$grave.inventory.Add($body)
$record=[Activator]::CreateInstance($wgoType)
$wgoType.GetField('obj_id').SetValue($record,'grave_ground')
$wgoType.GetField('unique_id').SetValue($record,[long]123456789)
$wgoType.GetField('item').SetValue($record,$grave)
$map=[Activator]::CreateInstance($mapType)
$mapType.GetField('_wgos',$binding).GetValue($map).Add($record)
$secondGrave=New-FixtureItem 'grave_empty'
Set-Value $secondGrave 'nikichmods.bps.look.stone' 2
Set-Value $secondGrave 'nikichmods.bps.look.fence' 4
$secondRecord=[Activator]::CreateInstance($wgoType)
$wgoType.GetField('obj_id').SetValue($secondRecord,'grave_empty')
$wgoType.GetField('unique_id').SetValue($secondRecord,[long]987654321)
$wgoType.GetField('item').SetValue($secondRecord,$secondGrave)
$mapType.GetField('_wgos',$binding).GetValue($map).Add($secondRecord)
$restoredMap=Roundtrip $mapType $map
$restored=$mapType.GetField('_wgos',$binding).GetValue($restoredMap)[0]
Assert-Value $restored.unique_id 123456789 'WGO identity'
Assert-Value (Read-Value $restored.item 'nikichmods.bps.conversions') 3 'Grave conversions'
Assert-Value (Read-Value $restored.item 'nikichmods.bps.wreaths') 2 'Grave wreaths'
Assert-Value (Read-Value $restored.item 'nikichmods.bps.actions') 5 'Shared action count'
Assert-Value $restored.item.inventory.Count 2 'Nested inventory'
Assert-Value (Read-Value $restored.item 'nikichmods.bps.look.stone') 7 'Grave gravestone appearance'
Assert-Value (Read-Value $restored.item 'nikichmods.bps.look.fence') 11 'Grave fence appearance'
Assert-Value (Read-Value $restored.item.inventory[0] 'nikichmods.bps.look.stone') 0 'No appearance exported to decoration'
$restoredSecond=$mapType.GetField('_wgos',$binding).GetValue($restoredMap)[1]
Assert-Value $restoredSecond.item.inventory.Count 0 'Empty grave has no physical decoration'
Assert-Value (Read-Value $restoredSecond.item 'nikichmods.bps.look.stone') 2 'Separate empty grave gravestone choice'
Assert-Value (Read-Value $restoredSecond.item 'nikichmods.bps.look.fence') 4 'Separate empty grave fence choice'
Assert-Value (Read-Value $restored.item.inventory[0] 'durability') ([single]0.625) 'Decoration durability'
Assert-Value (Read-Value $restored.item.inventory[1] 'nikichmods.bps.conversions') 0 'Body separation'
$chest=New-FixtureItem 'chest'
$chest.inventory.Add($decoration)
$restoredChest=Roundtrip $itemType $chest
Assert-Value (Read-Value $restoredChest.inventory[0] 'nikichmods.bps.look.stone') 0 'Stored ordinary decoration has no grave look'
Set-Value $restored.item 'nikichmods.bps.actions' 9
Assert-Value (Read-Value $grave 'nikichmods.bps.actions') 5 'Independent deserialized grave'
$empty=New-FixtureItem 'grave_empty'
Assert-Value (Read-Value $empty 'nikichmods.bps.actions') 0 'Fresh grave default'
Assert-Value (Read-Value $empty 'nikichmods.bps.look.stone') 0 'Default native appearance'
Write-Output 'PASS native binary serialization: SerializableWGO identity, grave counters and two grave-owned looks, separate empty-grave looks, ordinary decoration/chest data, durability, body separation and independent restored objects.'
Write-Output 'LIMIT: synthetic uninitialized Items; no live save/load, Unity rendering, craft removal, inventory transfer, WGO movement or production modifier execution was exercised.'
