

using System;
using UnityEngine;
using XRL.Core;
using XRL.Messages;

namespace XRL.World.Parts
{

    class DisplayFactionInDescription : IPart
    {
        static int displayNameEvents = 0;
        private string GetFactionString(Faction faction){
            XRL.World.GameObject player = XRLCore.Core?.Game?.Player?.Body;
            if (player is null) {
                //Debug.Log("Player is null, big sad");
                return "";
            }
            Debug.Log("Getting faction feeling for faction " + faction.Name);
            int? factionFeeling = faction.GetFeelingTowardsObject(player); 
            if (factionFeeling is null) {
                //Debug.Log($"Oh no, seems there was no faction by that name, I am cri :'(");
                return "";
            }
            char factionColour = (factionFeeling < 0) ? 'R' : (factionFeeling < 50) ? 'y' : 'G';
            return "{{" + factionColour + "|" + faction.DisplayName + "}}";
        }

        public override bool HandleEvent(GetDisplayNameEvent E)
        {
            try
            {
                displayNameEvents++;
                //MessageQueue.AddPlayerMessage($"Running GetDisplayName Event {displayNameEvents}");
                if (ParentObject is null || ParentObject.IsPlayer())
                {
                    return true;
                }
                string factionName;
                Faction faction;

                if ((factionName = ParentObject.pBrain?.GetPrimaryFaction()) != null
                && (faction = Factions.get(factionName)) != null
                && faction.Visible)
                {
                    E.AddClause($"({GetFactionString(faction)})");
                }
                if (ParentObject.IsOwned()
                && (factionName = ParentObject.Owner) != null
                && (faction = Factions.get(ParentObject.Owner)) != null
                && faction.Visible)
                {
                    //MessageQueue.AddPlayerMessage("Getting Owner baybee");
                    E.AddClause($"[{GetFactionString(faction)}]");
                }
            } catch (Exception ex){
                Debug.LogError("Failed to add Faction info to display name with exception: \n " + ex);
            }

            return true;
        }

        public override bool WantEvent(int ID, int cascade)
        {

            return ID == GetDisplayNameEvent.ID;

        }

    }

}