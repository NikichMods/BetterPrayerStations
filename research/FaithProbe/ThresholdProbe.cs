// SPDX-License-Identifier: MPL-2.0
using System;
using System.Collections.Generic;
using UnityEngine;
namespace NikichMods.BPSFaithProbe
{
    // Diagnostic planning only. Actual native getters verify every planned result.
    internal sealed class ThresholdPlan
    {
        public int C,L,Cost;public bool Conversion;public float Quality;public double Sum;
        public static float Predict(int red,int white,int available,float durability,float decor,int c,int l)
        {SavedFaithState.Convert(ref red,ref white,ref available,durability,c);return Math.Min((float)Math.Floor(decor+l)-red,available);}
        public static ThresholdPlan Find(int red,int white,int available,float durability,float decor,float current,double sum,int faith)
        {
            if(Predict(red,white,available,durability,decor,0,0)!=current)return null;
            ThresholdPlan best=null;
            for(int c=0;c<=Math.Min(red,SavedFaithState.Limit);c++)for(int l=0;l<=SavedFaithState.Limit;l++)
            {
                float q=Predict(red,white,available,durability,decor,c,l);
                double prepared=sum-ContributionMath.Positive(current)+ContributionMath.Positive(q);
                if(prepared%10!=9 || ContributionMath.Bonus(prepared)!=ContributionMath.Bonus(sum))continue;
                int cost=FaithPurchase.Price(c+l);if(cost>faith || (best!=null && cost>=best.Cost))continue;
                for(int kind=0;kind<2;kind++)
                {
                    bool conversion=kind==0;
                    if(conversion?(c>=red || c>=SavedFaithState.Limit):l>=SavedFaithState.Limit)continue;
                    float after=Predict(red,white,available,durability,decor,c+(conversion?1:0),l+(conversion?0:1));
                    if(after-q!=1 || ContributionMath.Positive(after)-ContributionMath.Positive(q)!=1)continue;
                    best=new ThresholdPlan{C=c,L=l,Cost=cost,Conversion=conversion,Quality=q,Sum=prepared};break;
                }
            }
            return best;
        }
    }
    public sealed partial class Plugin
    {
        ThresholdPlan threshold;WorldGameObject thresholdGrave,thresholdStation;
        string thresholdPhysical,thresholdInfo="Пороговый тест: подготовка бесплатно; последний шаг за настоящую веру.";
        void ResetThreshold(){threshold=null;thresholdGrave=thresholdStation=null;thresholdPhysical=null;}
        PassiveQuality.Row ThresholdOwner(WorldGameObject grave)
        {
            Rect rect;if(!coverage.Geometry.TryRect(grave,out rect))return null;
            var emitters=new List<ContributionMath.Emitter>();
            foreach(var row in passive.Rows)emitters.Add(new ContributionMath.Emitter{Id=row.Station.unique_id,X=row.Center.x,Y=row.Center.y,Half=row.Half,Zone=row.Zone});
            int k=ContributionMath.Choose(emitters,rect.center.x,rect.center.y,zoneOwner.GetValue(grave));
            return k<0?null:passive.Rows[k];
        }
        void PrepareThreshold()
        {
            if(!ready || !Current() || !effects || !passiveEnabled || !NativeWindowsClosed() || MainGame.me.gui_elements.build_mode_gui.is_shown)
            {status="Закройте игровые окна и выйдите из строительства; эффекты должны быть включены.";return;}
            if(graves.Items.Count!=0){status="Сначала очистите прежние тестовые назначения и перезагрузитесь. Подготовка не перезаписывает вложения.";return;}
            coverage.Refresh();RefreshPassive(true);ResetThreshold();
            ThresholdPlan best=null;WorldGameObject chosen=null,station=null;
            int faith=FaithPurchase.Count(MainGame.me.player.data);
            foreach(var grave in coverage.GraveOwners)
            {
                if(!Trackable(grave) || grave.obj_id!="grave_ground" || grave.components.craft.is_crafting || SavedFaithState.Read(grave.data).Actions!=0)continue;
                var corpse=grave.GetBodyFromInventory(true);if(corpse==null)continue;
                var row=ThresholdOwner(grave);if(row==null || row.Zone.definition==null || row.Zone.definition.calc_method!=WorldZoneDefinition.QualityCalcMethod.Sum || grave.obj_def.ignore_counting_at_zone || row.Station.obj_def.ignore_counting_at_zone)continue;
                int r,w,a;corpse.GetBodySkulls(out r,out w,out a,false);
                var plan=ThresholdPlan.Find(r,w,a,corpse.durability,grave.data.GetInventoryQuality(null),grave.quality,row.Sum,faith);
                if(plan!=null && (best==null || plan.Cost<best.Cost || (plan.Cost==best.Cost && grave.unique_id<chosen.unique_id)))
                {best=plan;chosen=grave;station=row.Station;}
            }
            if(best==null){status="Не найдена могила для точного шага через порог с текущим запасом веры. Ничего не изменено; см. журнал.";Log("THRESHOLD_NO_PLAN faith="+faith+" stations="+passive.Rows.Count);return;}
            string physical=GraveHash(chosen);float money=MainGame.me.player.data.money;
            MainGame.me.gui_elements.grave.Open(chosen);Select(1);
            if(!Bound(chosen) || !ReferenceEquals(target,chosen) || TwoGraves.SlotOf(chosen)!=1)throw new InvalidOperationException("Threshold native grave selection failed");
            var before=ThresholdOwner(chosen);int bonusBefore=before.Bonus;
            Measure("threshold-free-setup",delegate{SavedFaithState.Write(chosen.data,best.C,best.L);});
            RefreshPassive(true);var actual=ThresholdOwner(chosen);
            bool intact=physical==GraveHash(chosen) && faith==FaithPurchase.Count(MainGame.me.player.data) && money==MainGame.me.player.data.money;
            bool valid=actual!=null && ReferenceEquals(actual.Station,station) && actual.Sum==best.Sum && actual.Bonus==bonusBefore && chosen.quality==best.Quality;
            Log("THRESHOLD_SETUP grave="+chosen.unique_id+" station="+station.unique_id+" c="+best.C+" l="+best.L+" quality="+chosen.quality.ToString("R",Inv)+" sum="+(actual==null?-1:actual.Sum).ToString("R",Inv)+" bonus="+(actual==null?-1:actual.Bonus)+" next_action="+(best.Conversion?"conversion":"wreath")+" cost="+best.Cost+" free=True native_matches="+valid+" physical_money_faith_unchanged="+intact);
            if(!intact || !valid)throw new InvalidOperationException("Threshold free preparation does not match native state");
            threshold=best;thresholdGrave=chosen;thresholdStation=station;thresholdPhysical=physical;
            thresholdInfo="Могила №"+chosen.unique_id+": "+best.Quality+"; сумма "+best.Sum+" → "+(best.Sum+1)+". Купить "+(best.Conversion?"превращение":"венок")+" за "+best.Cost+" веры; ожидается +2 кладбищу.";
            status="Подготовка выполнена бесплатно. Тело и украшения сохранены. Теперь нажмите отдельную кнопку покупки шага через порог.";
        }
        void BuyThreshold()
        {
            if(threshold==null || !Current())return;
            var plan=threshold;
            coverage.Refresh();RefreshPassive(true);var row=ThresholdOwner(thresholdGrave);
            bool valid=ready && effects && passiveEnabled && Bound(thresholdGrave) && ReferenceEquals(target,thresholdGrave) && gui!=null && gui.gameObject.activeInHierarchy && ReferenceEquals(graveField.GetValue(gui),thresholdGrave) && !MainGame.me.gui_elements.build_mode_gui.is_shown && !FloatingWorldGameObject.IsFloating() && row!=null && ReferenceEquals(row.Station,thresholdStation) && row.Sum==plan.Sum && thresholdGrave.quality==plan.Quality && conversions==plan.C && wreaths==plan.L && GraveHash(thresholdGrave)==thresholdPhysical;
            if(!valid){status="Условия порогового шага изменились. Вера не списана; повторная подготовка требует очистки и загрузки.";Log("THRESHOLD_REJECT stale=True charged=False");ResetThreshold();return;}
            var zone=row.Zone;double sum=row.Sum;int bonus=row.Bonus;
            float quality=thresholdGrave.quality,stationQuality=thresholdStation.quality,total=zone.GetTotalQuality();
            int faith=FaithPurchase.Count(MainGame.me.player.data);float money=MainGame.me.player.data.money;
            Buy(plan.Conversion,plan.C+plan.L,plan.Cost);
            if(conversions!=plan.C+(plan.Conversion?1:0) || wreaths!=plan.L+(plan.Conversion?0:1))return; // Existing transaction reports shortage/refusal.
            RefreshPassive(true);row=ThresholdOwner(thresholdGrave);
            float after=zone.GetTotalQuality();int faithAfter=FaithPurchase.Count(MainGame.me.player.data);
            bool graveOk=thresholdGrave.quality==quality+1;
            bool stationOk=row!=null && ReferenceEquals(row.Station,thresholdStation) && row.Sum==sum+1 && row.Bonus==bonus+1 && thresholdStation.quality==stationQuality+1;
            bool zoneOk=Math.Abs(after-total-2)<.001f;
            bool intact=GraveHash(thresholdGrave)==thresholdPhysical && money==MainGame.me.player.data.money && faith-faithAfter==plan.Cost;
            Log("THRESHOLD_PURCHASE grave="+thresholdGrave.unique_id+" station="+thresholdStation.unique_id+" quality_before="+quality+" quality_after="+thresholdGrave.quality+" sum_before="+sum+" sum_after="+(row==null?-1:row.Sum)+" bonus_before="+bonus+" bonus_after="+(row==null?-1:row.Bonus)+" station_before="+stationQuality+" station_after="+thresholdStation.quality+" zone_before="+total+" zone_after="+after+" faith_before="+faith+" faith_after="+faithAfter+" cost="+plan.Cost+" grave_ok="+graveOk+" station_ok="+stationOk+" zone_ok="+zoneOk+" physical_money_debit_ok="+intact);
            ResetThreshold();
            if(!graveOk || !stationOk || !zoneOk || !intact)throw new InvalidOperationException("Paid faith/passive native threshold integration failed");
            thresholdInfo="Порог пройден: могила +1, молельня +1, кладбище +2. Вера списана; тело и украшения сохранены.";
            zoneCheckRequested=true;
        }
    }
}
