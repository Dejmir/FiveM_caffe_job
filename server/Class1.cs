using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;
using MySql.Data.MySqlClient;

using CitizenFX.Core;
using CitizenFX.Core.Native;
using static fivem_caffe_job.FuncHelper;

namespace fivem_caffe_job_server
{
    public class Class1 : BaseScript
    {
        dynamic ESX;
        public Class1()
        {
            TriggerEvent("esx:getSharedObject", new object[] { new Action<dynamic>(esx => {
            ESX = esx;
            })});

            EventHandlers["caffeHaveJob"] += new Action<int>(HaveJob);

            EventHandlers["caffeHaveSeeds"] += new Action<Player, string, string>(HaveEnoughItems);
            EventHandlers["caffeLowSeeds"] += new Action<Player, string>(LowSeeds);
            EventHandlers["caffeMediumSeeds"] += new Action<Player, string>(MediumSeeds);
            EventHandlers["caffeHighSeeds"] += new Action<Player, string>(HighSeeds);

            EventHandlers["caffeMakeLow"] += new Action<Player, string>(MakeLowCoffe);
            EventHandlers["caffeMakeMedium"] += new Action<Player, string>(MakeMediumCoffe);
            EventHandlers["caffeMakeHigh"] += new Action<Player, string>(MakeHighCoffe);

            EventHandlers["caffeDrinkLow"] += new Action<int>(DrinkLow);
            EventHandlers["caffeDrinkMedium"] += new Action<int>(DrinkMedium);
            EventHandlers["caffeDrinkHigh"] += new Action<int>(DrinkHigh);

            EventHandlers["caffeEmploy"] += new Action<Player, int, int>(Employ);
            EventHandlers["caffeFire"] += new Action<Player, int, string[]>(Fire); EventHandlers["caffeWorkers"] += new Action<Player>(Workers);
            EventHandlers["caffeRanks"] += new Action<Player, int, int, int>(Ranks);

            EventHandlers["caffePlayerInformation"] += new Action<Player, int>(PlayerInformation);
        }
        DB_helper DB = new DB_helper();
        private void LoadConfig()
        {

            JObject config;
            var configFileText = File.ReadAllText(@"resources\caffe_job\config.json");
            config = JObject.Parse(configFileText);
            string[][] configToPass = new string[2][]
            {
                new string []
                {
                    Convert.ToString(config.SelectToken("MakeCoffeLoc")).Split(',')[0],
                    Convert.ToString(config.SelectToken("MakeCoffeLoc")).Split(',')[1],
                    Convert.ToString(config.SelectToken("MakeCoffeLoc")).Split(',')[2],

                    Convert.ToString(config.SelectToken("pickUpLowQualityWaterLoc")).Split(',')[0],
                    Convert.ToString(config.SelectToken("pickUpLowQualityWaterLoc")).Split(',')[1],
                    Convert.ToString(config.SelectToken("pickUpLowQualityWaterLoc")).Split(',')[2],

                    Convert.ToString(config.SelectToken("pickUpMediumQualityWaterLoc")).Split(',')[0],
                    Convert.ToString(config.SelectToken("pickUpMediumQualityWaterLoc")).Split(',')[1],
                    Convert.ToString(config.SelectToken("pickUpMediumQualityWaterLoc")).Split(',')[2],

                    Convert.ToString(config.SelectToken("pickUpHighQualityWaterLoc")).Split(',')[0],
                    Convert.ToString(config.SelectToken("pickUpHighQualityWaterLoc")).Split(',')[1],
                    Convert.ToString(config.SelectToken("pickUpHighQualityWaterLoc")).Split(',')[2],

                    Convert.ToString(config.SelectToken("pickUpLowQualitySeedsLoc")).Split(',')[0],
                    Convert.ToString(config.SelectToken("pickUpLowQualitySeedsLoc")).Split(',')[1],
                    Convert.ToString(config.SelectToken("pickUpLowQualitySeedsLoc")).Split(',')[2],

                    Convert.ToString(config.SelectToken("pickUpMediumQualitySeedsLoc")).Split(',')[0],
                    Convert.ToString(config.SelectToken("pickUpMediumQualitySeedsLoc")).Split(',')[1],
                    Convert.ToString(config.SelectToken("pickUpMediumQualitySeedsLoc")).Split(',')[2],

                    Convert.ToString(config.SelectToken("pickUpHighQualitySeedsLoc")).Split(',')[0],
                    Convert.ToString(config.SelectToken("pickUpHighQualitySeedsLoc")).Split(',')[1],
                    Convert.ToString(config.SelectToken("pickUpHighQualitySeedsLoc")).Split(',')[2],

                    Convert.ToString(config.SelectToken("managementLoc")).Split(',')[0],
                    Convert.ToString(config.SelectToken("managementLoc")).Split(',')[1],
                    Convert.ToString(config.SelectToken("managementLoc")).Split(',')[2],
                },
                new string []
                {
                    //Miało być jagged array ale chuj bo fivem wtedy metody nie chce uruchomić, dlatego jest tak...
                }
            };

            //Players[1].TriggerEvent("caffeLoadConfig", configToPass);
            foreach (var player in Players)
            {
                //player.TriggerEvent("showNotification", "ez");
                player.TriggerEvent("caffeLoadConfig", configToPass);
            }

        }

        static int[] playerConfigLoaded = new int[9999];
        private void HaveJob(int id)
        {
            if(!playerConfigLoaded[id].Equals(1))
            {
                LoadConfig();
            }
            playerConfigLoaded[id] = 1;
            var player = ESX.GetPlayerFromId(id);
            string jobName = player.getJob().name;
            Player source = Players[id];
                if (jobName.ToUpper() == "CAFFE") source.TriggerEvent("caffeHaveJobClient", true);
                else source.TriggerEvent("caffeHaveJobClient", false);
        }
        private bool HaveJobBoolean(int id)
        {
            var player = ESX.GetPlayerFromId(id);
            string jobName = player.getJob().name;
            if (jobName.ToUpper() == "CAFFE") return true;
            else return false;
        }

        private void HaveEnoughItems([FromSource] Player source, string id, string handle)
        {
            Delay(250);

            var player = ESX.GetPlayerFromId(id);
            //player.addMoney(200);
            var seedsLow = player.getInventoryItem("ziarna_low").count;
            var seedsMedium = player.getInventoryItem("ziarna_medium").count;
            var seedsHigh = player.getInventoryItem("ziarna_high").count;
            if (false/*seedsLow < 3 && seedsMedium < 3 && seedsHigh < 3*/)
            {
                source.TriggerEvent("showNotification", "Nie masz wystarczająco ziaren lub wody z żadnej jakości");
            }
            else
            {
                source.TriggerEvent("showNotification", "Naciśnij ~INPUT_PICKUP~ aby otworzyć menu");
            }
        }

        private void LowSeeds([FromSource] Player source, string id)
        {
            var player = ESX.GetPlayerFromId(id);
            var seeds = player.getInventoryItem("ziarna_low").count;
            var water = player.getInventoryItem("woda_low").count;
            //source.TriggerEvent("sendMsg", $"{water}");

            if (seeds < 3 || water < 1)
            {
                source.TriggerEvent("caffePassBool", false, "low");
            }
            else if (seeds >= 3 && water > 0)
            {
                source.TriggerEvent("caffePassBool", true, "low");
            }
        }
        private void MediumSeeds([FromSource] Player source, string id)
        {
            var player = ESX.GetPlayerFromId(id);
            var seeds = player.getInventoryItem("ziarna_medium").count;
            var water = player.getInventoryItem("woda_medium").count;

            if (seeds < 3 || water < 1)
            {
                source.TriggerEvent("caffePassBool", false, "medium");
            }
            else if (seeds >= 3 && water > 0)
            {
                source.TriggerEvent("caffePassBool", true, "medium");
            }
        }
        private void HighSeeds([FromSource] Player source, string id)
        {
            var player = ESX.GetPlayerFromId(id);
            var seeds = player.getInventoryItem("ziarna_high").count;
            var water = player.getInventoryItem("woda_high").count;

            if (seeds < 3 || water < 1)
            {
                source.TriggerEvent("caffePassBool", false, "high");
            }
            else if (seeds >= 3 && water > 0)
            {
                source.TriggerEvent("caffePassBool", true, "high");
            }
        }

        private void MakeLowCoffe([FromSource] Player source, string id)
        {
            var player = ESX.GetPlayerFromId(id);

            player.removeInventoryItem("ziarna_low", 3);
            player.removeInventoryItem("woda_low", 1);
            player.addInventoryItem("kawa_low", 1);
        }
        private void MakeMediumCoffe([FromSource] Player source, string id)
        {
            var player = ESX.GetPlayerFromId(id);

            player.removeInventoryItem("ziarna_medium", 3);
            player.removeInventoryItem("woda_medium", 1);
            player.addInventoryItem("kawa_medium", 1);
        }
        private void MakeHighCoffe([FromSource] Player source, string id)
        {
            var player = ESX.GetPlayerFromId(id);

            player.removeInventoryItem("ziarna_high", 3);
            player.removeInventoryItem("woda_high", 1);
            player.addInventoryItem("kawa_high", 1);
        }

        private void DrinkLow(int id)
        {
            //TriggerClientEvent("sendMsg", $"Test raz kurwa dwa: {source.GetType().Name}");
            Player source = Players[id];
            Random random = new Random();
            int rng = random.Next(1, 100);
            if(rng < 5) source.TriggerEvent("caffeMythNotification", "inform", "Kawa była tak nie dobra że aż ci się słabo zrobiło.");
            else if (rng < 10) source.TriggerEvent("caffeMythNotification", "inform", "Masz odruchy wymiotne po tej kawie.");
            else if (rng < 15) source.TriggerEvent("caffeMythNotification", "inform", "Kawa smakuje jak szczyny psa");
            else if (rng < 30) source.TriggerEvent("caffeMythNotification", "inform", "Kawa smakuje jak przeterminowana z 7 dni.");
            else if (rng < 60) source.TriggerEvent("caffeMythNotification", "inform", "Dostrzegasz nie smaczny smak kawy.");
            else if (rng < 80) source.TriggerEvent("caffeMythNotification", "inform", "Wyczuwasz że kawa nie jest najświeższej jakości.");
            else if (rng < 90) source.TriggerEvent("caffeMythNotification", "inform", "Nie wyczuwasz smaku kawy.");
            else if (rng < 100) source.TriggerEvent("caffeMythNotification", "inform", "Kawa była dobra.");
        }
        private void DrinkMedium(int id)
        {
            Player source = Players[id];
            Random random = new Random();
            int rng = random.Next(1, 100);
            if (rng < 25) source.TriggerEvent("caffeMythNotification", "inform", "Kawa była dobra.");
            else if (rng < 50) source.TriggerEvent("caffeMythNotification", "inform", "Wyczuwasz świeżą jakość ziaren.");
            else if (rng < 75) source.TriggerEvent("caffeMythNotification", "inform", "Wyczuwasz dobrej jakości składniki.");
            else if (rng < 100) source.TriggerEvent("caffeMythNotification", "inform", "Kawa była świetna !");
        }
        private void DrinkHigh(int id)
        {
            Player source = Players[id];
            Random random = new Random();
            int rng = random.Next(1, 100);
            if (rng < 10) source.TriggerEvent("caffeMythNotification", "inform", "Kawa była tak dobra że poczułeś orgrazm !");
            else if (rng < 30) source.TriggerEvent("caffeMythNotification", "inform", "Kawa była świetna !");
            else if (rng < 50) source.TriggerEvent("caffeMythNotification", "inform", "Kawa smakuje jak prosto z nieba !");
            else if (rng < 70) source.TriggerEvent("caffeMythNotification", "inform", "Kawa smakuje jak TOP 1 !");
            else if (rng < 100) source.TriggerEvent("caffeMythNotification", "inform", "Kawa jest idealna !");
        }

        private void Employ([FromSource] Player player, int playerid, int employed)
        {
            if (HaveJobBoolean(playerid))
            {
                var esxplayer = ESX.GetPlayerFromId(employed);
                Player employedPlayer = Players[employed];
                esxplayer.setJob("caffe", 0);
                MySqlConnection connection = new MySqlConnection(DB_helper.connect);
                var result = DB.ExecuteQuery("SELECT * FROM caffe_job", connection);
                string firstname = "";
                string lastname = "";
                while (result.Read())
                {
                    firstname = result.GetString("firstname");
                    lastname = result.GetString("lastname");
                }
                result.Close();
                connection.Close();
                DB.ExecuteNonQuery($"INSERT INTO caffe_job (steam, firstname, lastname, rank) VALUES ({employedPlayer.Identifiers["steam"]}, {firstname}, {lastname}, 1)");
            }
            else
            {
                player.Drop("Cheaty?");
                //ban or log
            }
        }
        private void Fire([FromSource] Player player, int playerid, string[] fired)
        {
            if (HaveJobBoolean(playerid))
            {
                for (int i = 0; i < fired.Length; i++)
                {
                    Player firedPlayer = Players[i];
                    DB.ExecuteNonQuery($"DELETE FROM caffe_job WHERE steam={firedPlayer.Identifiers["steam"]}");
                    DB.ExecuteNonQuery($"UPDATE caffe_job SET job=unemployed, job_grade=0 WHERE steam={firedPlayer.Identifiers["steam"]}");
                }
            }
        }
        private void Ranks([FromSource] Player player, int playerid, int changedplayer, int rank)
        {

        }

        private void Workers([FromSource] Player player)
        {
            string[,] workers = new string[30, 2];

            MySqlConnection connection = new MySqlConnection(DB_helper.connect);
            var result = DB.ExecuteQuery("SELECT * FROM caffe_job", connection);
            int i = 0;
            while (result.Read()) 
            {
                string firstname = result.GetString("firstname");
                string lastname = result.GetString("lastname");
                workers[i, 0] = firstname + " " + lastname;
                workers[i, 1] = result.GetInt32("rank").ToString();
                i++;
            }
            i = 0;
            result.Close();
            connection.Close();
            player.TriggerEvent("caffeWorkersClient", workers);
        }
        
        void PlayerInformation([FromSource] Player player, int playerId)
        {
            Player checkPlayer = Players[playerId];
            if (player != checkPlayer) return;
            int id = playerId;
            int rank = 0;
            string steam = player.Identifiers["steamid"];

            MySqlConnection connection = new MySqlConnection(DB_helper.connect);
            var result = DB.ExecuteQuery("SELECT * FROM caffe_job", connection);
            while (result.Read())
            {
                rank = result.GetInt32("rank");
            }
            result.Close();
            connection.Close();

            player.TriggerEvent("caffePlayerInformationClient", id, rank, steam);
        }
    }
}
