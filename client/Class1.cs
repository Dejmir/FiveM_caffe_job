using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;

using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using NativeUI;

using static fivem_caffe_job.FuncHelper;

namespace fivem_caffe_job
{
    public class Class1 : BaseScript
    {
        dynamic ESX;
        public Class1()
        {
            TriggerEvent("esx:getSharedObject", new object[] { new Action<dynamic>(esx => {
            ESX = esx;
            })});

            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
            EventHandlers["caffeLoadConfig"] += new Action<List<dynamic>>(loadConfig);

            EventHandlers["caffeHaveJobClient"] += new Action<bool>(HaveJob);
            EventHandlers["sendMsg"] += new Action<string>(sendMsg);
            EventHandlers["showNotification"] += new Action<string>(showNotification);
            EventHandlers["caffePassBool"] += new Action<bool, string>(passBool);

            EventHandlers["drinkAnimation"] += new Action<string>(drinkAnimation);

            EventHandlers["caffeWorkersClient"] += new Action<List<dynamic>>(workersClient);

            EventHandlers["caffeMythNotification"] += new Action<string, string>(mythNotification);

            EventHandlers["caffePlayerInformationClient"] += new Action<int, int, string>(playerInformation);
        }


        bool caffeJob = true;
        Blip blipMakeCoffe;
        Blip blipGetLowWater;
        Blip blipGetMediumWater;
        Blip blipGetHighWater;

        Blip blipGetLowSeeds;
        Blip blipGetMediumSeeds;
        Blip blipGetHighSeeds;

        Blip blipManagement;
        private async void OnClientResourceStart(string resourceName)
        {
            await Delay(14000 /*8500*/);
            if (API.GetCurrentResourceName() != resourceName) return;
            loadMenu();

            API.RegisterCommand("showLocation", new Action<int, List<object>, string>((source, args, raw) =>
            {
                var playerpedpos = Game.PlayerPed;
                TriggerEvent("chat:addMessage", new
                {
                    color = new [] {0,0,0},
                    args = new[] {$"X:{playerpedpos.Position.X} Y:{playerpedpos.Position.Y} Z:{playerpedpos.Position.Z}"}
                });
            }), false);

            TriggerServerEvent("caffeHaveJob", LocalPlayer.ServerId);
        }
        float[] blipMakeCoffeLoc = new float[3];
        float[] pickUpLowQualityWaterLoc = new float[3];
        float[] pickUpMediumQualityWaterLoc = new float[3];
        float[] pickUpHighQualityWaterLoc = new float[3];

        float[] pickUpLowQualitySeedsLoc = new float[3];
        float[] pickUpMediumQualitySeedsLoc = new float[3];
        float[] pickUpHighQualitySeedsLoc = new float[3];

        float[] managementLoc = new float[3];
        private void loadConfig(List<dynamic> configString)
        {
            blipMakeCoffeLoc[0] = float.Parse(configString[0]);
            blipMakeCoffeLoc[1] = float.Parse(configString[1]);
            blipMakeCoffeLoc[2] = float.Parse(configString[2]);
            pickUpLowQualityWaterLoc[0] = float.Parse(configString[0 +3]);
            pickUpLowQualityWaterLoc[1] = float.Parse(configString[1 +3]);
            pickUpLowQualityWaterLoc[2] = float.Parse(configString[2 +3]);
            pickUpMediumQualityWaterLoc[0] = float.Parse(configString[0 +6]);
            pickUpMediumQualityWaterLoc[1] = float.Parse(configString[1 +6]);
            pickUpMediumQualityWaterLoc[2] = float.Parse(configString[2 +6]);
            pickUpHighQualityWaterLoc[0] = float.Parse(configString[0 +9]);
            pickUpHighQualityWaterLoc[1] = float.Parse(configString[1 +9]);
            pickUpHighQualityWaterLoc[2] = float.Parse(configString[2 +9]);
            pickUpLowQualitySeedsLoc[0] = float.Parse(configString[0 +12]);
            pickUpLowQualitySeedsLoc[1] = float.Parse(configString[1 +12]);
            pickUpLowQualitySeedsLoc[2] = float.Parse(configString[2 +12]);
            pickUpMediumQualitySeedsLoc[0] = float.Parse(configString[0 +15]);
            pickUpMediumQualitySeedsLoc[1] = float.Parse(configString[1 +15]);
            pickUpMediumQualitySeedsLoc[2] = float.Parse(configString[2 +15]);
            pickUpHighQualitySeedsLoc[0] = float.Parse(configString[0 +18]);
            pickUpHighQualitySeedsLoc[1] = float.Parse(configString[1 +18]);
            pickUpHighQualitySeedsLoc[2] = float.Parse(configString[2 +18]);
            managementLoc[0] = float.Parse(configString[0 +21]);
            managementLoc[1] = float.Parse(configString[1 +21]);
            managementLoc[2] = float.Parse(configString[2 +21]);
        }
        private async Task mainCaffeJob()
        {
            //await Delay(8000);
            /*var blipMakeCoffeLocJson = (string)config.SelectToken("blipMakeCoffeLoc");
            string[] blipMakeCoffeLoc = blipMakeCoffeLocJson.Split(',');*/
            blipMakeCoffe = CreateBlipWithExisting(new Vector3(blipMakeCoffeLoc[0], blipMakeCoffeLoc[1], blipMakeCoffeLoc[2]), BlipSprite.Health, BlipColor.Green, "Zrób kawę");
            //blipMakeCoffe = World.CreateBlip(new Vector3(blipMakeCoffeLoc[0], blipMakeCoffeLoc[1], blipMakeCoffeLoc[2] /*412, -1014, 29*/));
            /*blipMakeCoffe.Sprite = BlipSprite.Health;
            blipMakeCoffe.Name = "Zrób kawe";
            blipMakeCoffe.Color = BlipColor.Green;*/
            blipGetLowWater = CreateBlipWithExisting(new Vector3(pickUpLowQualityWaterLoc[0], pickUpLowQualityWaterLoc[1], pickUpLowQualityWaterLoc[2]), BlipSprite.Creator, 
                BlipColor.Blue,"Woda niskiej jakości");
            blipGetMediumWater = CreateBlipWithExisting(new Vector3(pickUpMediumQualityWaterLoc[0], pickUpMediumQualityWaterLoc[1], pickUpMediumQualityWaterLoc[2]), BlipSprite.Creator, 
                BlipColor.Blue, "Woda średniej jakości");
            blipGetHighWater = CreateBlipWithExisting(new Vector3(pickUpHighQualityWaterLoc[0], pickUpHighQualityWaterLoc[1], pickUpHighQualityWaterLoc[2]), BlipSprite.Creator, 
                BlipColor.Blue,"Woda wysokiej jakości");
            blipGetLowSeeds = CreateBlipWithExisting(new Vector3(pickUpLowQualitySeedsLoc[0], pickUpLowQualitySeedsLoc[1], pickUpLowQualitySeedsLoc[2]), BlipSprite.Creator,
                BlipColor.Green, "Nasiona niskiej jakości");
            blipGetMediumSeeds = CreateBlipWithExisting(new Vector3(pickUpMediumQualitySeedsLoc[0], pickUpMediumQualitySeedsLoc[1], pickUpMediumQualitySeedsLoc[2]), BlipSprite.Creator,
                BlipColor.Green, "Nasiona średniej jakości");
            blipGetHighSeeds = CreateBlipWithExisting(new Vector3(pickUpHighQualitySeedsLoc[0], pickUpHighQualitySeedsLoc[1], pickUpHighQualitySeedsLoc[2]), BlipSprite.Creator,
                BlipColor.Green, "Nasiona wysokiej jakości");
            blipManagement = CreateBlipWithExisting(new Vector3(managementLoc[0], managementLoc[1], managementLoc[2]), BlipSprite.Business,
                BlipColor.White, "Biuro kawiarni");

            await Delay(1000);
            Tick += MainOnTick;
            Tick += RenderMarkerMakeCoffe;
            Tick += CheckCaffeJob;
        }

        bool menuEnabled = false;
        int x = 0;
        private async Task CheckCaffeJob()
        {
            await Delay(4000);
            TriggerServerEvent("caffeHaveJob", LocalPlayer.ServerId);
            if (!caffeJob)
            {
                if (x == 0)
                {
                    Tick -= RenderMarkerMakeCoffe;
                    blipMakeCoffe.Delete();
                    blipGetLowWater.Delete();
                    blipGetMediumWater.Delete();
                    blipGetHighWater.Delete();
                    blipGetLowSeeds.Delete();
                    blipGetMediumSeeds.Delete();
                    blipGetHighSeeds.Delete();
                    blipManagement.Delete();
                    x++;
                }
            }
            else if (x == 1)
            {
                x = 0;
                mainCaffeJob();
            }
        }
        private async Task MainOnTick()
        {
            menuPool.ProcessMenus();

            var playerped = Game.PlayerPed;
            if(playerped.Position.DistanceToSquared(blipMakeCoffe.Position) < 2.1f)
            {
                TriggerServerEvent("caffeHaveSeeds", $"{Game.Player.ServerId}", $"{LocalPlayer.Handle}");
                if (menuEnabled)
                {
                    if (API.IsControlJustReleased(1, 51))
                    {
                        TriggerServerEvent("caffeLowSeeds", $"{Game.Player.ServerId}");
                        TriggerServerEvent("caffeMediumSeeds", $"{Game.Player.ServerId}");
                        TriggerServerEvent("caffeHighSeeds", $"{Game.Player.ServerId}");
                        //ShowHelp(HelpType.onestring, "//menu here");
                        //if (menuPool.IsAnyMenuOpen()) menuPool.CloseAllMenus();
                        await Delay(1200);
                        menu.Visible = true;
                    }
                }
            }
            else if(playerped.Position.DistanceToSquared(blipManagement.Position) < 2.1f)
            {
                ShowHelp(HelpType.onestring, "Naciśnij ~INPUT_PICKUP~ aby otworzyć menu");
                if (API.IsControlJustReleased(1, 51))
                {
                    menu.Visible = false;
                    managementMenu.Visible = true;
                }
            }
            else
            {
                menuPool.CloseAllMenus(); menu.Visible = false; managementMenu.Visible = false;
            }
        }

        private async Task RenderMarkerMakeCoffe()
        {
            World.DrawMarker(MarkerType.VerticalCylinder, new Vector3(blipMakeCoffeLoc[0], blipMakeCoffeLoc[1], blipMakeCoffeLoc[2]), 
                new Vector3(blipMakeCoffeLoc[0], blipMakeCoffeLoc[1], blipMakeCoffeLoc[2]), new Vector3(1, 1, 1), new Vector3(1, 1, 1), System.Drawing.Color.FromArgb(87, 0, 0));

            World.DrawMarker(MarkerType.VerticalCylinder, new Vector3(pickUpLowQualityWaterLoc[0], pickUpLowQualityWaterLoc[1], pickUpLowQualityWaterLoc[2]), 
                new Vector3(pickUpLowQualityWaterLoc[0], pickUpLowQualityWaterLoc[1], pickUpLowQualityWaterLoc[2]), new Vector3(1, 1, 1), new Vector3(1, 1, 1), 
                System.Drawing.Color.FromArgb(59, 134, 255));

            World.DrawMarker(MarkerType.VerticalCylinder, new Vector3(pickUpMediumQualityWaterLoc[0], pickUpMediumQualityWaterLoc[1], pickUpMediumQualityWaterLoc[2]),
                new Vector3(pickUpMediumQualityWaterLoc[0], pickUpMediumQualityWaterLoc[1], pickUpMediumQualityWaterLoc[2]), new Vector3(1, 1, 1), new Vector3(1, 1, 1), 
                System.Drawing.Color.FromArgb(59, 134, 255));

            World.DrawMarker(MarkerType.VerticalCylinder, new Vector3(pickUpHighQualityWaterLoc[0], pickUpHighQualityWaterLoc[1], pickUpHighQualityWaterLoc[2]),
                new Vector3(pickUpHighQualityWaterLoc[0], pickUpHighQualityWaterLoc[1], pickUpHighQualityWaterLoc[2]), new Vector3(1, 1, 1), new Vector3(1, 1, 1),
                System.Drawing.Color.FromArgb(59, 134, 255));

            World.DrawMarker(MarkerType.VerticalCylinder, new Vector3(pickUpLowQualitySeedsLoc[0], pickUpLowQualitySeedsLoc[1], pickUpLowQualitySeedsLoc[2]),
                new Vector3(pickUpLowQualitySeedsLoc[0], pickUpLowQualitySeedsLoc[1], pickUpLowQualitySeedsLoc[2]), new Vector3(1, 1, 1), new Vector3(1, 1, 1),
                System.Drawing.Color.FromArgb(59, 134, 255));

            World.DrawMarker(MarkerType.VerticalCylinder, new Vector3(pickUpMediumQualitySeedsLoc[0], pickUpMediumQualitySeedsLoc[1], pickUpMediumQualitySeedsLoc[2]),
                new Vector3(pickUpMediumQualitySeedsLoc[0], pickUpMediumQualitySeedsLoc[1], pickUpMediumQualitySeedsLoc[2]), new Vector3(1, 1, 1), new Vector3(1, 1, 1),
                System.Drawing.Color.FromArgb(59, 134, 255));

            World.DrawMarker(MarkerType.VerticalCylinder, new Vector3(pickUpHighQualitySeedsLoc[0], pickUpHighQualitySeedsLoc[1], pickUpHighQualitySeedsLoc[2]),
                new Vector3(pickUpHighQualitySeedsLoc[0], pickUpHighQualitySeedsLoc[1], pickUpHighQualitySeedsLoc[2]), new Vector3(1, 1, 1), new Vector3(1, 1, 1),
                System.Drawing.Color.FromArgb(59, 134, 255));

            World.DrawMarker(MarkerType.VerticalCylinder, new Vector3(managementLoc[0], managementLoc[1], managementLoc[2]),
                new Vector3(managementLoc[0], managementLoc[1], managementLoc[2]), new Vector3(1, 1, 1), new Vector3(1, 1, 1),
                System.Drawing.Color.FromArgb(47, 189, 233));
        }

        private void sendMsg(string msg)
        {
            ChatMessage(msg);
        }
        private void showNotification(string msg)
        {
            ShowHelp(HelpType.onestring, msg);
            if (msg == "Nie masz wystarczająco ziaren z żadnej jakości") { menuEnabled = false; }
            else if (msg == "Naciśnij ~INPUT_PICKUP~ aby otworzyć menu") { menuEnabled = true; }
        }
        int y = 0;
        private void HaveJob(bool boolean)
        {
            //ChatMessage(boolean.ToString());
            if (y == 0) { if (boolean) { mainCaffeJob(); y = 1; } }
            caffeJob = boolean;
        }

        int id = 0;
        int rank = 0;
        string steamIdentifier;


        private void playerInformation(int id_, int rank_, string steamIdentifier_)
        {
            id = id_;
            rank = rank_;
            steamIdentifier = steamIdentifier_;

            if (rank_ > 2) loadAdditionalMenus();
        }
        private void passBool(bool boolean, string type)
        {
            if(type.ToUpper() == "LOW")
            {
                if (!boolean)
                {
                    itemMakeLowCoffe.Enabled = false;
                    if (!itemMakeLowCoffe.Text.Contains(" [brak składników]")) itemMakeLowCoffe.Text += " [brak składników]";
                    itemMakeLowCoffe.SetRightBadge(UIMenuItem.BadgeStyle.Lock);
                }
                else if (boolean)
                {
                    itemMakeLowCoffe.Enabled = true;
                    itemMakeLowCoffe.Text = itemMakeLowCoffe.Text.Replace(" [brak składników]", "");
                    itemMakeLowCoffe.SetRightBadge(UIMenuItem.BadgeStyle.None);
                }
            }
            if(type.ToUpper() == "MEDIUM")
            {
                if (!boolean)
                {
                    itemMakeMediumCoffe.Enabled = false;
                    if(!itemMakeMediumCoffe.Text.Contains(" [brak składników]")) itemMakeMediumCoffe.Text += " [brak składników]";
                    itemMakeMediumCoffe.SetRightBadge(UIMenuItem.BadgeStyle.Lock);
                }
                else if (boolean)
                {
                    itemMakeMediumCoffe.Enabled = true;
                    itemMakeMediumCoffe.Text = itemMakeMediumCoffe.Text.Replace(" [brak składników]", "");
                    itemMakeMediumCoffe.SetRightBadge(UIMenuItem.BadgeStyle.None);
                }
            }
            if(type.ToUpper() == "HIGH")
            {
                if (!boolean)
                {
                    itemMakeHighCoffe.Enabled = false;
                    if (!itemMakeHighCoffe.Text.Contains(" [brak składników]")) itemMakeHighCoffe.Text += " [brak składników]";
                    itemMakeHighCoffe.SetRightBadge(UIMenuItem.BadgeStyle.Lock);
                }
                else if (boolean)
                {
                    itemMakeHighCoffe.Enabled = true;
                    itemMakeHighCoffe.Text = itemMakeHighCoffe.Text.Replace(" [brak składników]", "");
                    itemMakeHighCoffe.SetRightBadge(UIMenuItem.BadgeStyle.None);
                }
            }
        }


        
        private MenuPool menuPool;
        private UIMenu menu;
        private UIMenuItem itemMakeLowCoffe;
        private UIMenuItem itemMakeMediumCoffe;
        private UIMenuItem itemMakeHighCoffe;

        private UIMenu managementMenu;
        private UIMenuItem itemChangeRank;
        private UIMenuItem itemFireOff;
        private UIMenuItem itemEmploy;

        private UIMenu management_Ranks;
        private UIMenu management_Fire;
        private UIMenu management_Employ;

        private void loadMenu()
        {
            menuPool = new MenuPool();
            menu = new UIMenu("Caffe", "Zrób kawę");
            itemMakeLowCoffe = new UIMenuItem("Zrób kawę słabej jakości", "~r~chujowa woda i chujowe ziarna");
            itemMakeMediumCoffe = new UIMenuItem("Zrób kawę średniej jakości", "~y~średnie ziarna i średnia woda");
            itemMakeHighCoffe = new UIMenuItem("Zrób kawę dobrej jakości", "~g~dobra woda i dobre ziarna");
            menuPool.Add(menu);
            menu.AddItem(itemMakeLowCoffe);
            menu.AddItem(itemMakeMediumCoffe);
            menu.AddItem(itemMakeHighCoffe);

            managementMenu = new UIMenu("Caffe", "Zarządzaj");
            itemChangeRank = new UIMenuItem("Zarządzaj stopniami");
            itemFireOff = new UIMenuItem("Zwolnij");
            itemEmploy = new UIMenuItem("Zatrudnij");
            menuPool.Add(managementMenu);
            managementMenu.AddItem(itemEmploy);
            managementMenu.AddItem(itemFireOff);
            managementMenu.AddItem(itemChangeRank);

            menu.OnItemSelect += Menu_OnItemSelect;
            managementMenu.OnItemSelect += ManagementMenu_OnItemSelect;

            TriggerServerEvent("caffePlayerInformation", Game.Player.ServerId);
        }
        private void loadAdditionalMenus()
        {
            management_Employ = new UIMenu("Caffe", "Zatrudnij !");
            menuPool.Add(management_Employ);
            management_Employ.OnItemSelect += Management_Employ_OnItemSelect;

            management_Ranks = new UIMenu("Caffe", "Zarządzaj !");
            menuPool.Add(management_Ranks);
            management_Ranks.OnItemSelect += Management_Ranks_OnItemSelect;

            management_Fire = new UIMenu("Caffe", "Zwolnij !");
            menuPool.Add(management_Fire);
            management_Fire.OnItemSelect += Management_Fire_OnItemSelect;
        }

        private void Menu_OnItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            if (sender != menu) return;

            if(selectedItem == itemMakeLowCoffe)
            {
                TriggerServerEvent("caffeMakeLow", Game.Player.ServerId);
            }
            else if(selectedItem == itemMakeMediumCoffe)
            {
                TriggerServerEvent("caffeMakeMedium", Game.Player.ServerId);
            }
            else if(selectedItem == itemMakeHighCoffe)
            {
                TriggerServerEvent("caffeMakeHigh", Game.Player.ServerId);
            }
            menu.Visible = false;
            menu.RefreshIndex();
            menuPool.CloseAllMenus();
            menuEnabled = false;
            menuEnabled = true;
        }
        private void ManagementMenu_OnItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            if (sender != managementMenu) return;

            LoadAndOpenMenu(index);
        }
        private void Management_Employ_OnItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            if (sender != management_Employ) return;

            if (selectedItem.Text.Contains("Zatrudnij"))
            {
                int employed = Convert.ToInt32(selectedItem.Text.Replace('[', ' ').Remove(4));
                TriggerServerEvent("caffeEmploy", LocalPlayer.ServerId, employed);
            }
        }
        private void Management_Ranks_OnItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            if (sender != menu) return;
        }
        private void Management_Fire_OnItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            //if (sender != menu) return;
            
            if(selectedItem.Text == "Zwolnij zaznaczonych")
            {
                string[] toFireUp = new string[30];
                
                for (int i = 0; i < sender.MenuItems.Count; i++)
                {
                    UIMenuCheckboxItem item = (UIMenuCheckboxItem)sender.MenuItems[i];
                    
                    if (!item.Checked) continue;
                    string fullName = item.Text.Replace("[Zwolnij]", ""); fullName.Replace("-", "");
                    fullName = fullName.Remove(fullName.Length);
                    sendMsg(fullName + " ---"); //de
                }
            }
        }
        private void LoadAndOpenMenu(int menu)
        {
            managementMenu.Visible = false;
            switch (menu)
            {
                case 0:
                    bool findsomeone = false;
                    foreach (var item in Players)
                    {
                        if (item == LocalPlayer) continue;
                        if (LocalPlayer.Character.Position.DistanceToSquared(item.Character.Position) < 3f)
                        {
                            UIMenuItem closestPlayer = new UIMenuItem($"[{item.ServerId}] - Zatrudnij");
                            management_Employ.AddItem(closestPlayer);
                            management_Employ.Visible = true;
                            findsomeone = true;
                        }
                        else if (findsomeone) Screen.ShowNotification("Brak osób w pobliżu !", true);
                    }
                    break;
                case 1:
                    TriggerServerEvent("caffeWorkers");
                    break;
                case 2:

                    break;
                default:
                    break;
            }
        }
        private async void workersClient(List<dynamic> workers)
        {
            TriggerServerEvent("caffePlayerInformation", Game.Player.ServerId);
            Screen.ShowNotification("~y~Ładowanie menu...");
            await Delay(6000);
            if (rank < 3) return;
            if(management_Fire.MenuItems.Count > 1) management_Fire.MenuItems.Clear();
            string[,] workersConverted = new string[30, 2];
            for (int i = 0; i < 29; i++)
            {
                workersConverted[i, 0] = workers[i];
                workersConverted[i, 1] = workers[i+1];
                i++;
            }
            /*List<dynamic> ranklist = new List<dynamic>()
            {
                "pracownik",
                "kierownik",
                "Zastępca szefa",
                "CEO"
            };*/
            for (int i = 0; i < 30; i++)
            {
                if (workersConverted[i, 0] == null) continue;
                management_Fire.AddItem(new UIMenuCheckboxItem($"[~r~Zwolnij~s~] {workersConverted[i, 0]} - {workersConverted[i, 1]}", false));
            }
            UIMenuItem fireUpSelected = new UIMenuItem("Zwolnij zaznaczonych");
            fireUpSelected.SetLeftBadge(UIMenuItem.BadgeStyle.Mask);
            management_Fire.AddItem(fireUpSelected);
            management_Fire.Visible = true;
            /*sendMsg(workersConverted[0, 0]);
            sendMsg(workersConverted[0, 1]);*/
        }




        private void drinkAnimation(string nothing)
        {
            drinkAnimationTask();
        }
        bool first_time_drinked = false;
        private async Task drinkAnimationTask()
        {
            var player = Game.PlayerPed;
            var prop = World.CreateProp(new Model(-598185919), player.Position, false, false);
            Prop coffeprop = null;
            if (!first_time_drinked) player.Task.PlayAnimation("amb@world_human_drinking@coffee@male@enter", "enter", 8f, 10, AnimationFlags.UpperBodyOnly);
            first_time_drinked = true;
            await Delay(10);
            foreach (var item in World.GetAllProps())
            {
                if (item.Model == new Model(-598185919) && player.Position.DistanceToSquared(item.Position) < 0.35f) { item.AttachTo(player.Bones[Bone.PH_R_Hand], default, new Vector3(0, 0, 0)); coffeprop = item; }
            }
            Function.Call(Hash.TASK_PLAY_ANIM, player.Handle, "amb@world_human_drinking@coffee@male@enter", "enter", 8f, 8f, 3000, 49, 0, 0, 0, 0);

            await Delay(2950);
            coffeprop.Delete();
        }

        private void mythNotification(string type, string message)
        {
            Exports["mythic_notify"].DoLongHudText(type, message);
        }
    }
}
