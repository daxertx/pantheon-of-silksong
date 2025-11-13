using BepInEx;
using BepInEx.Logging;
using GenericVariableExtension;
using GlobalEnums;
using GlobalSettings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace SilksongBossRush
{
    [BepInPlugin("com.silksong.bossrush", "Act 1 Boss Rush", "2.2.0")]
    public class all3ActBossRushPlugin : BaseUnityPlugin
    {
        public bool gotalltools = false;
        private ManualLogSource log;
        private bool isTransitioning = false;

        private bool act1done = false;
        private bool act2done = false;
        private bool act3done = false;
        public int a = 0;
        public bool temp1 = true;
        public bool temp2 = true;
        public bool temp3 = true;

        public bool bench0 = true;
        public bool bench1 = true;
        public bool bench2 = true;
        public bool bench3 = true;
        public bool bench4 = true;
        public bool bench5 = true;
        public bool bench6 = true;
        
        public GameObject popupPrefab;

        private class BossData
        {
            public string Name;
            public string SceneName;
            public string GateName;
            public string PlayerDataFlag;
            public string[] AdditionalFlags;
            public Vector3? CustomPosition;

            
            
            public BossData(string name, string scene, string gate, string flag, params string[] additionalFlags)
            {
                Name = name;
                SceneName = scene;
                GateName = gate;
                PlayerDataFlag = flag;
                AdditionalFlags = additionalFlags;
                CustomPosition = null;
            }

            public BossData(string name, string scene, Vector3 pos, string flag, params string[] additionalFlags)
            {
                Name = name;
                SceneName = scene;
                GateName = null;
                PlayerDataFlag = flag;
                AdditionalFlags = additionalFlags;
                CustomPosition = pos;
            }
        }

        private List<BossData> act1Bosses = new List<BossData>
        {
            new BossData("Moss Mother", "Tut_03", "right1", "defeatedMossMother"),//good
            new BossData("Bell Beast", "Bone_05", new Vector3(83.40305f,3.567686f,0.004f), "defeatedBellBeast"),//good
            new BossData("Lace", "Bone_East_12", new Vector3(92.13885f,7.567686f,0.004f), "defeatedLace1", "laceLeftDocks"),//good
            new BossData("Fourth Chorus", "Bone_East_08",new Vector3(82.46f,12.26745f,0.004f), "defeatedFourthChorus"),// good
            new BossData("Savage Beastfly", "Ant_19", new Vector3(50.61314f,34.56768f,0.004f), "defeatedSavageBeastfly"),//good
            new BossData("Sister Splinter", "Shellwood_18", new Vector3(55.44654f,8.567684f,0.004f), "defeatedSplinterQueen"),//good
            new BossData("Skull Tyrant", "Bone_15", new Vector3(74.93963f,14.56769f,0.004f), "skullKingDefeated"),//good
            new BossData("moorwing", "Greymoor_08", new Vector3(37.91465f, 4.567685f, 0.004f), ""),//check
            new BossData("Widow", "Belltown_Shrine", new Vector3(51.51193f,8.567684f,0.004f), "hasNeedolin"),//good
            new BossData("MossEvolver", "Weave_03", new Vector3(32.89833f,20.56768f,0.004f), "defeatedMossEvolver"),//good
            new BossData("Great Conchflies", "Coral_11", new Vector3(54.95525f, 14.56769f, 0.004f), "defeatedCoralDrillers"),//good
            new BossData("Phantom", "Organ_01", new Vector3(83.79173f,104.5677f,0.004f), "defeatedPhantom"),//good
            new BossData("Last Judge", "Coral_Judge_Arena", new Vector3(45.96228f,24.56768f,0.004f), "defeatedLastJudge"),//good
        };
        private List<BossData> act2Bosses = new List<BossData>
        {
            new BossData("Cog Dancers", "Cog_Dancers",new Vector3(40f,4.59697f,0.004f), "defeatedCogworkDancers"),//good
            new BossData("Trobbio", "Library_13",new Vector3(74.04997f,14.56769f,0.004f), "defeatedTrobbio"),//good
            new BossData("Garmond and Zaza", "Library_09", new Vector3(77.44103f, 15.56768f, 0.004f), "garmondLibraryDefeatedHornet"),
            new BossData("Forebrothers Signis and Gron", "Dock_09",new Vector3(34.44567f,7.567685f,0.004f), "defeatedDockForemen"),//good
            //new BossData("The Unravelled", "Ward_02",new Vector3(49.67819f,6.567685f,0.004f), "defeatedGreyWarrior"),//need fix
            new BossData("Disgraced Chef Lugoli","Dust_Chef",new Vector3(44.87098f,35.58335f,0.004f), "defeatedRoachkeeperChef"),//good
            new BossData("Father Of The Flame", "Belltown_08",new Vector3(56.90835f, 11.56769f, 0.004f), "defeatedWispPyreEffigy"),//good
            new BossData("Groal The Great", "Shadow_18",new Vector3(65.7369f, 11.44238f, 0.004f), "DefeatedSwampShaman"),//good
            new BossData("Voltvyrm", "Coral_29", new Vector3(170.0934f, 24.56778f, 0.004f), "defeatedZapCoreEnemy"),//good
            new BossData("Raging Conchfly", "Coral_27", new Vector3(20.88691f, 33.56768f, 0.004f), "defeatedCoralDrillerSolo"),//good
            new BossData("First Sinner", "Slab_10b", new Vector3(43.05967f, 9.567684f, 0.004f), "defeatedFirstWeaver"),//good
            new BossData("Broodmother", "Slab_16b", new Vector3(60.44946f, 5.567685f, 0.004f), "idk"),//req quest maybe
            //shakara req special conditions
            
            new BossData("Second Sentinel", "Hang_17b", new Vector3(41.44738f, 4.567685f, 0.004f), "defeatedSongChevalierBoss"),//good
            new BossData("Lace2", "Song_Tower_01", new Vector3(50.84962f, 100.5677f, 0.004f), "defeatedLaceTower"),//good
            new BossData("Grand Mother Silk", "Cradle_03", new Vector3(48.20269f, 133.5677f, 0.004f), "idk"),//good
            new BossData("craw father","Room_CrowCourt_02",new Vector3(34.70296f,21.56768f,0.004f),"PickedUpCrowMemento")//good
            //the unraveelled doesnt spawn idk why yet probably bc key
            
            
        };
        private List<BossData> act3Bosses = new List<BossData>
        {
            //new BossData("Bell Eater","Bellway_Centipede_Arena", new Vector3(138.1828f, 7.567685f, 0.004f),"idk"),//boss exit problem
            ////error
            //new BossData("gardmond","","","idk"),//?
            //new BossData("palestag","Clover_19", new Vector3(24.94909f, 12.56769f, 0.004f),"idk"),//good
            new BossData("clover dancers","Clover_10",new Vector3(83.90851f,37.56768f,0.004f),"defeatedCloverDancers"),//good
            new BossData("seth","Shellwood_22", new Vector3(114.3001f,6.567686f,0.004f),"defeatedSeth"),//good
            new BossData("nyleth","Shellwood_11b_Memory", "right1","CollectedHeartFlower"),//good
            new BossData("karmelita","Memory_Ant_Queen", new Vector3(149.1029f,19.56768f,0.004f),"CollectedHeartHunter"),//good
            new BossData("Crust King Khann","Memory_Coral_Tower","door_wakeInMemory","CollectedHeartCoral"),//good
            new BossData("lost lace","Abyss_Cocoon","door_entry",""),//good
            new BossData("Pinstress","Peak_07",new Vector3(32.55001f,88.56767f,0.004f),"idk"),//need check
            new BossData("Gurr The Outcast","Bone_East_18b",new Vector3(172.4879f,79.56767f,0.004f),"defeatedAntTrapper"),//good
            new BossData("Trobbio2","Library_13",new Vector3(74.04997f,14.56769f,0.004f), "defeatedTormentedTrobbio")//check

        };

        private void Awake()
        {

            log = Logger;
            log.LogInfo("========================================");
            log.LogInfo("ACT 1 BOSS RUSH MOD LOADED!");
            log.LogInfo("========================================");
            log.LogInfo("Press 1: Move to next boss");
            log.LogInfo("Press 2: To tp to bench");
        }

        private void Start()
        {
            //give all tools
            
        }
        private void Update()
        {
            PlayerData pd = PlayerData.instance;
            if (pd == null)
                return;


            if ((Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.JoystickButton9)) && !isTransitioning)
            {

                if (!act1done)
                    movebossact1();
                else if (!act2done)
                    movebossact2();
                else if (!act3done)
                    movebossact3();
                else
                    log.LogInfo("u finished the phantheon gg");
                
            }
        }
        private void unlockallquests()
        {
            foreach(var quest in QuestManager.GetAllQuests())
            {
                QuestManager.GetQuest(quest.name).BeginQuest(null);
            }
        }
        private void getcords()
        {
            var player = HeroController.instance.transform.position;
            log.LogInfo(player.x+" "+player.y+" "+player.z);
        }
        private void ShowSceneInfo()
        {
            string scene = SceneManager.GetActiveScene().name;
            log.LogInfo($"Current Scene: {scene}");
            foreach (var gate in FindObjectsOfType<TransitionPoint>())
                log.LogInfo($"Gate: {gate.name}");
        }


        private void TeleportToBossSmart(BossData boss)
        {
            if (boss == null || isTransitioning)
            {
                log.LogError("BossData is null or already transitioning!");
                return;
            }

            if (GameManager.instance == null || PlayerData.instance == null)
            {
                log.LogError("GameManager or PlayerData missing!");
                return;
            }

            isTransitioning = true;
            var pd = PlayerData.instance;

            // Use custom position or gate
            if (boss.CustomPosition.HasValue)
            {
                //got vector 3
                log.LogInfo($"Teleporting to {boss.Name} using GameManager with position...");
                StartCoroutine(TeleportWithPosition(boss));
            }
            else
            {
                log.LogInfo($"Teleporting to {boss.Name} via gate {boss.GateName}...");
                GameManager.instance.BeginSceneTransition(new GameManager.SceneLoadInfo
                {
                    SceneName = boss.SceneName,
                    EntryGateName = boss.GateName,
                    PreventCameraFadeOut = false,
                    WaitForSceneTransitionCameraFade = true,
                    Visualization = GameManager.SceneLoadVisualizations.Default
                });
                StartCoroutine(ResetTransitionFlag(1f));
            }
        }

        private IEnumerator TeleportWithPosition(BossData boss)
        {
            // Use GameManager to load the scene properly
            if(boss.Name== "Cog Dancers")
            {
                GameManager.instance.BeginSceneTransition(new GameManager.SceneLoadInfo
                {
                    SceneName = boss.SceneName,
                    EntryGateName = "left1",
                    PreventCameraFadeOut = false,
                    WaitForSceneTransitionCameraFade = true,
                    Visualization = GameManager.SceneLoadVisualizations.Default
                });
            }
            else
            {
                GameManager.instance.BeginSceneTransition(new GameManager.SceneLoadInfo
                {
                    SceneName = boss.SceneName,
                    EntryGateName = "top1", // Use any gate to trigger proper load
                    PreventCameraFadeOut = false,
                    WaitForSceneTransitionCameraFade = true,
                    Visualization = GameManager.SceneLoadVisualizations.Default
                });
            }
                yield return new WaitForSeconds(0.5f);

            while (SceneManager.GetActiveScene().name != boss.SceneName)
                yield return new WaitForSeconds(0.1f);

            // Wait a bit more for scene initialization
            yield return new WaitForSeconds(0.3f);

            // Move hero to custom position
            if (HeroController.instance != null && boss.CustomPosition.HasValue)
            {
                HeroController.instance.transform.position = boss.CustomPosition.Value;
                log.LogInfo($"Moved hero to {boss.CustomPosition.Value}");
            }

            isTransitioning = false;
        }

        private IEnumerator ResetTransitionFlag(float delay)
        {
            yield return new WaitForSeconds(delay);
            isTransitioning = false;
        }

        private void movebossact1()
        {
            var pd = PlayerData.instance;
            if (pd == null)
            {
                log.LogWarning("PlayerData missing!");
                return;
            }

            
            if (!pd.defeatedMossMother)
            {
                StartCoroutine(UnlockAllCoroutine());
                TeleportToBossSmart(act1Bosses[0]);
                log.LogInfo("Going to Moss Mother");
            }
            else if(bench0)
            {
                bench0 = false;
                bench();
            }
            else if (!pd.defeatedBellBeast)
            {
                TeleportToBossSmart(act1Bosses[1]);
                log.LogInfo("Going to Bell Beast");
                pd.hasSilkSpecial = true;
                pd.hasNeedleThrow = true;
                UnlockStations();


            }
            else if (!pd.defeatedLace1)
            {
                TeleportToBossSmart(act1Bosses[2]);
                log.LogInfo("Going to Lace");
                pd.hasDash = true;
            }
            else if (!pd.defeatedSongGolem)
            {
                //give hp
                if (temp1)
                {
                    pd.AddToMaxHealth(1);

                    log.LogInfo("hp added now player is " + pd.maxHealth + "hp");
                    temp1 = false;
                }
                TeleportToBossSmart(act1Bosses[3]);
                log.LogInfo("Going to Fourth Chorus");
                pd.hasBrolly = true;
            }
            else if (!pd.completedMemory_beast)
            {
                TeleportToBossSmart(act1Bosses[4]);
                log.LogInfo("Going to Savage Beastfly");
                if (temp2)
                {
                    pd.AddToMaxHealth(1);
                    temp2 = false;
                }
                
                //get beast crest
            }
            else if (bench1)
            {
                bench1 = false;
                unlockcrest(7);
                unlockcrest(6);
                bench();
            }
            else if (!pd.defeatedSplinterQueen)
            {
                TeleportToBossSmart(act1Bosses[5]);
                log.LogInfo("Going to Sister Splinter");
                pd.completedMemory_beast = true;
                //get wall jump
            }
            else if (!pd.skullKingDefeated)
            {
                pd.hasWalljump = true;
                TeleportToBossSmart(act1Bosses[6]);
                log.LogInfo("Going to Skull tyrant");

            }
            else if (!pd.visitedBellhart)
            {
                TeleportToBossSmart(act1Bosses[7]);
                log.LogInfo("Going to moorwing");


            }
            else if (!pd.hasNeedolin)
            {
                TeleportToBossSmart(act1Bosses[8]);
                log.LogInfo("Going to Widow");
                //get needoline
            }
            else if (bench2)
            {
                pd.nailUpgrades = 1;
                bench2 = false;
                unlockcrest(8);
                bench();
            }
            else if (!pd.defeatedCoralDrillers)
            {
                TeleportToBossSmart(act1Bosses[10]);
                log.LogInfo("Going to Great Conchflies");
                //get needolin from boss no need to give

            }
            else if (!pd.defeatedPhantom)
            {
                pd.hasChargeSlash = true;

                TeleportToBossSmart(act1Bosses[11]);
                log.LogInfo("Going to Great Conchflies");
            }
            else if (!pd.defeatedLastJudge)
            {
                TeleportToBossSmart(act1Bosses[12]);
                log.LogInfo("Going to Last Judge");

                //5gates opened

                pd.bellShrineBellhart = true;
                pd.bellShrineBoneForest = true;
                pd.bellShrineEnclave = true;
                pd.bellShrineGreymoor = true;
                pd.bellShrineShellwood = true;
                pd.bellShrineWilds = true;



                pd.aspid_04_gate = true;
                pd.gatePilgrimNoNeedolinConvo = true;
                pd.slab_cloak_gate_reopened = true;
                pd.slab_05_gateOpen = true;
                pd.slab_07_gateOpen = true;
                pd.SeenLastJudgeGateOpen = true;
                pd.cog7_gateOpened = true;
                pd.greatBoneGateOpened = true;
                pd.openedDust05Gate = true;
                pd.visitedGrandGate = true;

            }
            else
            {
                //give hp
                if (temp3)
                {
                    pd.AddToMaxHealth(1);
                    log.LogInfo("hp added now player is " + pd.maxHealth + "hp");
                }

                log.LogInfo("========================================");
                log.LogInfo("ALL ACT 1 BOSSES DEFEATED!");
                log.LogInfo("========================================");
                log.LogInfo("========================================");
                log.LogInfo("ACT 2 BOSS RUSH STARTED!");
                log.LogInfo("========================================");
                act1done = true;
            }
        }
        private void movebossact2()
        {
            PlayerData.instance.act2Started = true;
            PlayerData.instance.cog7_automatonRepairingComplete = true;

            var pd = PlayerData.instance;
            if (pd == null)
            {
                log.LogWarning("PlayerData missing!");
                return;
            }

            pd.completedMemory_reaper = true;
            pd.completedMemory_shaman = true;
            pd.completedMemory_toolmaster = true;
            pd.completedMemory_wanderer = true;
            pd.completedMemory_witch = true;


            if (!pd.defeatedCogworkDancers)
            {
                //give hp
                
                pd.AddToMaxHealth(1);
                log.LogInfo("hp added now player is " + pd.maxHealth + "hp");

                TeleportToBossSmart(act2Bosses[0]);
                log.LogInfo("Going to gogwork dancers");
                log.LogInfo("nail in last upgrade");
                pd.silkMax = 12;
            }
            else if(bench3)
            {
                pd.nailUpgrades = 2;
                bench3 = false;
                bench();
            }
            else if (!pd.defeatedTrobbio)
            {
                pd.defeatedTormentedTrobbio = true;
                TeleportToBossSmart(act2Bosses[1]);
                log.LogInfo("Going to Trobbio");
                pd.hasDoubleJump = true;
                pd.hasHarpoonDash = true;
            }
            else if (!pd.defeatedDockForemen)
            {
                TeleportToBossSmart(act2Bosses[3]);
                log.LogInfo("Going to Forebrothers Signis and Gron");
            }
            else if (!pd.defeatedRoachkeeperChef)
            {
                TeleportToBossSmart(act2Bosses[4]);
                log.LogInfo("Going to Disgraced Chef Lugoli");
            }
            else if (!pd.defeatedWispPyreEffigy)
            {
                TeleportToBossSmart(act2Bosses[5]);
                log.LogInfo("Going to Father Of The Flame");
            }
            else if (!pd.DefeatedSwampShaman)
            {
                TeleportToBossSmart(act2Bosses[6]);
                log.LogInfo("Going to Groal The Great");
            }
            else if (!pd.defeatedZapCoreEnemy)
            {
                TeleportToBossSmart(act2Bosses[7]);
                log.LogInfo("Going to Voltvyrm");
            }
            else if (!pd.defeatedCoralDrillerSolo)
            {
                TeleportToBossSmart(act2Bosses[8]);
                log.LogInfo("Going to Raging Conchfly");
            }

            else if (!pd.hasSilkBomb)
            {
                pd.nailUpgrades = 3;
                TeleportToBossSmart(act2Bosses[9]);
                log.LogInfo("Going to First Sinner");
            }
            else if (!pd.defeatedBroodMother)
            {
                TeleportToBossSmart(act2Bosses[10]);
                log.LogInfo("Going to Broodmother");
            }
            else if (bench4)
            {
                bench4 = false;
                bench();
            }
            else if (!pd.PickedUpCrowMemento)
            {
                TeleportToBossSmart(act2Bosses[14]);
                log.LogInfo("Going to Craw father");
            }

            else if (!pd.defeatedSongChevalierBoss)
            {
                TeleportToBossSmart(act2Bosses[11]);
                log.LogInfo("Going to Second Sentinel");
            }
            else if (!pd.defeatedLaceTower)
            {
                //need check
                TeleportToBossSmart(act2Bosses[12]);
                log.LogInfo("Going to Lace 2");
            }
            else if (pd.CompletedEndings != SaveSlotCompletionIcons.CompletionState.Act2SoulSnare)
            {
                pd.hasNeedolinMemoryPowerup = true;
                pd.hasSuperJump = true;
                PlayerData.instance.soulSnareReady = true;
                TeleportToBossSmart(act2Bosses[13]);
                log.LogInfo("Going to Grand Mother Silk");
            }
            else
            {
                log.LogInfo("===================================");
                log.LogInfo("ALL ACT 2 BOSSES DEFEATED!");
                log.LogInfo("===================================");
                act2done = true;
            }

        }
        private void movebossact3()
        {
            try
            {
                var pd = PlayerData.instance;
                if (pd == null)
                {
                    log.LogWarning("PlayerData missing!");
                    return;
                }

                else if (bench5)
                {
                    pd.nailUpgrades = 4;
                    bench5 = false;
                    unlockcrest(5);
                    unlockcrest(9);
                    bench();
                }
                else if(!pd.defeatedTormentedTrobbio)
                {
                    QuestManager.GetQuest("Tormented Trobbio").BeginQuest(null);
                    pd.defeatedTormentedTrobbio = false;
                    TeleportToBossSmart(act3Bosses[8]);
                    log.LogInfo("Going to trobbio 2");
                    
                }
                if (!pd.CollectedHeartClover)
                {
                    TeleportToBossSmart(act3Bosses[0]);
                    log.LogInfo("Going to Pinstress");
                    QuestManager.GetQuest("Pinstress Battle").BeginQuest(null);
                }

                else if (!QuestManager.GetQuest("Pinstress Battle").IsCompleted)
                {
                    TeleportToBossSmart(act3Bosses[6]);
                    log.LogInfo("going to pinstress");
                }

                else if (!pd.defeatedSeth)
                {
                    //fight seth
                    TeleportToBossSmart(act3Bosses[1]);
                    log.LogInfo("Going to Seth");
                }
                else if (!pd.CollectedHeartFlower)
                {
                    TeleportToBossSmart(act3Bosses[2]);
                    log.LogInfo("Going to nyleth");
                }
                else if (!pd.defeatedAntTrapper)
                {
                    //trapper
                    QuestManager.GetQuest("Ant Trapper").BeginQuest(null);
                    TeleportToBossSmart(act3Bosses[7]);
                    log.LogInfo("Gurr The Outcast");
                }
                else if (!pd.CollectedHeartHunter)
                {
                    //karamlita
                    TeleportToBossSmart(act3Bosses[3]);
                    log.LogInfo("Going to karmelita");
                }
                else if (bench6)
                {
                    bench6 = false;
                    bench();
                }
                else if (!pd.CollectedHeartCoral)
                {
                    //crab arena
                    TeleportToBossSmart(act3Bosses[4]);
                    log.LogInfo("Going to Coral King Heart Arena");
                }
                else if (!act3done)
                {
                    //fight lost lace
                    TeleportToBossSmart(act3Bosses[5]);
                    log.LogInfo("Going to Lost Lace");
                }
                else
                {
                    log.LogInfo("===================================");
                    log.LogInfo("ALL ACT 3 BOSSES DEFEATED!");
                    log.LogInfo("===================================");
                    act3done = true;
                }
            }
            catch (Exception e)
            {
                log.LogInfo($"ERROR");
            }
        }

        private System.Collections.IEnumerator UnlockAllCoroutine()
        {

            // Wait until the player data and tool manager are ready
            yield return new WaitUntil(() => PlayerData.instance != null && ToolItemManager.GetAllTools() != null);

            int unlockedCount = 0;

            foreach (var tool in ToolItemManager.GetAllTools())
            {
                if (tool == null)
                    continue;

                if (!tool.IsUnlocked  && !tool.name.Contains("Dazzle Bind"))
                {
                    // Unlock without tutorial popups
                    tool.Unlock(null, ToolItem.PopupFlags.None);
                    unlockedCount++;
                }
            }

            log.LogInfo($"Unlocked {unlockedCount} tools!");
            gotalltools = true;
        }



        public void UnlockStations()
        {
            var pd = PlayerData.instance;
            if (pd == null)
            {
                Debug.LogError("❌ PlayerData.instance is null!");
                return;
            }

            int count = 0;
            var fields = pd.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                // Find bool fields with "station" in their name
                if (field.FieldType == typeof(bool) && field.Name.ToLower().Contains("station"))
                {
                    try
                    {

                        field.SetValue(pd, true);
                        count++;
                        Debug.Log($"✅ Unlocked station: {field.Name}");
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"⚠️ Failed to unlock {field.Name}: {e.Message}");
                    }
                }
            }

            Debug.Log($"All stations unlocked! ({count} total)");
        }
        public void PrintAllPlayerData()
        {
            var pd = PlayerData.instance;
            if (pd == null)
            {
                Debug.LogError("❌ PlayerData.instance is null!");
                return;
            }

            Type pdType = pd.GetType();
            FieldInfo[] fields = pdType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            Debug.Log($"----- 🧾 PlayerData Dump: {fields.Length} fields -----");

            foreach (var field in fields)
            {
                try
                {
                    object value = field.GetValue(pd);
                    string valString = value == null ? "null" : value.ToString();
                    Debug.Log($"{field.Name} = {valString}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error reading {field.Name}: {e.Message}");
                }
            }

            Debug.Log("----- ✅ PlayerData Dump Complete -----");
        }
        private void bench()
        {
            if (PlayerData.instance == null)
                return;
            PlayerData pd = PlayerData.instance;
            if(!gotalltools)
                StartCoroutine(UnlockAllCoroutine());

            pd.ShellShards = 800;
            TeleportToBossSmart(new BossData("", "Bone_04", new Vector3(11.27499f, 18.56768f, 0.004f), "", ""));
        }
        private void unlockcrest(int number)
        {
            string[] crests = {"Cloakless","Cursed","Witch","Warrior","Toolmaster","Spell","Reaper","Wanderer","Hunter_v2","Hunter_v3"};
            if (!Resources.FindObjectsOfTypeAll<ToolCrest>()[number].IsUnlocked)
                Resources.FindObjectsOfTypeAll<ToolCrest>()[number].Unlock();
            else
                log.LogInfo("crest already unlocked");
        }
        private void givememory()
        {
            if (PlayerData.instance == null)
                return;

            foreach(var crest in Resources.FindObjectsOfTypeAll<ToolCrest>())
            {
                ToolItemManager manager = ManagerSingleton<ToolItemManager>.Instance;
                ToolCrestsData.Data data = PlayerData.instance.ToolEquips.GetData(crest.name); 
                int count = 0;
                foreach (var slot in data.Slots)
                {
                    log.LogInfo($"slot {count}: Unlocked = {data.Slots[count].IsUnlocked}");
                    count++;
                }
                for(int i = 0; i < count; i++)
                {
                    var slot = data.Slots[i];
                    slot.IsUnlocked = true;
                    data.Slots[i] = slot;
                }

            }
        }
    }
}
