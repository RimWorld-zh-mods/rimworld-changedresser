﻿using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ChangeDresser
{
    public class PawnOutfits : IExposable
    {
        public List<Outfit> Outfits = new List<Outfit>();
        public Pawn Pawn = null;

        private Outfit lastBattleOutfit = null;
        public Outfit LastBattleOutfit { set { this.lastBattleOutfit = value; } }

        private Outfit lastCivilianOutfit = null;
        public Outfit LastCivilianOutfit { set { this.lastCivilianOutfit = value; } }

        private List<Color> ColorForLayer = null;
        private List<bool> IsColorAssigned;

        public PawnOutfits()
        {
            this.InitializeIsColorAssigned();
        }

        public void InitializeIsColorAssigned()
        {
            int size = Enum.GetValues(typeof(ApparelLayer)).Length;
            this.IsColorAssigned = new List<bool>(size);
            for (int i = 0; i < size; ++i)
            {
                this.IsColorAssigned.Add(false);
            }
        }

        public bool ColorApparel(Apparel apparel)
        {
#if DEBUG
            Log.Message(Environment.NewLine + "Start PawnOutfits.TryGetColorFor Layer: " + layer + " " + ((int)layer).ToString());

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < this.IsColorAssigned.Count; ++i)
            {
                sb.Append(this.IsColorAssigned[i]);
                if (this.ColorForLayer != null)
                {
                    sb.Append(" ");
                    sb.Append(this.ColorForLayer[i].ToString());
                }
                sb.Append(" -- ");
            }
            Log.Warning(sb.ToString());
#endif
            int layer = (int)apparel.def.apparel.LastLayer;
            if (layer < this.IsColorAssigned.Count && 
                this.IsColorAssigned[layer])
            {
                apparel.SetColor(this.ColorForLayer[layer]);
#if DEBUG
                Log.Message("Start PawnOutfits.TryGetColorFor" + Environment.NewLine);
#endif
                return true;
            }
            return false;
        }

        public void ColorApparel(Pawn pawn)
        {
            foreach (Apparel a in pawn.apparel.WornApparel)
            {
                this.ColorApparel(a);
            }
        }

        public void SetColorFor(ApparelLayer layer, Color color)
        {
#if DEBUG
            Log.Message(Environment.NewLine + "Start PawnOutfits.SetColorFor Layer: " + layer + " " + ((int)layer).ToString() + " Color: " + color);
#endif
            if (this.ColorForLayer == null || this.ColorForLayer.Count == 0)
            {
                this.ColorForLayer = new List<Color>(this.IsColorAssigned.Count);
                for(int i = 0; i < this.IsColorAssigned.Count; ++i)
                {
                    this.ColorForLayer.Add(default(Color));
                }
            }
            this.IsColorAssigned[(int)layer] = true;
            this.ColorForLayer[(int)layer] = color;
#if DEBUG
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < this.IsColorAssigned.Count; ++i)
            {
                sb.Append(this.IsColorAssigned[i]);
                if (this.ColorForLayer != null)
                {
                    sb.Append(" ");
                    sb.Append(this.ColorForLayer[i].ToString());
                }
                sb.Append(" -- ");
            }
            Log.Warning(sb.ToString());
            Log.Message("End PawnOutfits.SetColorFor" + Environment.NewLine);
#endif
        }

        public bool TryGetBattleOutfit(out Outfit outfit)
        {
            if (this.lastBattleOutfit != null)
            {
                if (WorldComp.OutfitsForBattle.Contains(this.lastBattleOutfit) &&
                    this.Outfits.Contains(this.lastBattleOutfit))
                {
                    outfit = this.lastBattleOutfit;
                    return true;
                }
                else
                {
                    this.lastBattleOutfit = null;
                }
            }

            foreach (Outfit o in this.Outfits)
            {
                if (WorldComp.OutfitsForBattle.Contains(o))
                {
                    outfit = o;
                    return true;
                }
            }
            outfit = null;
            return false;
        }

        public bool TryGetCivilianOutfit(out Outfit outfit)
        {
            if (this.lastCivilianOutfit != null)
            {
                if (!WorldComp.OutfitsForBattle.Contains(this.lastCivilianOutfit) &&
                    this.Outfits.Contains(this.lastCivilianOutfit))
                {
                    outfit = this.lastCivilianOutfit;
                    return true;
                }
                else
                {
                    this.lastCivilianOutfit = null;
                }
            }

            foreach (Outfit o in this.Outfits)
            {
                if (!WorldComp.OutfitsForBattle.Contains(o))
                {
                    outfit = o;
                    return true;
                }
            }
            outfit = null;
            return false;
        }

        public void ExposeData()
        {
            Scribe_References.Look(ref this.Pawn, "pawn");
            Scribe_Collections.Look(ref this.Outfits, "outfits", LookMode.Reference, new object[0]);
            Scribe_References.Look(ref this.lastBattleOutfit, "lastBattleOutfit");
            Scribe_References.Look(ref this.lastCivilianOutfit, "lastCivilianOutfit");
            Scribe_Collections.Look(ref this.IsColorAssigned, "isColorAssigned", LookMode.Value);
            Scribe_Collections.Look(ref this.ColorForLayer, "colorForLayer", LookMode.Value, new object[0]);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (this.Outfits == null)
                {
                    this.Outfits = new List<Outfit>(0);
                }

                if (this.IsColorAssigned == null)
                {
                    this.InitializeIsColorAssigned();
                    this.ColorForLayer = null;
                }
            }
        }
    }
}
