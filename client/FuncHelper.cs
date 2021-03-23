using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace fivem_caffe_job
{
    public class FuncHelper : BaseScript
    {
        public static void ChatMessage(string msg)
        {
            TriggerEvent("chat:addMessage", new
            {
                color = new[] { 255, 255, 255 },
                args = new[] { $"{msg}" }
            });
        }
        public static void ChatMessage(string msg, Array color)
        {
            TriggerEvent("chat:addMessage", new
            {
                color = color, //color = new[] { 0, 0, 0 },
                args = new[] { $"{msg}" }
            });
        }

        public enum HelpType
        {
            onestring = 1,
            threestring = 3
        }
        public static void ShowHelp(HelpType helpType, string msg, string linetwo = "", string linethree = "")
        {
            if((int)helpType == 3)
            {
                API.BeginTextCommandDisplayHelp("THREESTRINGS");
                API.AddTextComponentSubstringPlayerName(msg);
                API.AddTextComponentSubstringPlayerName(linetwo);
                API.AddTextComponentSubstringPlayerName(linethree);
                API.EndTextCommandDisplayHelp(0, true, false, 5000);
            }
            else if ((int)helpType == 1)
            {
                API.BeginTextCommandDisplayHelp("STRING");
                API.AddTextComponentSubstringPlayerName(msg);
                API.EndTextCommandDisplayHelp(0, false, true, 5000);
            }
        }

        public static void CreateBlip(Vector3 position, BlipSprite sprite, BlipColor color, string name)
        {
            Blip blip = World.CreateBlip(position);
            blip.Sprite = sprite;
            blip.Color = color;
            blip.Name = name;
        }
        public static Blip CreateBlipWithExisting(/*Blip blip,*/ Vector3 position, BlipSprite sprite, BlipColor color, string name)
        {
            Blip blip = World.CreateBlip(position);
            blip.Sprite = sprite;
            blip.Color = color;
            blip.Name = name;
            blip.IsShortRange = true;

            return blip;
        }
    }
}
