// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;

namespace NikichMods.BPSMenuProbe
{
    // Owns only the three transient station cards, never the shared prefab.
    internal sealed class StationMenuView:IDisposable
    {
        internal static readonly string[] ResourceNames={"BPS.Prayer.png","BPS.Place.png","BPS.Remove.png"};
        internal static readonly string[] ResourceHashes={
            "0f9324e6b6c40607856e922b750963675723f0551ad9337a2706f37a106e06c7",
            "cdb90b9719f3efce7656b1b8403c4033eb407cf76dafa509b7a23919ff0adfbb",
            "38c0dd4ec6a76170460e48227a60bb6a39631f6fd7db43b4276ba6401b46be4d"};
        readonly Texture2D[] textures=new Texture2D[3];
        readonly Sprite[] sprites=new Sprite[3];
        readonly Dictionary<CraftItemGUI,RowSnapshot> rows=new Dictionary<CraftItemGUI,RowSnapshot>();
        static readonly FieldInfo ingredients=AccessToolsField();
        static FieldInfo AccessToolsField()
        {
            var field=typeof(CraftItemGUI).GetField("_ingredients",BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public);
            if(field==null || field.FieldType!=typeof(BaseItemCellGUI[]))throw new MissingFieldException("Native ingredient cells");
            return field;
        }
        internal void Load()
        {
            for(int i=0;i<3;i++)
            {
                byte[] bytes;
                using(var stream=Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceNames[i]))
                {
                    if(stream==null)throw new FileNotFoundException(ResourceNames[i]);
                    using(var buffer=new MemoryStream()){stream.CopyTo(buffer);bytes=buffer.ToArray();}
                }
                string hash;using(var sha=SHA256.Create())hash=BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();
                if(hash!=ResourceHashes[i])throw new InvalidDataException("Unexpected station icon "+ResourceNames[i]);
                var texture=new Texture2D(2,2,TextureFormat.RGBA32,false);textures[i]=texture;
                if(!ImageConversion.LoadImage(texture,bytes,false))throw new InvalidDataException("Invalid icon PNG");
                texture.filterMode=FilterMode.Point;texture.wrapMode=TextureWrapMode.Clamp;
                int expected=i==0?100:37;
                if(texture.width!=expected || texture.height!=expected)throw new InvalidDataException("Unexpected icon dimensions");
                sprites[i]=Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),new Vector2(.5f,.5f),100f);
                sprites[i].name=ResourceNames[i];
            }
        }
        internal void BeforeRedraw(CraftItemGUI row)
        {
            RowSnapshot snapshot;
            if(!rows.TryGetValue(row,out snapshot)){snapshot=new RowSnapshot(row);rows.Add(row,snapshot);}
            snapshot.ResetGeometry();
            row.auto_height=true;
            row.label_descr.overflowMethod=UILabel.Overflow.ResizeHeight;
            // Native Redraw uses description.height-20 for its additional height.
            row.label_descr.height=20;
        }
        internal void AfterRedraw(CraftItemGUI row,int icon,int cost,int carried)
        {
            var cell=row.item_gui.container;
            if(cell==null || cell.icon==null)throw new MissingMemberException("Native action icon");
            cell.icon.sprite2D=sprites[icon];cell.icon.width=37;cell.icon.height=37;
            if(cell.empty_item_gfx!=null)cell.empty_item_gfx.SetActive(false);
            if(icon!=0)return;
            var cells=(BaseItemCellGUI[])ingredients.GetValue(row);
            if(cells==null || cells.Length==0 || cells[0]==null)throw new MissingMemberException("Native faith ingredient cell");
            var faith=cells[0];
            faith.gameObject.SetActive(cost>0);
            if(cost<=0)return;
            if(faith.container==null || faith.container.counter==null)throw new MissingMemberException("Native faith counter");
            // Native DrawIngredient format/colors, with the actual payer's funds.
            // Its generic interaction MultiInventory may also contain chests.
            faith.container.counter.text=carried+"/"+cost;
            faith.container.counter.color=carried<cost?faith.colors.not_enough_res:faith.colors.enough_res;
        }
        internal void Restore()
        {
            foreach(var pair in rows)if(pair.Key!=null)pair.Value.Restore();
            rows.Clear();
        }
        public void Dispose()
        {
            Restore();
            for(int i=0;i<3;i++)
            {
                if(sprites[i]!=null)UnityEngine.Object.Destroy(sprites[i]);
                if(textures[i]!=null)UnityEngine.Object.Destroy(textures[i]);
            }
        }
        sealed class RowSnapshot
        {
            readonly CraftItemGUI row;
            readonly CraftDefinition craft;
            readonly UIWidget root,parent;
            readonly int rootHeight,parentBottom,hgtHeight,descriptionHeight,iconWidth,iconHeight;
            readonly bool autoHeight,emptyActive;
            readonly UILabel.Overflow overflow;
            readonly UI2DSprite image;
            readonly GameObject empty;
            readonly Sprite sprite;
            internal RowSnapshot(CraftItemGUI value)
            {
                row=value;craft=row.craft_definition;root=row.GetComponent<UIWidget>();
                if(row.label_descr==null || row.hgt_widget==null || root==null)throw new MissingMemberException("Native card geometry");
                parent=row.label_descr.transform.parent.GetComponent<UIWidget>();
                if(parent==null)throw new MissingMemberException("Native description parent");
                rootHeight=root.height;parentBottom=parent.bottomAnchor.absolute;hgtHeight=row.hgt_widget.height;
                descriptionHeight=row.label_descr.height;overflow=row.label_descr.overflowMethod;autoHeight=row.auto_height;
                image=row.item_gui.x1.icon;empty=row.item_gui.x1.empty_item_gfx;
                if(image==null)throw new MissingMemberException("Native card image");
                iconWidth=image.width;iconHeight=image.height;sprite=image.sprite2D;emptyActive=empty!=null && empty.activeSelf;
            }
            internal void ResetGeometry()
            {
                root.height=rootHeight;row.hgt_widget.height=hgtHeight;
                parent.bottomAnchor.absolute=parentBottom;
                row.label_descr.height=descriptionHeight;
            }
            internal void Restore()
            {
                // Never restore a stale snapshot over an already-reused pooled row.
                if(row==null || !ReferenceEquals(row.craft_definition,craft))return;
                ResetGeometry();row.auto_height=autoHeight;row.label_descr.overflowMethod=overflow;
                image.sprite2D=sprite;image.width=iconWidth;image.height=iconHeight;
                if(empty!=null)empty.SetActive(emptyActive);
            }
        }
    }
}
