// Decompiled with JetBrains decompiler
// Type: SoulsIds.Universe
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.Linq;

namespace SoulsIds {
  public class Universe {
    public static Dictionary<uint, int> LotTypes = new Dictionary<uint, int>() {
        {
            0U,
            0
        }, {
            268435456U,
            1
        }, {
            536870912U,
            2
        }, {
            1073741824U,
            3
        }
    };

    private static HashSet<Universe.Namespace> Quotes =
        new HashSet<Universe.Namespace>() {
            Universe.Namespace.ACTION,
            Universe.Namespace.DIALOGUE,
            Universe.Namespace.TALK
        };

    public static readonly List<string> ActiveVerb = new List<string>() {
        "reads",
        "writes",
        "produces",
        "consumes",
        "contains",
        "has text"
    };

    public static readonly List<string> PassiveVerb = new List<string>() {
        "read by",
        "written by",
        "produced by",
        "consumed by",
        "contained in",
        "used in"
    };

    public readonly Dictionary<Universe.Obj, Universe.Node> Nodes =
        new Dictionary<Universe.Obj, Universe.Node>();

    public readonly Dictionary<Universe.Obj, string> Names =
        new Dictionary<Universe.Obj, string>();

    public string Name(Universe.Obj obj) {
      string str;
      return this.Names.TryGetValue(obj, out str)
                 ? string.Format("{0}:{1}",
                                 (object) obj,
                                 Universe.Quotes.Contains(obj.Type)
                                     ? (object) ("\"" + str + "\"")
                                     : (object) str)
                 : string.Format("{0}", (object) obj);
    }

    public List<Universe.Obj> Next(
        Universe.Obj obj,
        Universe.Verb v,
        Universe.Namespace type = Universe.Namespace.GLOBAL) {
      return !this.Nodes.ContainsKey(obj)
                 ? new List<Universe.Obj>()
                 : this.Nodes[obj]
                       .To.Where<Universe.Relation>(
                           (Func<Universe.Relation, bool>) (r => r.Verb == v &&
                                                                 r.To.HasType(
                                                                     type)))
                       .Select<Universe.Relation, Universe.Obj>(
                           (Func<Universe.Relation, Universe.Obj>) (r => r.To))
                       .ToList<Universe.Obj>();
    }

    public List<Universe.Obj> Prev(
        Universe.Obj obj,
        Universe.Verb v,
        Universe.Namespace type = Universe.Namespace.GLOBAL) {
      return !this.Nodes.ContainsKey(obj)
                 ? new List<Universe.Obj>()
                 : this.Nodes[obj]
                       .From.Where<Universe.Relation>(
                           (Func<Universe.Relation, bool>) (r => r.Verb == v &&
                                                                 r.From
                                                                  .HasType(
                                                                      type)))
                       .Select<Universe.Relation, Universe.Obj>(
                           (Func<Universe.Relation, Universe.Obj>)
                           (r => r.From))
                       .ToList<Universe.Obj>();
    }

    public Universe.Node Add(Universe.Obj obj) {
      if (!this.Nodes.ContainsKey(obj))
        this.Nodes[obj] = new Universe.Node() {
            Obj = obj,
            From = new List<Universe.Relation>(),
            To = new List<Universe.Relation>()
        };
      return this.Nodes[obj];
    }

    public void Add(Universe.Verb verb, Universe.Obj from, Universe.Obj to) {
      Universe.Node node1 = this.Add(from);
      Universe.Node node2 = this.Add(to);
      Universe.Relation relation = new Universe.Relation() {
          From = from,
          To = to,
          Verb = verb
      };
      node1.To.Add(relation);
      node2.From.Add(relation);
    }

    public List<Universe.Obj> Connected(
        Universe.Obj obj,
        Universe.RelationSpec rel) {
      return !rel.Subject
                 ? this.Prev(obj, rel.Verb, rel.Type)
                 : this.Next(obj, rel.Verb, rel.Type);
    }

    private static int Nest(int first, int second) {
      return first == 0 ? second : first;
    }

    public enum Namespace {
      GLOBAL,
      ITEM,
      EVENT,
      EVENT_FLAG,
      ESD,
      LOT,
      SHOP,
      NPC,
      MATERIAL,
      SKILL,
      WEAPON,
      PROTECTOR,
      ACCESSORY,
      GOODS,
      TALK,
      DIALOGUE,
      ENTITY,
      PART,
      OBJ_MODEL,
      CHR_MODEL,
      TREASURE,
      MAP,
      BONFIRE,
      HUMAN,
      ACTION,
      CUTSCENE,
      ACHIEVEMENT,
      ANIMATION,
      SP_EFFECT,
      SFX,
    }

    public class Obj : IComparable<Universe.Obj> {
      public string ID { get; set; }

      public Universe.Namespace Type { get; set; }

      public int RangeEnd { get; set; }

      public override string ToString() {
        return this.Type.ToString().ToLower() +
               ":" +
               this.ID +
               (this.RangeEnd == -1
                    ? ""
                    : string.Format(":{0}", (object) this.RangeEnd));
      }

      public override bool Equals(object obj) {
        return obj is Universe.Obj o && this.Equals(o);
      }

      public bool Equals(Universe.Obj o) {
        return this.Type == o.Type &&
               this.ID.Equals(o.ID) &&
               this.RangeEnd == o.RangeEnd;
      }

      public override int GetHashCode() {
        return (int) this.Type << 24 ^ this.ID.GetHashCode() ^ this.RangeEnd;
      }

      public int CompareTo(Universe.Obj o) {
        int num = this.Type.CompareTo((object) o.Type);
        if (num != 0)
          return num;
        int result1;
        int result2;
        return int.TryParse(this.ID, out result1) &&
               int.TryParse(o.ID, out result2)
                   ? Universe.Nest(result1.CompareTo(result2),
                                   this.RangeEnd.CompareTo(o.RangeEnd))
                   : Universe.Nest(this.ID.CompareTo(o.ID),
                                   this.RangeEnd.CompareTo(o.RangeEnd));
      }

      public bool HasType(Universe.Namespace n) {
        switch (n) {
          case Universe.Namespace.GLOBAL:
            return true;
          case Universe.Namespace.ITEM:
            return this.Type >= Universe.Namespace.WEAPON &&
                   this.Type <= Universe.Namespace.GOODS;
          default:
            return n == this.Type;
        }
      }

      private Obj(object ID, Universe.Namespace Type, int RangeEnd = -1) {
        this.ID = ID.ToString();
        this.Type = Type;
        this.RangeEnd = RangeEnd;
      }

      public static Universe.Obj Lot(int id) {
        return new Universe.Obj((object) id, Universe.Namespace.LOT, -1);
      }

      public static Universe.Obj Shop(int id, int end = -1) {
        return new Universe.Obj((object) id, Universe.Namespace.SHOP, end);
      }

      public static Universe.Obj EventFlag(int id, int end = -1) {
        return new Universe.Obj((object) id,
                                Universe.Namespace.EVENT_FLAG,
                                end);
      }

      public static Universe.Obj Talk(int id) {
        return new Universe.Obj((object) id, Universe.Namespace.TALK, -1);
      }

      public static Universe.Obj Action(int id) {
        return new Universe.Obj((object) id, Universe.Namespace.ACTION, -1);
      }

      public static Universe.Obj Esd(int id) {
        return new Universe.Obj((object) id, Universe.Namespace.ESD, -1);
      }

      public static Universe.Obj Npc(int id) {
        return new Universe.Obj((object) id, Universe.Namespace.NPC, -1);
      }

      public static Universe.Obj Material(int id) {
        return new Universe.Obj((object) id, Universe.Namespace.MATERIAL, -1);
      }

      public static Universe.Obj Skill(int id) {
        return new Universe.Obj((object) id, Universe.Namespace.SKILL, -1);
      }

      public static Universe.Obj Map(string id) {
        return new Universe.Obj((object) id, Universe.Namespace.MAP, -1);
      }

      public static Universe.Obj Entity(int id) {
        return new Universe.Obj((object) id, Universe.Namespace.ENTITY, -1);
      }

      public static Universe.Obj Part(string map, string id) {
        return new Universe.Obj((object) (map + "_" + id),
                                Universe.Namespace.PART,
                                -1);
      }

      public static Universe.Obj ObjModel(string id) {
        return new Universe.Obj((object) id, Universe.Namespace.OBJ_MODEL, -1);
      }

      public static Universe.Obj ChrModel(string id) {
        return new Universe.Obj((object) id, Universe.Namespace.CHR_MODEL, -1);
      }

      public static Universe.Obj Treasure(string map, int index) {
        return new Universe.Obj(
            (object) string.Format("{0}_{1}", (object) map, (object) index),
            Universe.Namespace.TREASURE,
            -1);
      }

      public static Universe.Obj Dialogue(int id) {
        return new Universe.Obj((object) id, Universe.Namespace.DIALOGUE, -1);
      }

      public static Universe.Obj Bonfire(int id) {
        return new Universe.Obj((object) id, Universe.Namespace.BONFIRE, -1);
      }

      public static Universe.Obj Human(int id) {
        return new Universe.Obj((object) id, Universe.Namespace.HUMAN, -1);
      }

      public static Universe.Obj Of(Universe.Namespace type, object id) {
        return new Universe.Obj(id, type, -1);
      }

      public static Universe.Obj Item(uint type, int id) {
        Universe.Namespace Type = type <= 4U
                                      ? (Universe.Namespace) (10 + (int) type)
                                      : (Universe.Namespace) (10 +
                                                              Universe.LotTypes[
                                                                  type]);
        return new Universe.Obj((object) id.ToString(), Type, -1);
      }
    }

    public enum Verb {
      READS,
      WRITES,
      PRODUCES,
      CONSUMES,
      CONTAINS,
      HAS_TEXT,
    }

    public class Relation {
      public Universe.Obj From { get; set; }

      public Universe.Obj To { get; set; }

      public Universe.Verb Verb { get; set; }
    }

    public class Node {
      public Universe.Obj Obj { get; set; }

      public List<Universe.Relation> From { get; set; }

      public List<Universe.Relation> To { get; set; }
    }

    public class PartialRelation {
      public Universe.Obj Obj { get; set; }

      public Universe.Verb Verb { get; set; }

      public bool Subject { get; set; }

      public static Universe.PartialRelation Verbs(
          Universe.Verb verb,
          Universe.Obj obj) {
        return new Universe.PartialRelation() {
            Obj = obj,
            Verb = verb,
            Subject = false
        };
      }

      public static Universe.PartialRelation VerbedBy(
          Universe.Obj obj,
          Universe.Verb verb) {
        return new Universe.PartialRelation() {
            Obj = obj,
            Verb = verb,
            Subject = true
        };
      }

      public Universe.Relation Complete(Universe.Obj other) {
        return new Universe.Relation() {
            From = this.Subject ? this.Obj : other,
            To = this.Subject ? other : this.Obj,
            Verb = this.Verb
        };
      }

      public override bool Equals(object obj) {
        return obj is Universe.PartialRelation o && this.Equals(o);
      }

      public bool Equals(Universe.PartialRelation o) {
        return this.Obj.Equals(o.Obj) &&
               this.Verb == o.Verb &&
               this.Subject == o.Subject;
      }

      public override int GetHashCode() {
        return (int) this.Verb << 24 ^
               this.Obj.GetHashCode() ^
               (this.Subject ? 0 : (int) ushort.MaxValue);
      }
    }

    public class RelationSpec {
      public Universe.Verb Verb { get; set; }

      public bool Subject { get; set; }

      public Universe.Namespace Type { get; set; }

      public static Universe.RelationSpec Verbs(
          Universe.Verb verb,
          Universe.Namespace type = Universe.Namespace.GLOBAL) {
        return new Universe.RelationSpec() {
            Verb = verb,
            Subject = true,
            Type = type
        };
      }

      public static Universe.RelationSpec VerbedBy(
          Universe.Verb verb,
          Universe.Namespace type = Universe.Namespace.GLOBAL) {
        return new Universe.RelationSpec() {
            Verb = verb,
            Subject = false,
            Type = type
        };
      }
    }
  }
}