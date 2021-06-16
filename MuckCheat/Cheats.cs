using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity;
using Random = UnityEngine.Random;

namespace MuckCheat
{
    class Cheats : MonoBehaviour
    {
        bool GodMode = false;
        bool NoHunger = false;
        float SpeedMult = 1;
        bool InfiniteJumps = false;
        float multiplier = 1.0f;
        float bossmultiplier = 1.0f;

        Menu miscitems = new Menu(10, 10, "Misc");
        Menu itemeditor;
        Menu mobspawn;
        Menu playerstatusEditor;

        void Start()
        {
            miscitems.m_controls.bottom = 20;
            miscitems.StartX = 5;
            miscitems.StartY = 15;
        }

        void DropRandomPowerUp(Vector3 pos)
        {
            Powerup randomPowerup = ItemManager.Instance.GetRandomPowerup(0.30f, 0.30f, 0.30f);
            int nextId = ItemManager.Instance.GetNextId();
            ItemManager.Instance.DropPowerupAtPosition(randomPowerup.id, pos, nextId);
            ServerSend.DropPowerupAtPosition(randomPowerup.id, nextId, pos);
        }

        void SpawnRandomMob()
        {
            MobManager mobmanager = MobManager.Instance;

            int RandomMobId = Random.Range(0, MobSpawner.Instance.allMobs.Length);
            MobType randmob = MobSpawner.Instance.allMobs[RandomMobId];

            int nextId = MobManager.Instance.GetNextId();
            MobSpawner.Instance.ServerSpawnNewMob(nextId, randmob.id, PlayerStatus.Instance.gameObject.transform.position, multiplier, bossmultiplier);

        }
        int next_x_menu = 0;
        void Misc()
        {
            miscitems.Start();
            {
                GodMode = miscitems.Toggle("GodMode", GodMode);
                NoHunger = miscitems.Toggle("NoHunger", NoHunger);
                InfiniteJumps = miscitems.Toggle("InfiniteJumps", InfiniteJumps);


                if (GameManager.instance != null)
                {
                    if (miscitems.NumericUpDown3(String.Format("Day: {0}", GameLoop.Instance.currentDay), ref GameLoop.Instance.currentDay, 150, 30))
                    {
                        GameManager.instance.UpdateDay(GameLoop.Instance.currentDay);
                    }
                }

                SpeedMult = miscitems.Slider_Float("SpeedMult", SpeedMult, 100, 30, 0, 20);
                if (miscitems.Button("DropRandomPowerup", 160, 30))
                    DropRandomPowerUp(PlayerStatus.Instance.transform.position);

                if (miscitems.Button("DropRandomPowerup x10", 160, 30))
                {
                    for (int i = 0; i < 10; i++)
                    {
                        DropRandomPowerUp(PlayerStatus.Instance.transform.position);
                    }
                }

                if (miscitems.Button("DropRandomPowerup x100", 160, 30))
                {
                    for (int i = 0; i < 100; i++)
                    {
                        DropRandomPowerUp(PlayerStatus.Instance.transform.position);
                    }
                }



                //if(miscitems.Button("Eject", 140, 30))
                //{
                //Loader.Eject();
                //}
            }

            next_x_menu += miscitems.Width + 20;
        }

        void ItemEditor()
        {
            InventoryItem curritem = InventoryUI.Instance.hotbar.currentItem;
            if (curritem != null)
            {
                if (itemeditor == null)
                {
                    itemeditor = new Menu(20 + next_x_menu, 10, "Item Editor");

                    itemeditor.m_controls.bottom = 20;
                    itemeditor.StartX = 5;
                    itemeditor.StartY = 15;


                }

                itemeditor.x = next_x_menu;

                itemeditor.Start();
                {
                    itemeditor.Label("name: " + curritem.name);

                    curritem.amount = itemeditor.NumericUpDown2(String.Format("Ammount: {0}", curritem.amount), curritem.amount, 130, 30);
                    curritem.max = itemeditor.NumericUpDown2(String.Format("MaxAmmount: {0}", curritem.max), curritem.max, 130, 30);
                    curritem.attackDamage = itemeditor.NumericUpDown2(String.Format("AttackDamage: {0}", curritem.attackDamage), curritem.attackDamage, 130, 30);
                    curritem.resourceDamage = itemeditor.NumericUpDown2(String.Format("ResourceDamage: {0}", curritem.resourceDamage), (int)curritem.resourceDamage, 130, 30);
                    curritem.attackSpeed = itemeditor.NumericUpDown2(String.Format("AttackSpeed: {0}", curritem.attackSpeed), (int)curritem.attackSpeed, 130, 30);
                    curritem.tier = itemeditor.NumericUpDown2(String.Format("Tier: {0}", curritem.tier), (int)curritem.tier, 130, 30);
                    curritem.scale = itemeditor.NumericUpDown2(String.Format("Scale: {0}", curritem.scale), (int)curritem.scale, 130, 30);
                }
                next_x_menu += itemeditor.Width + 20;

            }

        }

        void Mob_Spawner()
        {
            if (mobspawn == null)
            {
                mobspawn = new Menu(20 + next_x_menu, 10, "MobSpawner");
                mobspawn.m_controls.bottom = 20;
                mobspawn.StartX = 5;
                mobspawn.StartY = 15;
            }

            mobspawn.x = next_x_menu;


            mobspawn.Start();
            {
                bossmultiplier = mobspawn.Slider_Float("BossMultiplier", bossmultiplier, 150, 30, 1, 50);
                multiplier = mobspawn.Slider_Float("Multiplier", multiplier, 150, 30, 1, 50);

                if (mobspawn.Button("Spawn Random", 160, 30))
                {
                    SpawnRandomMob();
                }

                if (mobspawn.Button("Spawn Random x10", 160, 30))
                {
                    for (int i = 0; i < 10; i++)
                    {
                        SpawnRandomMob();
                    }

                }

                if (mobspawn.Button("Spawn Random x100", 160, 30))
                {
                    for (int i = 0; i < 100; i++)
                    {
                        SpawnRandomMob();
                    }

                }
            }
            next_x_menu += mobspawn.Width + 20;
        }

        void Update()
        {
            if (NoHunger)
            {
                PlayerStatus.Instance.hunger = PlayerStatus.Instance.maxHunger;
            }
            if (GodMode)
            {
                PlayerStatus.Instance.hp = PlayerStatus.Instance.maxHp;
                PlayerStatus.Instance.stamina = PlayerStatus.Instance.maxStamina;

                ClientSend.PlayerHp(PlayerStatus.Instance.maxHp, PlayerStatus.Instance.maxHp);

                Server.clients[LocalClient.instance.myId].player.dead = false;

            }
            if (SpeedMult > 1)
            {
                FieldInfo movespeed = typeof(PlayerMovement).GetField("moveSpeed", BindingFlags.Instance | BindingFlags.NonPublic);
                FieldInfo maxspeed = typeof(PlayerMovement).GetField("maxSpeed", BindingFlags.Instance | BindingFlags.NonPublic);
                FieldInfo maxRunSpeed = typeof(PlayerMovement).GetField("maxRunSpeed", BindingFlags.Instance | BindingFlags.NonPublic);
                FieldInfo maxWalkSpeed = typeof(PlayerMovement).GetField("maxWalkSpeed", BindingFlags.Instance | BindingFlags.NonPublic);

                movespeed.SetValue(PlayerMovement.Instance, 3500 * SpeedMult);
                maxspeed.SetValue(PlayerMovement.Instance, 6.5f * SpeedMult);
                maxRunSpeed.SetValue(PlayerMovement.Instance, 13.0f * SpeedMult);
                maxWalkSpeed.SetValue(PlayerMovement.Instance, 6.5f * SpeedMult);
            }
            if (InfiniteJumps)
            {
                FieldInfo jumps = typeof(PlayerMovement).GetField("jumps", BindingFlags.Instance | BindingFlags.NonPublic);

                jumps.SetValue(PlayerMovement.Instance, 999);
            }

            if(Input.GetKeyDown(KeyCode.Insert))
            {
                MenuOpen = !MenuOpen;
            }

        }
        bool MenuOpen = true;
        void OnGUI()
        {
            next_x_menu = 0;

            if (MenuOpen)
            {
                Misc();

                ItemEditor();

                Mob_Spawner();

            }

        }



    }
}
