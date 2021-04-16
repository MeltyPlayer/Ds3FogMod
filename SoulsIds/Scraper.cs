// Decompiled with JetBrains decompiler
// Type: SoulsIds.Scraper
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using SoulsFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SoulsIds
{
  public class Scraper
  {
    public static readonly Dictionary<string, Universe.Namespace> MsgTypes = new Dictionary<string, Universe.Namespace>()
    {
      {
        "NPC名",
        Universe.Namespace.NPC
      },
      {
        "武器名",
        Universe.Namespace.WEAPON
      },
      {
        "アイテム名",
        Universe.Namespace.GOODS
      },
      {
        "イベントテキスト",
        Universe.Namespace.ACTION
      },
      {
        "会話",
        Universe.Namespace.DIALOGUE
      },
      {
        "会話_dlc1",
        Universe.Namespace.DIALOGUE
      },
      {
        "会話_dlc2",
        Universe.Namespace.DIALOGUE
      },
      {
        "防具名",
        Universe.Namespace.PROTECTOR
      },
      {
        "アクセサリ名",
        Universe.Namespace.ACCESSORY
      },
      {
        "Weapon_name_",
        Universe.Namespace.WEAPON
      },
      {
        "Armor_name_",
        Universe.Namespace.PROTECTOR
      },
      {
        "Accessory_name_",
        Universe.Namespace.ACCESSORY
      },
      {
        "Item_name_",
        Universe.Namespace.GOODS
      },
      {
        "Event_text_",
        Universe.Namespace.ACTION
      },
      {
        "Conversation_",
        Universe.Namespace.DIALOGUE
      },
      {
        "mapevent",
        Universe.Namespace.ACTION
      },
      {
        "npcmenu",
        Universe.Namespace.NPC
      },
      {
        "itemname",
        Universe.Namespace.GOODS
      },
      {
        "bonfirename",
        Universe.Namespace.BONFIRE
      }
    };
    public static readonly Dictionary<Universe.Namespace, string> ItemParams = new Dictionary<Universe.Namespace, string>()
    {
      {
        Universe.Namespace.WEAPON,
        "EquipParamWeapon"
      },
      {
        Universe.Namespace.PROTECTOR,
        "EquipParamProtector"
      },
      {
        Universe.Namespace.ACCESSORY,
        "EquipParamAccessory"
      },
      {
        Universe.Namespace.GOODS,
        "EquipParamGoods"
      }
    };
    private static readonly Dictionary<GameSpec.FromGame, string> talkParamMsgId = new Dictionary<GameSpec.FromGame, string>()
    {
      [GameSpec.FromGame.DS1R] = "msgId",
      [GameSpec.FromGame.DS3] = "PcGenderFemale1",
      [GameSpec.FromGame.SDT] = "TalkParamId1"
    };
    private GameSpec spec;
    private GameEditor editor;
    private Dictionary<string, PARAM> Params;

    public Scraper(GameSpec spec)
    {
      this.spec = spec;
      this.editor = new GameEditor(spec);
    }

    private void LoadParams()
    {
      if (this.Params != null)
        return;
      this.Params = new GameEditor(this.spec).LoadParams((Dictionary<string, PARAM.Layout>) null, false);
    }

    public bool ScrapeMsgs(Universe u)
    {
      if (this.spec.MsgDir == null)
        return false;
      foreach (KeyValuePair<string, FMG> keyValuePair in (IEnumerable<KeyValuePair<string, FMG>>) this.editor.LoadBnds<FMG>(this.spec.MsgDir, (Func<byte[], string, FMG>) ((data, name) => SoulsFile<FMG>.Read(data)), "*bnd.dcx", (string) null).SelectMany<KeyValuePair<string, Dictionary<string, FMG>>, KeyValuePair<string, FMG>>((Func<KeyValuePair<string, Dictionary<string, FMG>>, IEnumerable<KeyValuePair<string, FMG>>>) (e => (IEnumerable<KeyValuePair<string, FMG>>) e.Value)).Concat<KeyValuePair<string, FMG>>((IEnumerable<KeyValuePair<string, FMG>>) this.editor.Load<FMG>(this.spec.MsgDir, (Func<string, FMG>) (name => SoulsFile<FMG>.Read(name)), "*.fmg")).OrderBy<KeyValuePair<string, FMG>, string>((Func<KeyValuePair<string, FMG>, string>) (e => e.Key)))
      {
        if (Scraper.MsgTypes.ContainsKey(keyValuePair.Key))
        {
          Universe.Namespace msgType = Scraper.MsgTypes[keyValuePair.Key];
          foreach (FMG.Entry entry in keyValuePair.Value.Entries)
            u.Names[Universe.Obj.Of(msgType, (object) entry.ID)] = entry.Text;
        }
      }
      if (Directory.Exists(this.spec.GameDir + "\\" + this.spec.MsgDir + "\\talk"))
      {
        foreach (KeyValuePair<string, FMG> keyValuePair in this.editor.Load<FMG>(this.spec.MsgDir + "\\talk", (Func<string, FMG>) (name => SoulsFile<FMG>.Read(name)), "*.fmg"))
        {
          foreach (FMG.Entry entry in keyValuePair.Value.Entries)
            u.Names[Universe.Obj.Talk(entry.ID)] = entry.Text;
        }
      }
      if (this.spec.ParamFile == null || !Scraper.talkParamMsgId.ContainsKey(this.spec.Game))
        return false;
      this.LoadParams();
      if (!this.Params.ContainsKey("TalkParam"))
        return false;
      string index = Scraper.talkParamMsgId[this.spec.Game];
      foreach (PARAM.Row row in this.Params["TalkParam"].Rows)
      {
        int num = (int) row[index].Value;
        if (num > 0)
        {
          Universe.Obj key = Universe.Obj.Of(Universe.Namespace.DIALOGUE, (object) num);
          if (u.Names.ContainsKey(key))
            u.Names[Universe.Obj.Talk((int) row.ID)] = u.Names[key];
        }
      }
      return true;
    }

    public bool ScrapeItems(Universe u)
    {
      if (this.spec.ParamFile == null)
        return false;
      this.LoadParams();
      if (this.spec.Game == GameSpec.FromGame.DS2S)
      {
        if (!this.Params.ContainsKey("ItemLotParam2_Other"))
          return false;
        foreach (PARAM.Row row in this.Params["ItemLotParam2_Other"].Rows.Concat<PARAM.Row>((IEnumerable<PARAM.Row>) this.Params["ItemLotParam2_Chr"].Rows))
        {
          Universe.Obj index1 = Universe.Obj.Lot((int) row.ID);
          Universe.Obj index2 = Universe.Obj.Item(3U, (int) (uint) row["Unk2C"].Value);
          u.Add(Universe.Verb.PRODUCES, index1, index2);
          if (u.Names.ContainsKey(index2) && !u.Names.ContainsKey(index1))
            u.Names[index1] = u.Names[index2];
        }
      }
      if (!this.Params.ContainsKey("ItemLotParam"))
        return false;
      if (this.spec.Game == GameSpec.FromGame.DS1R)
      {
        foreach (PARAM.Row row in this.Params["ItemLotParam"].Rows)
        {
          Universe.Obj from = Universe.Obj.Lot((int) row.ID);
          int id1 = (int) row["getItemFlagId"].Value;
          if (id1 != -1)
            u.Add(Universe.Verb.WRITES, from, Universe.Obj.EventFlag(id1, -1));
          for (int index = 1; index <= 8; ++index)
          {
            int id2 = (int) row[string.Format("lotItemId0{0}", (object) index)].Value;
            int num = (int) row[string.Format("lotItemCategory0{0}", (object) index)].Value;
            if (id2 != 0 && num != -1)
              u.Add(Universe.Verb.PRODUCES, from, Universe.Obj.Item((uint) num, id2));
          }
        }
        foreach (PARAM.Row row in this.Params["ShopLineupParam"].Rows)
        {
          if (row.ID < 9000000L)
          {
            Universe.Obj from = Universe.Obj.Shop((int) row.ID, -1);
            int id1 = (int) row["eventFlag"].Value;
            if (id1 != -1)
              u.Add(Universe.Verb.WRITES, from, Universe.Obj.EventFlag(id1, -1));
            int id2 = (int) row["qwcId"].Value;
            if (id2 != -1)
              u.Add(Universe.Verb.READS, from, Universe.Obj.EventFlag(id2, -1));
            int num = (int) (byte) row["equipType"].Value;
            int id3 = (int) row["equipId"].Value;
            u.Add(Universe.Verb.PRODUCES, from, Universe.Obj.Item((uint) num, id3));
            int id4 = (int) row["mtrlId"].Value;
            if (id4 != -1)
              u.Add(Universe.Verb.CONSUMES, from, Universe.Obj.Material(id4));
          }
        }
        foreach (PARAM.Row row in this.Params["EquipMtrlSetParam"].Rows)
        {
          Universe.Obj from = Universe.Obj.Material((int) row.ID);
          for (int index = 1; index <= 5; ++index)
          {
            int id = (int) row[string.Format("materialId0{0}", (object) index)].Value;
            if (id > 0)
              u.Add(Universe.Verb.CONSUMES, from, Universe.Obj.Item(3U, id));
          }
        }
      }
      else
      {
        if (this.spec.Game != GameSpec.FromGame.DS3)
          return false;
        foreach (PARAM.Row row in this.Params["ItemLotParam"].Rows)
        {
          Universe.Obj index1 = Universe.Obj.Lot((int) row.ID);
          int id1 = (int) row["getItemFlagId"].Value;
          if (id1 != -1)
            u.Add(Universe.Verb.WRITES, index1, Universe.Obj.EventFlag(id1, -1));
          for (int index2 = 1; index2 <= 8; ++index2)
          {
            int id2 = (int) row[string.Format("ItemLotId{0}", (object) index2)].Value;
            uint type = (uint) row[string.Format("LotItemCategory0{0}", (object) index2)].Value;
            if (id2 != 0 && type != uint.MaxValue)
            {
              Universe.Obj index3 = Universe.Obj.Item(type, id2);
              u.Add(Universe.Verb.PRODUCES, index1, index3);
              if (u.Names.ContainsKey(index3) && !u.Names.ContainsKey(index1))
                u.Names[index1] = u.Names[index3];
            }
          }
        }
        foreach (PARAM.Row row in this.Params["ShopLineupParam"].Rows)
        {
          if (row.ID < 9000000L)
          {
            Universe.Obj from = Universe.Obj.Shop((int) row.ID, -1);
            int id1 = (int) row["EventFlag"].Value;
            if (id1 != -1)
              u.Add(Universe.Verb.WRITES, from, Universe.Obj.EventFlag(id1, -1));
            int id2 = (int) row["qwcID"].Value;
            if (id2 != -1)
              u.Add(Universe.Verb.READS, from, Universe.Obj.EventFlag(id2, -1));
            Universe.Obj index = Universe.Obj.Item((uint) (byte) row["equipType"].Value, (int) row["EquipId"].Value);
            u.Add(Universe.Verb.PRODUCES, from, index);
            if (u.Names.ContainsKey(index))
              u.Names[from] = u.Names[index];
            int id3 = (int) row["mtrlId"].Value;
            if (id3 != -1)
              u.Add(Universe.Verb.CONSUMES, from, Universe.Obj.Material(id3));
          }
        }
        foreach (PARAM.Row row in this.Params["EquipMtrlSetParam"].Rows)
        {
          Universe.Obj from = Universe.Obj.Material((int) row.ID);
          for (int index = 1; index <= 5; ++index)
          {
            int id = (int) row[string.Format("MaterialId0{0}", (object) index)].Value;
            if (id > 0)
              u.Add(Universe.Verb.CONSUMES, from, Universe.Obj.Item(3U, id));
          }
        }
        foreach (PARAM.Row row in this.Params["NpcParam"].Rows)
        {
          int id = (int) row["ItemLotId1"].Value;
          if (id != -1)
            u.Add(Universe.Verb.PRODUCES, Universe.Obj.Npc((int) row.ID), Universe.Obj.Lot(id));
        }
      }
      return true;
    }

    public void LoadNames(Universe u)
    {
      if (this.spec.NameDir == null)
        return;
      foreach (KeyValuePair<string, string> loadName in this.editor.LoadNames<string>("ModelName", (Func<string, string>) (n => n), true))
      {
        u.Names[Universe.Obj.Of(Universe.Namespace.OBJ_MODEL, (object) loadName.Key)] = loadName.Value;
        u.Names[Universe.Obj.Of(Universe.Namespace.CHR_MODEL, (object) loadName.Key)] = loadName.Value;
      }
      foreach (KeyValuePair<int, string> loadName in this.editor.LoadNames<int>("CharaInitParam", (Func<string, int>) (n => int.Parse(n)), true))
        u.Names[Universe.Obj.Of(Universe.Namespace.NPC, (object) loadName.Key)] = loadName.Value;
      foreach (KeyValuePair<int, string> loadName in this.editor.LoadNames<int>("ShopQwc", (Func<string, int>) (n => int.Parse(n)), true))
        u.Names[Universe.Obj.Of(Universe.Namespace.EVENT_FLAG, (object) loadName.Key)] = loadName.Value;
    }

    public bool ScrapeMaps(Universe u)
    {
      if (this.spec.MsbDir == null)
        return false;
      if (this.spec.Game == GameSpec.FromGame.SDT)
      {
        foreach (KeyValuePair<string, MSBS> keyValuePair in this.editor.Load<MSBS>(this.spec.MsbDir, (Func<string, MSBS>) (path => SoulsFile<MSBS>.Read(path)), "*.dcx"))
        {
          string key = keyValuePair.Key;
          MSBS msbs = keyValuePair.Value;
          Universe.Obj from1 = Universe.Obj.Map(key);
          int index1 = 0;
          foreach (MSBS.Event.Treasure treasure in msbs.Events.Treasures)
          {
            if (treasure.TreasurePartName != null && treasure.ItemLotID != -1)
            {
              Universe.Obj obj = Universe.Obj.Treasure(key, index1);
              Universe.Obj from2 = Universe.Obj.Part(key, treasure.TreasurePartName);
              u.Add(Universe.Verb.PRODUCES, from2, obj);
              u.Add(Universe.Verb.PRODUCES, obj, Universe.Obj.Lot(treasure.ItemLotID));
            }
            ++index1;
          }
          foreach (MSBS.Event.Talk talk in msbs.Events.Talks)
          {
            for (int index2 = 0; index2 < 2; ++index2)
            {
              string enemyName = talk.EnemyNames[index2];
              int talkId = talk.TalkIDs[index2];
              if (enemyName != null && talkId >= 0)
              {
                Universe.Obj from2 = Universe.Obj.Part(key, enemyName);
                u.Add(Universe.Verb.CONTAINS, from2, Universe.Obj.Esd(talkId));
              }
            }
          }
          foreach (MSBS.Part entry in msbs.Parts.GetEntries())
          {
            if (entry is MSBS.Part part)
            {
              Universe.Obj obj = Universe.Obj.Part(key, part.Name);
              if (part.EntityID != -1)
                u.Add(Universe.Verb.CONTAINS, Universe.Obj.Entity(part.EntityID), obj);
              foreach (int id in ((IEnumerable<int>) part.EntityGroupIDs).Where<int>((Func<int, bool>) (groupID => groupID > 0)))
                u.Add(Universe.Verb.CONTAINS, Universe.Obj.Entity(id), obj);
              if (part is MSBS.Part.Enemy enemy)
              {
                string id = part.Name.Split('_')[0];
                u.Add(Universe.Verb.CONTAINS, obj, Universe.Obj.ChrModel(id));
                if (enemy.NPCParamID != -1)
                  u.Add(Universe.Verb.CONTAINS, obj, Universe.Obj.Npc(enemy.NPCParamID));
              }
              else if (part is MSBS.Part.Object)
              {
                string id = part.Name.Split('_')[0];
                u.Add(Universe.Verb.CONTAINS, obj, Universe.Obj.ObjModel(id));
              }
              if (u.Nodes.ContainsKey(obj))
                u.Add(Universe.Verb.CONTAINS, from1, obj);
            }
          }
        }
      }
      else if (this.spec.Game == GameSpec.FromGame.DS1 || this.spec.Game == GameSpec.FromGame.DS1R)
      {
        foreach (KeyValuePair<string, MSB1> keyValuePair in this.editor.Load<MSB1>(this.spec.MsbDir, (Func<string, MSB1>) (path => !path.Contains("m99") ? SoulsFile<MSB1>.Read(path) : (MSB1) null), "*.msb"))
        {
          string key = keyValuePair.Key;
          Universe.Obj from = Universe.Obj.Map(key);
          MSB1 msB1 = keyValuePair.Value;
          if (msB1 != null)
          {
            foreach (MSB1.Part entry in msB1.Parts.GetEntries())
            {
              if (entry is MSB1.Part part)
              {
                Universe.Obj obj = Universe.Obj.Part(key, part.Name);
                if (part.EntityID != -1)
                  u.Add(Universe.Verb.CONTAINS, Universe.Obj.Entity(part.EntityID), obj);
                if (part is MSB1.Part.Enemy enemy)
                {
                  string id = part.Name.Split('_')[0];
                  u.Add(Universe.Verb.CONTAINS, obj, Universe.Obj.ChrModel(id));
                  if (enemy.NPCParamID != -1 && id == "c0000")
                    u.Add(Universe.Verb.CONTAINS, obj, Universe.Obj.Human(enemy.NPCParamID));
                  if (enemy.TalkID != -1)
                    u.Add(Universe.Verb.CONTAINS, obj, Universe.Obj.Esd(enemy.TalkID));
                }
                else if (part is MSB1.Part.Object)
                {
                  string id = part.Name.Split('_')[0];
                  u.Add(Universe.Verb.CONTAINS, obj, Universe.Obj.ObjModel(id));
                }
                if (u.Nodes.ContainsKey(obj))
                  u.Add(Universe.Verb.CONTAINS, from, obj);
              }
            }
          }
        }
      }
      else
      {
        if (this.spec.Game != GameSpec.FromGame.DS3)
          return false;
        foreach (KeyValuePair<string, MSB3> keyValuePair in this.editor.Load<MSB3>(this.spec.MsbDir, (Func<string, MSB3>) (path => !path.Contains("m99") ? SoulsFile<MSB3>.Read(path) : (MSB3) null), "*.dcx"))
        {
          string key = keyValuePair.Key;
          Universe.Obj from = Universe.Obj.Map(key);
          MSB3 msB3 = keyValuePair.Value;
          if (msB3 != null)
          {
            foreach (MSB3.Part entry in msB3.Parts.GetEntries())
            {
              if (entry is MSB3.Part part)
              {
                Universe.Obj obj = Universe.Obj.Part(key, part.Name);
                if (part.EventEntityID != -1)
                  u.Add(Universe.Verb.CONTAINS, Universe.Obj.Entity(part.EventEntityID), obj);
                if (part is MSB3.Part.Enemy enemy)
                {
                  string id = part.Name.Split('_')[0];
                  u.Add(Universe.Verb.CONTAINS, obj, Universe.Obj.ChrModel(id));
                  if (enemy.CharaInitID > 0)
                    u.Add(Universe.Verb.CONTAINS, obj, Universe.Obj.Human(enemy.CharaInitID));
                  if (enemy.TalkID != -1)
                    u.Add(Universe.Verb.CONTAINS, obj, Universe.Obj.Esd(enemy.TalkID));
                }
                else if (part is MSB3.Part.Object)
                {
                  string id = part.Name.Split('_')[0];
                  u.Add(Universe.Verb.CONTAINS, obj, Universe.Obj.ObjModel(id));
                }
                if (u.Nodes.ContainsKey(obj))
                  u.Add(Universe.Verb.CONTAINS, from, obj);
              }
            }
          }
        }
      }
      return true;
    }
  }
}
