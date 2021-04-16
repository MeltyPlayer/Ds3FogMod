// Decompiled with JetBrains decompiler
// Type: SoulsFormats.DRB
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace SoulsFormats {
  [ComVisible(true)]
  public class DRB {
    public DCX.Type Compression = DCX.Type.None;
    private const int ANIK_SIZE = 32;
    private const int ANIO_SIZE = 16;
    private const int SCDK_SIZE = 32;
    private const int SCDO_SIZE = 16;
    private const int DLGO_SIZE = 32;

    public bool DSR { get; set; }

    public List<DRB.Texture> Textures { get; set; }

    public byte[] AnipBytes { get; set; }

    public byte[] IntpBytes { get; set; }

    public List<DRB.Anim> Anims { get; set; }

    public List<DRB.Scdl> Scdls { get; set; }

    public List<DRB.Dlg> Dlgs { get; set; }

    private static bool Is(BinaryReaderEx br) {
      return br.Length >= 4L && br.GetASCII(0L, 4) == "DRB\0";
    }

    private void Read(BinaryReaderEx br, bool dsr) {
      br.BigEndian = false;
      this.DSR = dsr;
      DRB.ReadNullBlock(br, "DRB\0");
      Dictionary<int, string> strings = this.ReadSTR(br);
      this.Textures = this.ReadTEXI(br, strings);
      long shprStart = DRB.ReadBlobBlock(br, "SHPR");
      long ctprStart = DRB.ReadBlobBlock(br, "CTPR");
      this.AnipBytes = DRB.ReadBlobBytes(br, "ANIP");
      this.IntpBytes = DRB.ReadBlobBytes(br, "INTP");
      long scdpStart = DRB.ReadBlobBlock(br, "SCDP");
      Dictionary<int, DRB.Shape> shapes =
          this.ReadSHAP(br, dsr, strings, shprStart);
      Dictionary<int, DRB.Control> controls =
          this.ReadCTRL(br, strings, ctprStart);
      Dictionary<int, DRB.Anik> aniks = this.ReadANIK(br, strings);
      Dictionary<int, DRB.Anio> anios = this.ReadANIO(br, aniks);
      this.Anims = this.ReadANIM(br, strings, anios);
      Dictionary<int, DRB.Scdk> scdks = this.ReadSCDK(br, strings, scdpStart);
      Dictionary<int, DRB.Scdo> scdos = this.ReadSCDO(br, strings, scdks);
      this.Scdls = this.ReadSCDL(br, strings, scdos);
      Dictionary<int, DRB.Dlgo> dlgos =
          this.ReadDLGO(br, strings, shapes, controls);
      this.Dlgs = this.ReadDLG(br, strings, shapes, controls, dlgos);
      DRB.ReadNullBlock(br, "END\0");
      foreach (DRB.Dlg dlg in this.Dlgs) {
        CheckShape(dlg.Shape);
        foreach (DRB.Dlgo dlgo in dlg.Dlgos)
          CheckShape(dlgo.Shape);
      }

      void CheckShape(DRB.Shape shape) {
        if (!(shape is DRB.Shape.Dialog dialog) ||
            dialog.DlgIndex == (short) -1)
          return;
        dialog.Dlg = this.Dlgs[(int) dialog.DlgIndex];
      }
    }

    private void Write(BinaryWriterEx bw) {
      foreach (DRB.Dlg dlg in this.Dlgs) {
        CheckShape(dlg.Shape);
        foreach (DRB.Dlgo dlgo in dlg.Dlgos)
          CheckShape(dlgo.Shape);
      }
      bw.BigEndian = false;
      DRB.WriteNullBlock(bw, "DRB\0");
      Dictionary<string, int> stringOffsets = this.WriteSTR(bw);
      this.WriteTEXI(bw, stringOffsets);
      Queue<int> shprOffsets = this.WriteSHPR(bw, this.DSR, stringOffsets);
      Queue<int> ctprOffsets = this.WriteCTPR(bw);
      DRB.WriteBlobBytes(bw, "ANIP", this.AnipBytes);
      DRB.WriteBlobBytes(bw, "INTP", this.IntpBytes);
      Queue<int> scdpOffsets = this.WriteSCDP(bw);
      Queue<int> shapOffsets = this.WriteSHAP(bw, stringOffsets, shprOffsets);
      Queue<int> ctrlOffsets = this.WriteCTRL(bw, stringOffsets, ctprOffsets);
      Queue<int> anikOffsets = this.WriteANIK(bw, stringOffsets);
      Queue<int> anioOffsets = this.WriteANIO(bw, anikOffsets);
      this.WriteANIM(bw, stringOffsets, anioOffsets);
      Queue<int> scdkOffsets = this.WriteSCDK(bw, stringOffsets, scdpOffsets);
      Queue<int> scdoOffsets = this.WriteSCDO(bw, stringOffsets, scdkOffsets);
      this.WriteSCDL(bw, stringOffsets, scdoOffsets);
      Queue<int> dlgoOffsets =
          this.WriteDLGO(bw, stringOffsets, shapOffsets, ctrlOffsets);
      this.WriteDLG(bw, stringOffsets, shapOffsets, ctrlOffsets, dlgoOffsets);
      DRB.WriteNullBlock(bw, "END\0");

      void CheckShape(DRB.Shape shape) {
        if (!(shape is DRB.Shape.Dialog dialog))
          return;
        if (dialog.Dlg == null) {
          dialog.DlgIndex = (short) -1;
        } else {
          if (!this.Dlgs.Contains(dialog.Dlg))
            throw new InvalidDataException(
                "Dlg \"" +
                dialog.Dlg.Name +
                "\" is referenced but no found in Dlgs list.");
          dialog.DlgIndex = (short) this.Dlgs.IndexOf(dialog.Dlg);
        }
      }
    }

    public DRB.Dlg this[string name] {
      get {
        return this.Dlgs.Find((Predicate<DRB.Dlg>) (dlg => dlg.Name == name));
      }
    }

    private Dictionary<int, string> ReadSTR(BinaryReaderEx br) {
      int count;
      long num = DRB.ReadBlockHeader(br, "STR\0", out count);
      Dictionary<int, string> dictionary = new Dictionary<int, string>(count);
      for (int index1 = 0; index1 < count; ++index1) {
        int index2 = (int) (br.Position - num);
        dictionary[index2] = br.ReadUTF16();
      }
      br.Pad(16);
      return dictionary;
    }

    private Dictionary<string, int> WriteSTR(BinaryWriterEx bw) {
      var start = DRB.WriteBlockHeader(bw, "STR\0");
      var stringOffsets = new Dictionary<string, int>();

      void writeString(string str) {
        // ISSUE: reference to a compiler-generated field
        if (stringOffsets.ContainsKey(str))
          return;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        long num = bw.Position - start;
        // ISSUE: reference to a compiler-generated field
        bw.WriteUTF16(str, true);
        // ISSUE: reference to a compiler-generated field
        stringOffsets[str] = (int) num;
      }

      void writeShapeStrings(DRB.Shape shape) {
        writeString(shape.Type.ToString());
        switch (shape) {
          case DRB.Shape.ScrollText scrollText
              when scrollText.TextType == DRB.Shape.TxtType.Literal:
            writeString(scrollText.TextLiteral);
            break;
          case DRB.Shape.Text text
              when text.TextType == DRB.Shape.TxtType.Literal:
            writeString(text.TextLiteral);
            break;
        }
      }

      foreach (DRB.Texture texture in this.Textures) {
        writeString(texture.Name);
        writeString(texture.Path);
      }
      foreach (DRB.Anim anim in this.Anims) {
        writeString(anim.Name);
        foreach (DRB.Anio anio in anim.Anios) {
          foreach (DRB.Anik anik in anio.Aniks)
            writeString(anik.Name);
        }
      }
      foreach (DRB.Scdl scdl in this.Scdls) {
        writeString(scdl.Name);
        foreach (DRB.Scdo scdo in scdl.Scdos) {
          writeString(scdo.Name);
          foreach (DRB.Scdk scdk in scdo.Scdks)
            writeString(scdk.Name);
        }
      }
      foreach (DRB.Dlg dlg in this.Dlgs) {
        writeString(dlg.Name);
        writeShapeStrings(dlg.Shape);
        writeString(dlg.Control.Type.ToString());
        foreach (DRB.Dlgo dlgo in dlg.Dlgos) {
          writeString(dlgo.Name);
          writeShapeStrings(dlgo.Shape);
          writeString(dlgo.Control.Type.ToString());
        }
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      DRB.FinishBlockHeader(bw, "STR\0", start, stringOffsets.Count);
      // ISSUE: reference to a compiler-generated field
      return stringOffsets;
    }

    private List<DRB.Texture> ReadTEXI(
        BinaryReaderEx br,
        Dictionary<int, string> strings) {
      int count;
      DRB.ReadBlockHeader(br, "TEXI", out count);
      List<DRB.Texture> textureList = new List<DRB.Texture>(count);
      for (int index = 0; index < count; ++index)
        textureList.Add(new DRB.Texture(br, strings));
      br.Pad(16);
      return textureList;
    }

    private void WriteTEXI(
        BinaryWriterEx bw,
        Dictionary<string, int> stringOffsets) {
      long start = DRB.WriteBlockHeader(bw, "TEXI");
      foreach (DRB.Texture texture in this.Textures)
        texture.Write(bw, stringOffsets);
      DRB.FinishBlockHeader(bw, "TEXI", start, this.Textures.Count);
    }

    private Queue<int> WriteSHPR(
        BinaryWriterEx bw,
        bool dsr,
        Dictionary<string, int> stringOffsets) {
      var start = DRB.WriteBlobBlock(bw, "SHPR");
      var shprOffsets = new Queue<int>();


      void writeShape(DRB.Shape shape) {
        int num = (int) (bw.Position - start);
        shprOffsets.Enqueue(num);
        shape.WriteData(bw, dsr, stringOffsets);
      }

      foreach (DRB.Dlg dlg in this.Dlgs) {
        foreach (DRB.Dlgo dlgo in dlg.Dlgos)
          writeShape(dlgo.Shape);
      }
      foreach (DRB.Dlgo dlg in this.Dlgs)
        writeShape(dlg.Shape);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      DRB.FinishBlobBlock(bw, "SHPR", start);
      // ISSUE: reference to a compiler-generated field
      return shprOffsets;
    }

    private Queue<int> WriteCTPR(BinaryWriterEx bw) {
      var start = DRB.WriteBlobBlock(bw, "CTPR");
      var ctprOffsets = new Queue<int>();

      void writeControl(DRB.Control control) {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        int num = (int) (bw.Position - start);
        // ISSUE: reference to a compiler-generated field
        ctprOffsets.Enqueue(num);
        // ISSUE: reference to a compiler-generated field
        control.WriteData(bw);
      }

      foreach (DRB.Dlg dlg in this.Dlgs) {
        foreach (DRB.Dlgo dlgo in dlg.Dlgos)
          writeControl(dlgo.Control);
      }
      foreach (DRB.Dlgo dlg in this.Dlgs)
        writeControl(dlg.Control);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      DRB.FinishBlobBlock(bw, "CTPR", start);
      // ISSUE: reference to a compiler-generated field
      return ctprOffsets;
    }

    private Queue<int> WriteSCDP(BinaryWriterEx bw) {
      long start = DRB.WriteBlobBlock(bw, "SCDP");
      Queue<int> intQueue = new Queue<int>();
      foreach (DRB.Scdl scdl in this.Scdls) {
        foreach (DRB.Scdo scdo in scdl.Scdos) {
          foreach (DRB.Scdk scdk in scdo.Scdks) {
            int num = (int) (bw.Position - start);
            intQueue.Enqueue(num);
            BinaryWriterEx bw1 = bw;
            scdk.WriteSCDP(bw1);
          }
        }
      }
      DRB.FinishBlobBlock(bw, "SCDP", start);
      return intQueue;
    }

    private Dictionary<int, DRB.Shape> ReadSHAP(
        BinaryReaderEx br,
        bool dsr,
        Dictionary<int, string> strings,
        long shprStart) {
      int count;
      long num = DRB.ReadBlockHeader(br, "SHAP", out count);
      Dictionary<int, DRB.Shape> dictionary =
          new Dictionary<int, DRB.Shape>(count);
      for (int index1 = 0; index1 < count; ++index1) {
        int index2 = (int) (br.Position - num);
        dictionary[index2] = DRB.Shape.Read(br, dsr, strings, shprStart);
      }
      br.Pad(16);
      return dictionary;
    }

    private Queue<int> WriteSHAP(
        BinaryWriterEx bw,
        Dictionary<string, int> stringOffsets,
        Queue<int> shprOffsets) {
      var start = DRB.WriteBlockHeader(bw, "SHAP");
      var shapOffsets = new Queue<int>();

      void writeShape(DRB.Shape shape) {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        int num = (int) (bw.Position - start);
        // ISSUE: reference to a compiler-generated field
        shapOffsets.Enqueue(num);
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        shape.WriteHeader(bw, stringOffsets, shprOffsets);
      }

      foreach (DRB.Dlg dlg in this.Dlgs) {
        foreach (DRB.Dlgo dlgo in dlg.Dlgos)
          writeShape(dlgo.Shape);
      }
      foreach (DRB.Dlgo dlg in this.Dlgs)
        writeShape(dlg.Shape);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      DRB.FinishBlockHeader(bw, "SHAP", start, shapOffsets.Count);
      // ISSUE: reference to a compiler-generated field
      return shapOffsets;
    }

    private Dictionary<int, DRB.Control> ReadCTRL(
        BinaryReaderEx br,
        Dictionary<int, string> strings,
        long ctprStart) {
      int count;
      long num = DRB.ReadBlockHeader(br, "CTRL", out count);
      Dictionary<int, DRB.Control> dictionary =
          new Dictionary<int, DRB.Control>(count);
      for (int index1 = 0; index1 < count; ++index1) {
        int index2 = (int) (br.Position - num);
        dictionary[index2] = DRB.Control.Read(br, strings, ctprStart);
      }
      br.Pad(16);
      return dictionary;
    }

    private Queue<int> WriteCTRL(
        BinaryWriterEx bw,
        Dictionary<string, int> stringOffsets,
        Queue<int> ctprOffsets) {
      var start = DRB.WriteBlockHeader(bw, "CTRL");
      var ctrlOffsets = new Queue<int>();

      void writeControl(DRB.Control control) {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        int num = (int) (bw.Position - start);
        // ISSUE: reference to a compiler-generated field
        ctrlOffsets.Enqueue(num);
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        control.WriteHeader(bw, stringOffsets, ctprOffsets);
      }

      foreach (DRB.Dlg dlg in this.Dlgs) {
        foreach (DRB.Dlgo dlgo in dlg.Dlgos)
          writeControl(dlgo.Control);
      }
      foreach (DRB.Dlgo dlg in this.Dlgs)
        writeControl(dlg.Control);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      DRB.FinishBlockHeader(bw, "CTRL", start, ctrlOffsets.Count);
      // ISSUE: reference to a compiler-generated field
      return ctrlOffsets;
    }

    private Dictionary<int, DRB.Anik> ReadANIK(
        BinaryReaderEx br,
        Dictionary<int, string> strings) {
      int count;
      long num = DRB.ReadBlockHeader(br, "ANIK", out count);
      Dictionary<int, DRB.Anik> dictionary =
          new Dictionary<int, DRB.Anik>(count);
      for (int index1 = 0; index1 < count; ++index1) {
        int index2 = (int) (br.Position - num);
        dictionary[index2] = new DRB.Anik(br, strings);
      }
      br.Pad(16);
      return dictionary;
    }

    private Queue<int> WriteANIK(
        BinaryWriterEx bw,
        Dictionary<string, int> stringOffsets) {
      long start = DRB.WriteBlockHeader(bw, "ANIK");
      int count = 0;
      Queue<int> intQueue = new Queue<int>();
      foreach (DRB.Anim anim in this.Anims) {
        foreach (DRB.Anio anio in anim.Anios) {
          int num = (int) (bw.Position - start);
          intQueue.Enqueue(num);
          count += anio.Aniks.Count;
          foreach (DRB.Anik anik in anio.Aniks)
            anik.Write(bw, stringOffsets);
        }
      }
      DRB.FinishBlockHeader(bw, "ANIK", start, count);
      return intQueue;
    }

    private Dictionary<int, DRB.Anio> ReadANIO(
        BinaryReaderEx br,
        Dictionary<int, DRB.Anik> aniks) {
      int count;
      long num = DRB.ReadBlockHeader(br, "ANIO", out count);
      Dictionary<int, DRB.Anio> dictionary =
          new Dictionary<int, DRB.Anio>(count);
      for (int index1 = 0; index1 < count; ++index1) {
        int index2 = (int) (br.Position - num);
        dictionary[index2] = new DRB.Anio(br, aniks);
      }
      br.Pad(16);
      return dictionary;
    }

    private Queue<int> WriteANIO(BinaryWriterEx bw, Queue<int> anikOffsets) {
      long start = DRB.WriteBlockHeader(bw, "ANIO");
      int count = 0;
      Queue<int> intQueue = new Queue<int>();
      foreach (DRB.Anim anim in this.Anims) {
        int num = (int) (bw.Position - start);
        intQueue.Enqueue(num);
        count += anim.Anios.Count;
        foreach (DRB.Anio anio in anim.Anios)
          anio.Write(bw, anikOffsets);
      }
      DRB.FinishBlockHeader(bw, "ANIO", start, count);
      return intQueue;
    }

    private List<DRB.Anim> ReadANIM(
        BinaryReaderEx br,
        Dictionary<int, string> strings,
        Dictionary<int, DRB.Anio> anios) {
      int count;
      DRB.ReadBlockHeader(br, "ANIM", out count);
      List<DRB.Anim> animList = new List<DRB.Anim>(count);
      for (int index = 0; index < count; ++index)
        animList.Add(new DRB.Anim(br, strings, anios));
      br.Pad(16);
      return animList;
    }

    private void WriteANIM(
        BinaryWriterEx bw,
        Dictionary<string, int> stringOffsets,
        Queue<int> anioOffsets) {
      long start = DRB.WriteBlockHeader(bw, "ANIM");
      foreach (DRB.Anim anim in this.Anims)
        anim.Write(bw, stringOffsets, anioOffsets);
      DRB.FinishBlockHeader(bw, "ANIM", start, this.Anims.Count);
    }

    private Dictionary<int, DRB.Scdk> ReadSCDK(
        BinaryReaderEx br,
        Dictionary<int, string> strings,
        long scdpStart) {
      int count;
      long num = DRB.ReadBlockHeader(br, "SCDK", out count);
      Dictionary<int, DRB.Scdk> dictionary =
          new Dictionary<int, DRB.Scdk>(count);
      for (int index1 = 0; index1 < count; ++index1) {
        int index2 = (int) (br.Position - num);
        dictionary[index2] = new DRB.Scdk(br, strings, scdpStart);
      }
      br.Pad(16);
      return dictionary;
    }

    private Queue<int> WriteSCDK(
        BinaryWriterEx bw,
        Dictionary<string, int> stringOffsets,
        Queue<int> scdpOffsets) {
      long start = DRB.WriteBlockHeader(bw, "SCDK");
      int count = 0;
      Queue<int> intQueue = new Queue<int>();
      foreach (DRB.Scdl scdl in this.Scdls) {
        foreach (DRB.Scdo scdo in scdl.Scdos) {
          int num = (int) (bw.Position - start);
          intQueue.Enqueue(num);
          count += scdo.Scdks.Count;
          foreach (DRB.Scdk scdk in scdo.Scdks)
            scdk.Write(bw, stringOffsets, scdpOffsets);
        }
      }
      DRB.FinishBlockHeader(bw, "SCDK", start, count);
      return intQueue;
    }

    private Dictionary<int, DRB.Scdo> ReadSCDO(
        BinaryReaderEx br,
        Dictionary<int, string> strings,
        Dictionary<int, DRB.Scdk> scdks) {
      int count;
      long num = DRB.ReadBlockHeader(br, "SCDO", out count);
      Dictionary<int, DRB.Scdo> dictionary =
          new Dictionary<int, DRB.Scdo>(count);
      for (int index1 = 0; index1 < count; ++index1) {
        int index2 = (int) (br.Position - num);
        dictionary[index2] = new DRB.Scdo(br, strings, scdks);
      }
      br.Pad(16);
      return dictionary;
    }

    private Queue<int> WriteSCDO(
        BinaryWriterEx bw,
        Dictionary<string, int> stringOffsets,
        Queue<int> scdkOffsets) {
      long start = DRB.WriteBlockHeader(bw, "SCDO");
      int count = 0;
      Queue<int> intQueue = new Queue<int>();
      foreach (DRB.Scdl scdl in this.Scdls) {
        int num = (int) (bw.Position - start);
        intQueue.Enqueue(num);
        count += scdl.Scdos.Count;
        foreach (DRB.Scdo scdo in scdl.Scdos)
          scdo.Write(bw, stringOffsets, scdkOffsets);
      }
      DRB.FinishBlockHeader(bw, "SCDO", start, count);
      return intQueue;
    }

    private List<DRB.Scdl> ReadSCDL(
        BinaryReaderEx br,
        Dictionary<int, string> strings,
        Dictionary<int, DRB.Scdo> scdos) {
      int count;
      DRB.ReadBlockHeader(br, "SCDL", out count);
      List<DRB.Scdl> scdlList = new List<DRB.Scdl>(count);
      for (int index = 0; index < count; ++index)
        scdlList.Add(new DRB.Scdl(br, strings, scdos));
      br.Pad(16);
      return scdlList;
    }

    private void WriteSCDL(
        BinaryWriterEx bw,
        Dictionary<string, int> stringOffsets,
        Queue<int> scdoOffsets) {
      long start = DRB.WriteBlockHeader(bw, "SCDL");
      foreach (DRB.Scdl scdl in this.Scdls)
        scdl.Write(bw, stringOffsets, scdoOffsets);
      DRB.FinishBlockHeader(bw, "SCDL", start, this.Scdls.Count);
    }

    private Dictionary<int, DRB.Dlgo> ReadDLGO(
        BinaryReaderEx br,
        Dictionary<int, string> strings,
        Dictionary<int, DRB.Shape> shapes,
        Dictionary<int, DRB.Control> controls) {
      int count;
      long num = DRB.ReadBlockHeader(br, "DLGO", out count);
      Dictionary<int, DRB.Dlgo> dictionary =
          new Dictionary<int, DRB.Dlgo>(count);
      for (int index1 = 0; index1 < count; ++index1) {
        int index2 = (int) (br.Position - num);
        dictionary[index2] = new DRB.Dlgo(br, strings, shapes, controls);
      }
      br.Pad(16);
      return dictionary;
    }

    private Queue<int> WriteDLGO(
        BinaryWriterEx bw,
        Dictionary<string, int> stringOffsets,
        Queue<int> shapOffsets,
        Queue<int> ctrlOffsets) {
      long start = DRB.WriteBlockHeader(bw, "DLGO");
      int count = 0;
      Queue<int> intQueue = new Queue<int>();
      foreach (DRB.Dlg dlg in this.Dlgs) {
        int num = (int) (bw.Position - start);
        intQueue.Enqueue(num);
        count += dlg.Dlgos.Count;
        foreach (DRB.Dlgo dlgo in dlg.Dlgos)
          dlgo.Write(bw, stringOffsets, shapOffsets, ctrlOffsets);
      }
      DRB.FinishBlockHeader(bw, "DLGO", start, count);
      return intQueue;
    }

    private List<DRB.Dlg> ReadDLG(
        BinaryReaderEx br,
        Dictionary<int, string> strings,
        Dictionary<int, DRB.Shape> shapes,
        Dictionary<int, DRB.Control> controls,
        Dictionary<int, DRB.Dlgo> dlgos) {
      int count;
      DRB.ReadBlockHeader(br, "DLG\0", out count);
      List<DRB.Dlg> dlgList = new List<DRB.Dlg>(count);
      for (int index = 0; index < count; ++index)
        dlgList.Add(new DRB.Dlg(br, strings, shapes, controls, dlgos));
      br.Pad(16);
      return dlgList;
    }

    private void WriteDLG(
        BinaryWriterEx bw,
        Dictionary<string, int> stringOffsets,
        Queue<int> shapOffsets,
        Queue<int> ctrlOffsets,
        Queue<int> dlgoOffsets) {
      long start = DRB.WriteBlockHeader(bw, "DLG\0");
      foreach (DRB.Dlg dlg in this.Dlgs)
        dlg.Write(bw, stringOffsets, shapOffsets, ctrlOffsets, dlgoOffsets);
      DRB.FinishBlockHeader(bw, "DLG\0", start, this.Dlgs.Count);
    }

    private static void ReadNullBlock(BinaryReaderEx br, string name) {
      br.AssertASCII(name);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
      br.AssertInt32(new int[1]);
    }

    private static void WriteNullBlock(BinaryWriterEx bw, string name) {
      bw.WriteASCII(name, false);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
      bw.WriteInt32(0);
    }

    private static long ReadBlobBlock(BinaryReaderEx br, string name) {
      br.AssertASCII(name);
      int count = br.ReadInt32();
      br.AssertInt32(1);
      br.AssertInt32(new int[1]);
      long position = br.Position;
      br.Skip(count);
      br.Pad(16);
      return position;
    }

    private static long WriteBlobBlock(BinaryWriterEx bw, string name) {
      bw.WriteASCII(name, false);
      bw.ReserveInt32("BlobBlockSize" + name);
      bw.WriteInt32(1);
      bw.WriteInt32(0);
      return bw.Position;
    }

    private static void FinishBlobBlock(
        BinaryWriterEx bw,
        string name,
        long start) {
      bw.Pad(16);
      bw.FillInt32("BlobBlockSize" + name, (int) (bw.Position - start));
    }

    private static byte[] ReadBlobBytes(BinaryReaderEx br, string name) {
      br.AssertASCII(name);
      int count = br.ReadInt32();
      br.AssertInt32(1);
      br.AssertInt32(new int[1]);
      byte[] numArray = br.ReadBytes(count);
      br.Pad(16);
      return numArray;
    }

    private static void WriteBlobBytes(
        BinaryWriterEx bw,
        string name,
        byte[] bytes) {
      bw.WriteASCII(name, false);
      bw.ReserveInt32("BlobSize");
      bw.WriteInt32(1);
      bw.WriteInt32(0);
      long position = bw.Position;
      bw.WriteBytes(bytes);
      bw.Pad(16);
      bw.FillInt32("BlobSize", (int) (bw.Position - position));
    }

    private static long ReadBlockHeader(
        BinaryReaderEx br,
        string name,
        out int count) {
      br.AssertASCII(name);
      br.ReadInt32();
      count = br.ReadInt32();
      br.AssertInt32(new int[1]);
      return br.Position;
    }

    private static long WriteBlockHeader(BinaryWriterEx bw, string name) {
      bw.WriteASCII(name, false);
      bw.ReserveInt32("BlockSize" + name);
      bw.ReserveInt32("BlockCount" + name);
      bw.WriteInt32(0);
      return bw.Position;
    }

    private static void FinishBlockHeader(
        BinaryWriterEx bw,
        string name,
        long start,
        int count) {
      bw.Pad(16);
      bw.FillInt32("BlockSize" + name, (int) (bw.Position - start));
      bw.FillInt32("BlockCount" + name, count);
    }

    private static Color ReadABGR(BinaryReaderEx br) {
      byte[] numArray = br.ReadBytes(4);
      return Color.FromArgb((int) numArray[0],
                            (int) numArray[3],
                            (int) numArray[2],
                            (int) numArray[1]);
    }

    private static void WriteABGR(BinaryWriterEx bw, Color color) {
      bw.WriteByte(color.A);
      bw.WriteByte(color.B);
      bw.WriteByte(color.G);
      bw.WriteByte(color.R);
    }

    public static bool Is(byte[] bytes) {
      return bytes.Length != 0 &&
             DRB.Is(SFUtil.GetDecompressedBR(new BinaryReaderEx(false, bytes),
                                             out DCX.Type _));
    }

    public static bool Is(string path) {
      using (FileStream fileStream = File.OpenRead(path))
        return fileStream.Length != 0L &&
               DRB.Is(SFUtil.GetDecompressedBR(
                          new BinaryReaderEx(false, (Stream) fileStream),
                          out DCX.Type _));
    }

    public static DRB Read(byte[] bytes, bool dsr) {
      BinaryReaderEx br = new BinaryReaderEx(false, bytes);
      DRB drb = new DRB();
      BinaryReaderEx decompressedBr =
          SFUtil.GetDecompressedBR(br, out drb.Compression);
      drb.Read(decompressedBr, dsr);
      return drb;
    }

    public static DRB Read(string path, bool dsr) {
      using (FileStream fileStream = File.OpenRead(path)) {
        BinaryReaderEx br = new BinaryReaderEx(false, (Stream) fileStream);
        DRB drb = new DRB();
        BinaryReaderEx decompressedBr =
            SFUtil.GetDecompressedBR(br, out drb.Compression);
        drb.Read(decompressedBr, dsr);
        return drb;
      }
    }

    private void Write(BinaryWriterEx bw, DCX.Type compression) {
      if (compression == DCX.Type.None) {
        this.Write(bw);
      } else {
        BinaryWriterEx bw1 = new BinaryWriterEx(false);
        this.Write(bw1);
        DCX.Compress(bw1.FinishBytes(), bw, compression);
      }
    }

    public byte[] Write() {
      return this.Write(this.Compression);
    }

    public byte[] Write(DCX.Type compression) {
      BinaryWriterEx bw = new BinaryWriterEx(false);
      this.Write(bw, compression);
      return bw.FinishBytes();
    }

    public void Write(string path) {
      this.Write(path, this.Compression);
    }

    public void Write(string path, DCX.Type compression) {
      using (FileStream fileStream = File.Create(path)) {
        BinaryWriterEx bw = new BinaryWriterEx(false, (Stream) fileStream);
        this.Write(bw, compression);
        bw.Finish();
      }
    }

    public class Texture {
      public string Name { get; set; }

      public string Path { get; set; }

      public Texture() {
        this.Name = "";
        this.Path = "";
      }

      public Texture(string name, string path) {
        this.Name = name;
        this.Path = path;
      }

      internal Texture(BinaryReaderEx br, Dictionary<int, string> strings) {
        int index1 = br.ReadInt32();
        int index2 = br.ReadInt32();
        br.AssertInt32(new int[1]);
        br.AssertInt32(new int[1]);
        this.Name = strings[index1];
        this.Path = strings[index2];
      }

      internal void Write(
          BinaryWriterEx bw,
          Dictionary<string, int> stringOffsets) {
        bw.WriteInt32(stringOffsets[this.Name]);
        bw.WriteInt32(stringOffsets[this.Path]);
        bw.WriteInt32(0);
        bw.WriteInt32(0);
      }

      public override string ToString() {
        return this.Name + " - " + this.Path;
      }
    }

    public enum ShapeType {
      Dialog,
      GouraudRect,
      GouraudSprite,
      Mask,
      MonoFrame,
      MonoRect,
      Null,
      ScrollText,
      Sprite,
      Text,
    }

    public abstract class Shape {
      public abstract DRB.ShapeType Type { get; }

      public short LeftEdge { get; set; }

      public short TopEdge { get; set; }

      public short RightEdge { get; set; }

      public short BottomEdge { get; set; }

      public short ScalingOriginX { get; set; }

      public short ScalingOriginY { get; set; }

      public int ScalingType { get; set; }

      internal Shape() {
        this.ScalingOriginX = (short) -1;
        this.ScalingOriginY = (short) -1;
      }

      internal static DRB.Shape Read(
          BinaryReaderEx br,
          bool dsr,
          Dictionary<int, string> strings,
          long shprStart) {
        int index = br.ReadInt32();
        int num = br.ReadInt32();
        string str = strings[index];
        br.StepIn(shprStart + (long) num);
        DRB.Shape shape;
        if (str == "Dialog")
          shape = (DRB.Shape) new DRB.Shape.Dialog(br, dsr);
        else if (str == "GouraudRect")
          shape = (DRB.Shape) new DRB.Shape.GouraudRect(br, dsr);
        else if (str == "GouraudSprite")
          shape = (DRB.Shape) new DRB.Shape.GouraudSprite(br, dsr);
        else if (str == "Mask")
          shape = (DRB.Shape) new DRB.Shape.Mask(br, dsr);
        else if (str == "MonoFrame")
          shape = (DRB.Shape) new DRB.Shape.MonoFrame(br, dsr);
        else if (str == "MonoRect")
          shape = (DRB.Shape) new DRB.Shape.MonoRect(br, dsr);
        else if (str == "Null")
          shape = (DRB.Shape) new DRB.Shape.Null(br, dsr);
        else if (str == "ScrollText")
          shape = (DRB.Shape) new DRB.Shape.ScrollText(br, dsr, strings);
        else if (str == "Sprite") {
          shape = (DRB.Shape) new DRB.Shape.Sprite(br, dsr);
        } else {
          if (!(str == "Text"))
            throw new InvalidDataException("Unknown shape type: " + str);
          shape = (DRB.Shape) new DRB.Shape.Text(br, dsr, strings);
        }
        br.StepOut();
        return shape;
      }

      internal Shape(BinaryReaderEx br, bool dsr) {
        this.LeftEdge = br.ReadInt16();
        this.TopEdge = br.ReadInt16();
        this.RightEdge = br.ReadInt16();
        this.BottomEdge = br.ReadInt16();
        if (dsr && this.Type != DRB.ShapeType.Null) {
          this.ScalingOriginX = br.ReadInt16();
          this.ScalingOriginY = br.ReadInt16();
          this.ScalingType = br.ReadInt32();
        } else {
          this.ScalingOriginX = (short) -1;
          this.ScalingOriginY = (short) -1;
          this.ScalingType = 0;
        }
      }

      internal void WriteData(
          BinaryWriterEx bw,
          bool dsr,
          Dictionary<string, int> stringOffsets) {
        bw.WriteInt16(this.LeftEdge);
        bw.WriteInt16(this.TopEdge);
        bw.WriteInt16(this.RightEdge);
        bw.WriteInt16(this.BottomEdge);
        if (dsr && this.Type != DRB.ShapeType.Null) {
          bw.WriteInt16(this.ScalingOriginX);
          bw.WriteInt16(this.ScalingOriginY);
          bw.WriteInt32(this.ScalingType);
        }
        this.WriteSpecific(bw, stringOffsets);
      }

      internal abstract void WriteSpecific(
          BinaryWriterEx bw,
          Dictionary<string, int> stringOffsets);

      internal void WriteHeader(
          BinaryWriterEx bw,
          Dictionary<string, int> stringOffsets,
          Queue<int> shprOffsets) {
        bw.WriteInt32(stringOffsets[this.Type.ToString()]);
        bw.WriteInt32(shprOffsets.Dequeue());
      }

      public override string ToString() {
        return string.Format("{0} ({1}, {2}) ({3}, {4})",
                             (object) this.Type,
                             (object) this.LeftEdge,
                             (object) this.TopEdge,
                             (object) this.RightEdge,
                             (object) this.BottomEdge);
      }

      public class Dialog : DRB.Shape {
        internal short DlgIndex;

        public override DRB.ShapeType Type {
          get { return DRB.ShapeType.Dialog; }
        }

        public DRB.Dlg Dlg { get; set; }

        public byte Unk02 { get; set; }

        public byte Unk03 { get; set; }

        public int Unk04 { get; set; }

        public int Unk08 { get; set; }

        public int Unk0C { get; set; }

        public Dialog() {
          this.DlgIndex = (short) -1;
          this.Unk03 = (byte) 1;
        }

        internal Dialog(BinaryReaderEx br, bool dsr)
            : base(br, dsr) {
          this.DlgIndex = br.ReadInt16();
          this.Unk02 = br.ReadByte();
          this.Unk03 = br.ReadByte();
          this.Unk04 = br.ReadInt32();
          this.Unk08 = br.ReadInt32();
          this.Unk0C = br.ReadInt32();
        }

        internal override void WriteSpecific(
            BinaryWriterEx bw,
            Dictionary<string, int> stringOffsets) {
          bw.WriteInt16(this.DlgIndex);
          bw.WriteByte(this.Unk02);
          bw.WriteByte(this.Unk03);
          bw.WriteInt32(this.Unk04);
          bw.WriteInt32(this.Unk08);
          bw.WriteInt32(this.Unk0C);
        }
      }

      public class GouraudRect : DRB.Shape {
        public override DRB.ShapeType Type {
          get { return DRB.ShapeType.GouraudRect; }
        }

        public int Unk00 { get; set; }

        public Color TopLeftColor { get; set; }

        public Color TopRightColor { get; set; }

        public Color BottomRightColor { get; set; }

        public Color BottomLeftColor { get; set; }

        public GouraudRect() {
          this.Unk00 = 1;
        }

        internal GouraudRect(BinaryReaderEx br, bool dsr)
            : base(br, dsr) {
          this.Unk00 = br.ReadInt32();
          this.TopLeftColor = DRB.ReadABGR(br);
          this.TopRightColor = DRB.ReadABGR(br);
          this.BottomRightColor = DRB.ReadABGR(br);
          this.BottomLeftColor = DRB.ReadABGR(br);
        }

        internal override void WriteSpecific(
            BinaryWriterEx bw,
            Dictionary<string, int> stringOffsets) {
          bw.WriteInt32(this.Unk00);
          DRB.WriteABGR(bw, this.TopLeftColor);
          DRB.WriteABGR(bw, this.TopRightColor);
          DRB.WriteABGR(bw, this.BottomRightColor);
          DRB.WriteABGR(bw, this.BottomLeftColor);
        }
      }

      public class GouraudSprite : DRB.Shape {
        public override DRB.ShapeType Type {
          get { return DRB.ShapeType.GouraudSprite; }
        }

        public int Unk00 { get; set; }

        public int Unk04 { get; set; }

        public short Unk08 { get; set; }

        public short Unk0A { get; set; }

        public Color TopLeftColor { get; set; }

        public Color TopRightColor { get; set; }

        public Color BottomRightColor { get; set; }

        public Color BottomLeftColor { get; set; }

        public GouraudSprite() {
          this.Unk08 = (short) -1;
          this.Unk0A = (short) 256;
        }

        internal GouraudSprite(BinaryReaderEx br, bool dsr)
            : base(br, dsr) {
          this.Unk00 = br.ReadInt32();
          this.Unk04 = br.ReadInt32();
          this.Unk08 = br.ReadInt16();
          this.Unk0A = br.ReadInt16();
          this.TopLeftColor = DRB.ReadABGR(br);
          this.TopRightColor = DRB.ReadABGR(br);
          this.BottomRightColor = DRB.ReadABGR(br);
          this.BottomLeftColor = DRB.ReadABGR(br);
        }

        internal override void WriteSpecific(
            BinaryWriterEx bw,
            Dictionary<string, int> stringOffsets) {
          bw.WriteInt32(this.Unk00);
          bw.WriteInt32(this.Unk04);
          bw.WriteInt16(this.Unk08);
          bw.WriteInt16(this.Unk0A);
          DRB.WriteABGR(bw, this.TopLeftColor);
          DRB.WriteABGR(bw, this.TopRightColor);
          DRB.WriteABGR(bw, this.BottomRightColor);
          DRB.WriteABGR(bw, this.BottomLeftColor);
        }
      }

      public class Mask : DRB.Shape {
        public override DRB.ShapeType Type {
          get { return DRB.ShapeType.Mask; }
        }

        public int Unk00 { get; set; }

        public int Unk04 { get; set; }

        public int Unk08 { get; set; }

        public byte Unk0C { get; set; }

        public byte Unk0D { get; set; }

        public byte Unk0E { get; set; }

        public Mask() {
          this.Unk04 = 1;
        }

        internal Mask(BinaryReaderEx br, bool dsr)
            : base(br, dsr) {
          this.Unk00 = br.ReadInt32();
          this.Unk04 = br.ReadInt32();
          this.Unk08 = br.ReadInt32();
          this.Unk0C = br.ReadByte();
          this.Unk0D = br.ReadByte();
          this.Unk0E = br.ReadByte();
        }

        internal override void WriteSpecific(
            BinaryWriterEx bw,
            Dictionary<string, int> stringOffsets) {
          bw.WriteInt32(this.Unk00);
          bw.WriteInt32(this.Unk04);
          bw.WriteInt32(this.Unk08);
          bw.WriteByte(this.Unk0C);
          bw.WriteByte(this.Unk0D);
          bw.WriteByte(this.Unk0E);
        }
      }

      public class MonoFrame : DRB.Shape {
        public override DRB.ShapeType Type {
          get { return DRB.ShapeType.MonoFrame; }
        }

        public byte Unk00 { get; set; }

        public byte Unk01 { get; set; }

        public byte Unk02 { get; set; }

        public byte Unk03 { get; set; }

        public int Unk04 { get; set; }

        public int Unk08 { get; set; }

        public MonoFrame() {
          this.Unk00 = (byte) 1;
          this.Unk03 = (byte) 1;
        }

        internal MonoFrame(BinaryReaderEx br, bool dsr)
            : base(br, dsr) {
          this.Unk00 = br.ReadByte();
          this.Unk01 = br.ReadByte();
          this.Unk02 = br.ReadByte();
          this.Unk03 = br.ReadByte();
          this.Unk04 = br.ReadInt32();
          this.Unk08 = br.ReadInt32();
        }

        internal override void WriteSpecific(
            BinaryWriterEx bw,
            Dictionary<string, int> stringOffsets) {
          bw.WriteByte(this.Unk00);
          bw.WriteByte(this.Unk01);
          bw.WriteByte(this.Unk02);
          bw.WriteByte(this.Unk03);
          bw.WriteInt32(this.Unk04);
          bw.WriteInt32(this.Unk08);
        }
      }

      public class MonoRect : DRB.Shape {
        public override DRB.ShapeType Type {
          get { return DRB.ShapeType.MonoRect; }
        }

        public int Unk00 { get; set; }

        public int PaletteColor { get; set; }

        public Color CustomColor { get; set; }

        public MonoRect() {
          this.Unk00 = 1;
        }

        internal MonoRect(BinaryReaderEx br, bool dsr)
            : base(br, dsr) {
          this.Unk00 = br.ReadInt32();
          this.PaletteColor = br.ReadInt32();
          this.CustomColor = DRB.ReadABGR(br);
        }

        internal override void WriteSpecific(
            BinaryWriterEx bw,
            Dictionary<string, int> stringOffsets) {
          bw.WriteInt32(this.Unk00);
          bw.WriteInt32(this.PaletteColor);
          DRB.WriteABGR(bw, this.CustomColor);
        }
      }

      public class Null : DRB.Shape {
        public override DRB.ShapeType Type {
          get { return DRB.ShapeType.Null; }
        }

        public Null() {}

        internal Null(BinaryReaderEx br, bool dsr)
            : base(br, dsr) {}

        internal override void WriteSpecific(
            BinaryWriterEx bw,
            Dictionary<string, int> stringOffsets) {}
      }

      public class ScrollText : DRB.Shape.TextBase {
        public override DRB.ShapeType Type {
          get { return DRB.ShapeType.ScrollText; }
        }

        public int Unk1C { get; set; }

        public int Unk20 { get; set; }

        public int Unk24 { get; set; }

        public int Unk28 { get; set; }

        public short Unk2C { get; set; }

        public ScrollText() {
          this.Unk24 = 15;
        }

        internal ScrollText(
            BinaryReaderEx br,
            bool dsr,
            Dictionary<int, string> strings)
            : base(br, dsr, strings) {
          this.Unk1C = br.ReadInt32();
          this.Unk20 = br.ReadInt32();
          this.Unk24 = br.ReadInt32();
          this.Unk28 = br.ReadInt32();
          this.Unk2C = br.ReadInt16();
        }

        internal override void WriteSpecific(
            BinaryWriterEx bw,
            Dictionary<string, int> stringOffsets) {
          base.WriteSpecific(bw, stringOffsets);
          bw.WriteInt32(this.Unk1C);
          bw.WriteInt32(this.Unk20);
          bw.WriteInt32(this.Unk24);
          bw.WriteInt32(this.Unk28);
          bw.WriteInt16(this.Unk2C);
        }
      }

      [Flags]
      public enum SpriteFlags : ushort {
        None = 0,
        RotateCW = 16,        // 0x0010
        Rotate180 = 32,       // 0x0020
        FlipVertical = 64,    // 0x0040
        FlipHorizontal = 128, // 0x0080
        Alpha = 256,          // 0x0100
        Overlay = 512,        // 0x0200
      }

      public class Sprite : DRB.Shape {
        public override DRB.ShapeType Type {
          get { return DRB.ShapeType.Sprite; }
        }

        public short TexLeftEdge { get; set; }

        public short TexTopEdge { get; set; }

        public short TexRightEdge { get; set; }

        public short TexBottomEdge { get; set; }

        public short TextureIndex { get; set; }

        public DRB.Shape.SpriteFlags Flags { get; set; }

        public int PaletteColor { get; set; }

        public Color CustomColor { get; set; }

        public Sprite() {
          this.Flags = DRB.Shape.SpriteFlags.Alpha;
          this.CustomColor = Color.White;
        }

        internal Sprite(BinaryReaderEx br, bool dsr)
            : base(br, dsr) {
          this.TexLeftEdge = br.ReadInt16();
          this.TexTopEdge = br.ReadInt16();
          this.TexRightEdge = br.ReadInt16();
          this.TexBottomEdge = br.ReadInt16();
          this.TextureIndex = br.ReadInt16();
          this.Flags = (DRB.Shape.SpriteFlags) br.ReadUInt16();
          this.PaletteColor = br.ReadInt32();
          this.CustomColor = DRB.ReadABGR(br);
        }

        internal override void WriteSpecific(
            BinaryWriterEx bw,
            Dictionary<string, int> stringOffsets) {
          bw.WriteInt16(this.TexLeftEdge);
          bw.WriteInt16(this.TexTopEdge);
          bw.WriteInt16(this.TexRightEdge);
          bw.WriteInt16(this.TexBottomEdge);
          bw.WriteInt16(this.TextureIndex);
          bw.WriteUInt16((ushort) this.Flags);
          bw.WriteInt32(this.PaletteColor);
          DRB.WriteABGR(bw, this.CustomColor);
        }
      }

      [Flags]
      public enum AlignFlags : byte {
        TopLeft = 0,
        Right = 1,
        CenterHorizontal = 2,
        Bottom = 4,
        CenterVertical = 8,
      }

      public enum TxtType : byte {
        Literal,
        FMG,
        Dynamic,
      }

      public class Text : DRB.Shape.TextBase {
        public override DRB.ShapeType Type {
          get { return DRB.ShapeType.Text; }
        }

        public int Unk1C { get; set; }

        public short Unk20 { get; set; }

        public Text() {}

        internal Text(
            BinaryReaderEx br,
            bool dsr,
            Dictionary<int, string> strings)
            : base(br, dsr, strings) {
          this.Unk1C = br.ReadInt32();
          this.Unk20 = br.ReadInt16();
        }

        internal override void WriteSpecific(
            BinaryWriterEx bw,
            Dictionary<string, int> stringOffsets) {
          base.WriteSpecific(bw, stringOffsets);
          bw.WriteInt32(this.Unk1C);
          bw.WriteInt16(this.Unk20);
        }
      }

      public abstract class TextBase : DRB.Shape {
        public byte Unk00 { get; set; }

        public byte Unk01 { get; set; }

        public short LineSpacing { get; set; }

        public int PaletteColor { get; set; }

        public Color CustomColor { get; set; }

        public short FontSize { get; set; }

        public DRB.Shape.AlignFlags Alignment { get; set; }

        public DRB.Shape.TxtType TextType { get; set; }

        public int Unk10 { get; set; }

        public int CharLength { get; set; }

        public string TextLiteral { get; set; }

        public int TextID { get; set; }

        internal TextBase() {
          this.Unk01 = (byte) 1;
          this.TextType = DRB.Shape.TxtType.FMG;
          this.Unk10 = 28;
          this.TextID = -1;
        }

        internal TextBase(
            BinaryReaderEx br,
            bool dsr,
            Dictionary<int, string> strings)
            : base(br, dsr) {
          this.Unk00 = br.ReadByte();
          this.Unk01 = br.ReadByte();
          this.LineSpacing = br.ReadInt16();
          this.PaletteColor = br.ReadInt32();
          this.CustomColor = DRB.ReadABGR(br);
          this.FontSize = br.ReadInt16();
          this.Alignment = (DRB.Shape.AlignFlags) br.ReadByte();
          this.TextType = br.ReadEnum8<DRB.Shape.TxtType>();
          this.Unk10 = br.ReadInt32();
          if (this.TextType == DRB.Shape.TxtType.Literal) {
            int index = br.ReadInt32();
            this.TextLiteral = strings[index];
            this.CharLength = -1;
            this.TextID = -1;
          } else if (this.TextType == DRB.Shape.TxtType.FMG) {
            this.CharLength = br.ReadInt32();
            this.TextID = br.ReadInt32();
            this.TextLiteral = (string) null;
          } else {
            if (this.TextType != DRB.Shape.TxtType.Dynamic)
              return;
            this.CharLength = br.ReadInt32();
            this.TextLiteral = (string) null;
            this.TextID = -1;
          }
        }

        internal override void WriteSpecific(
            BinaryWriterEx bw,
            Dictionary<string, int> stringOffsets) {
          bw.WriteByte(this.Unk00);
          bw.WriteByte(this.Unk01);
          bw.WriteInt16(this.LineSpacing);
          bw.WriteInt32(this.PaletteColor);
          DRB.WriteABGR(bw, this.CustomColor);
          bw.WriteInt16(this.FontSize);
          bw.WriteByte((byte) this.Alignment);
          bw.WriteByte((byte) this.TextType);
          bw.WriteInt32(this.Unk10);
          if (this.TextType == DRB.Shape.TxtType.Literal)
            bw.WriteInt32(stringOffsets[this.TextLiteral]);
          else if (this.TextType == DRB.Shape.TxtType.FMG) {
            bw.WriteInt32(this.CharLength);
            bw.WriteInt32(this.TextID);
          } else {
            if (this.TextType != DRB.Shape.TxtType.Dynamic)
              return;
            bw.WriteInt32(this.CharLength);
          }
        }
      }
    }

    public enum ControlType {
      DmeCtrlScrollText,
      FrpgMenuDlgObjContentsHelpItem,
      Static,
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public abstract class Control {
      public abstract DRB.ControlType Type { get; }

      internal static DRB.Control Read(
          BinaryReaderEx br,
          Dictionary<int, string> strings,
          long ctprStart) {
        int index = br.ReadInt32();
        int num = br.ReadInt32();
        string str = strings[index];
        br.StepIn(ctprStart + (long) num);
        DRB.Control control;
        if (str == "DmeCtrlScrollText")
          control = (DRB.Control) new DRB.Control.ScrollTextDummy(br);
        else if (str == "FrpgMenuDlgObjContentsHelpItem") {
          control = (DRB.Control) new DRB.Control.HelpItem(br);
        } else {
          if (!(str == "Static"))
            throw new InvalidDataException("Unknown control type: " + str);
          control = (DRB.Control) new DRB.Control.Static(br);
        }
        br.StepOut();
        return control;
      }

      internal abstract void WriteData(BinaryWriterEx bw);

      internal void WriteHeader(
          BinaryWriterEx bw,
          Dictionary<string, int> stringOffsets,
          Queue<int> ctprOffsets) {
        bw.WriteInt32(stringOffsets[this.Type.ToString()]);
        bw.WriteInt32(ctprOffsets.Dequeue());
      }

      public override string ToString() {
        return string.Format("{0}", (object) this.Type);
      }

      public class ScrollTextDummy : DRB.Control {
        public override DRB.ControlType Type {
          get { return DRB.ControlType.DmeCtrlScrollText; }
        }

        public int Unk00 { get; set; }

        public ScrollTextDummy() {}

        internal ScrollTextDummy(BinaryReaderEx br) {
          this.Unk00 = br.ReadInt32();
        }

        internal override void WriteData(BinaryWriterEx bw) {
          bw.WriteInt32(this.Unk00);
        }
      }

      public class HelpItem : DRB.Control {
        public override DRB.ControlType Type {
          get { return DRB.ControlType.FrpgMenuDlgObjContentsHelpItem; }
        }

        public int Unk00 { get; set; }

        public int Unk04 { get; set; }

        public int Unk08 { get; set; }

        public int Unk0C { get; set; }

        public int Unk10 { get; set; }

        public int Unk14 { get; set; }

        public int TextID { get; set; }

        public HelpItem() {
          this.TextID = -1;
        }

        internal HelpItem(BinaryReaderEx br) {
          this.Unk00 = br.ReadInt32();
          this.Unk04 = br.ReadInt32();
          this.Unk08 = br.ReadInt32();
          this.Unk0C = br.ReadInt32();
          this.Unk10 = br.ReadInt32();
          this.Unk14 = br.ReadInt32();
          this.TextID = br.ReadInt32();
        }

        internal override void WriteData(BinaryWriterEx bw) {
          bw.WriteInt32(this.Unk00);
          bw.WriteInt32(this.Unk04);
          bw.WriteInt32(this.Unk08);
          bw.WriteInt32(this.Unk0C);
          bw.WriteInt32(this.Unk10);
          bw.WriteInt32(this.Unk14);
          bw.WriteInt32(this.TextID);
        }
      }

      public class Static : DRB.Control {
        public override DRB.ControlType Type {
          get { return DRB.ControlType.Static; }
        }

        public int Unk00 { get; set; }

        public Static() {}

        internal Static(BinaryReaderEx br) {
          this.Unk00 = br.ReadInt32();
        }

        internal override void WriteData(BinaryWriterEx bw) {
          bw.WriteInt32(this.Unk00);
        }
      }
    }

    public class Anik {
      public string Name { get; set; }

      public int Unk04 { get; set; }

      public byte Unk08 { get; set; }

      public byte Unk09 { get; set; }

      public short Unk0A { get; set; }

      public int IntpOffset { get; set; }

      public int AnipOffset { get; set; }

      public int Unk14 { get; set; }

      public int Unk18 { get; set; }

      public int Unk1C { get; set; }

      internal Anik(BinaryReaderEx br, Dictionary<int, string> strings) {
        int index = br.ReadInt32();
        this.Unk04 = br.ReadInt32();
        this.Unk08 = br.ReadByte();
        this.Unk09 = br.ReadByte();
        this.Unk0A = br.ReadInt16();
        this.IntpOffset = br.ReadInt32();
        this.AnipOffset = br.ReadInt32();
        this.Unk14 = br.ReadInt32();
        this.Unk18 = br.ReadInt32();
        this.Unk1C = br.ReadInt32();
        this.Name = strings[index];
      }

      internal void Write(
          BinaryWriterEx bw,
          Dictionary<string, int> stringOffsets) {
        bw.WriteInt32(stringOffsets[this.Name]);
        bw.WriteInt32(this.Unk04);
        bw.WriteByte(this.Unk08);
        bw.WriteByte(this.Unk09);
        bw.WriteInt16(this.Unk0A);
        bw.WriteInt32(this.IntpOffset);
        bw.WriteInt32(this.AnipOffset);
        bw.WriteInt32(this.Unk14);
        bw.WriteInt32(this.Unk18);
        bw.WriteInt32(this.Unk1C);
      }

      public override string ToString() {
        return this.Name ?? "";
      }
    }

    public class Anio {
      public int Unk00 { get; set; }

      public List<DRB.Anik> Aniks { get; set; }

      public int Unk0C { get; set; }

      public Anio() {
        this.Aniks = new List<DRB.Anik>();
      }

      internal Anio(BinaryReaderEx br, Dictionary<int, DRB.Anik> aniks) {
        this.Unk00 = br.ReadInt32();
        int capacity = br.ReadInt32();
        int num = br.ReadInt32();
        this.Unk0C = br.ReadInt32();
        this.Aniks = new List<DRB.Anik>(capacity);
        for (int index = 0; index < capacity; ++index) {
          int key = num + 32 * index;
          this.Aniks.Add(aniks[key]);
          aniks.Remove(key);
        }
      }

      internal void Write(BinaryWriterEx bw, Queue<int> anikOffsets) {
        bw.WriteInt32(this.Unk00);
        bw.WriteInt32(this.Aniks.Count);
        bw.WriteInt32(anikOffsets.Dequeue());
        bw.WriteInt32(this.Unk0C);
      }

      public override string ToString() {
        return string.Format("Anio[{0}]", (object) this.Aniks.Count);
      }
    }

    public class Anim {
      public string Name { get; set; }

      public List<DRB.Anio> Anios { get; set; }

      public int Unk0C { get; set; }

      public int Unk10 { get; set; }

      public int Unk14 { get; set; }

      public int Unk18 { get; set; }

      public int Unk1C { get; set; }

      public int Unk20 { get; set; }

      public int Unk24 { get; set; }

      public int Unk28 { get; set; }

      public int Unk2C { get; set; }

      public Anim() {
        this.Name = "";
        this.Anios = new List<DRB.Anio>();
        this.Unk10 = 4;
        this.Unk14 = 4;
        this.Unk18 = 4;
        this.Unk1C = 1;
      }

      internal Anim(
          BinaryReaderEx br,
          Dictionary<int, string> strings,
          Dictionary<int, DRB.Anio> anios) {
        int index1 = br.ReadInt32();
        int capacity = br.ReadInt32();
        int num = br.ReadInt32();
        this.Unk0C = br.ReadInt32();
        this.Unk10 = br.ReadInt32();
        this.Unk14 = br.ReadInt32();
        this.Unk18 = br.ReadInt32();
        this.Unk1C = br.ReadInt32();
        this.Unk20 = br.ReadInt32();
        this.Unk24 = br.ReadInt32();
        this.Unk28 = br.ReadInt32();
        this.Unk2C = br.ReadInt32();
        this.Name = strings[index1];
        this.Anios = new List<DRB.Anio>(capacity);
        for (int index2 = 0; index2 < capacity; ++index2) {
          int key = num + 16 * index2;
          this.Anios.Add(anios[key]);
          anios.Remove(key);
        }
      }

      internal void Write(
          BinaryWriterEx bw,
          Dictionary<string, int> stringOffsets,
          Queue<int> anioOffsets) {
        bw.WriteInt32(stringOffsets[this.Name]);
        bw.WriteInt32(this.Anios.Count);
        bw.WriteInt32(anioOffsets.Dequeue());
        bw.WriteInt32(this.Unk0C);
        bw.WriteInt32(this.Unk10);
        bw.WriteInt32(this.Unk14);
        bw.WriteInt32(this.Unk18);
        bw.WriteInt32(this.Unk1C);
        bw.WriteInt32(this.Unk20);
        bw.WriteInt32(this.Unk24);
        bw.WriteInt32(this.Unk28);
        bw.WriteInt32(this.Unk2C);
      }

      public override string ToString() {
        return string.Format("{0}[{1}]",
                             (object) this.Name,
                             (object) this.Anios.Count);
      }
    }

    public class Scdk {
      public string Name { get; set; }

      public int Unk04 { get; set; }

      public int Unk08 { get; set; }

      public int Unk0C { get; set; }

      public int Unk14 { get; set; }

      public int Unk18 { get; set; }

      public int Unk1C { get; set; }

      public int AnimIndex { get; set; }

      public int Scdp04 { get; set; }

      public Scdk() {
        this.Name = "";
        this.Unk08 = 1;
      }

      internal Scdk(
          BinaryReaderEx br,
          Dictionary<int, string> strings,
          long scdpStart) {
        int index = br.ReadInt32();
        this.Unk04 = br.ReadInt32();
        this.Unk08 = br.ReadInt32();
        this.Unk0C = br.ReadInt32();
        int num = br.ReadInt32();
        this.Unk14 = br.ReadInt32();
        this.Unk18 = br.ReadInt32();
        this.Unk1C = br.ReadInt32();
        this.Name = strings[index];
        br.StepIn(scdpStart + (long) num);
        this.AnimIndex = br.ReadInt32();
        this.Scdp04 = br.ReadInt32();
        br.StepOut();
      }

      internal void WriteSCDP(BinaryWriterEx bw) {
        bw.WriteInt32(this.AnimIndex);
        bw.WriteInt32(this.Scdp04);
      }

      internal void Write(
          BinaryWriterEx bw,
          Dictionary<string, int> stringOffsets,
          Queue<int> scdpOffsets) {
        bw.WriteInt32(stringOffsets[this.Name]);
        bw.WriteInt32(this.Unk04);
        bw.WriteInt32(this.Unk08);
        bw.WriteInt32(this.Unk0C);
        bw.WriteInt32(scdpOffsets.Dequeue());
        bw.WriteInt32(this.Unk14);
        bw.WriteInt32(this.Unk18);
        bw.WriteInt32(this.Unk1C);
      }

      public override string ToString() {
        return this.Name ?? "";
      }
    }

    public class Scdo {
      public string Name { get; set; }

      public List<DRB.Scdk> Scdks { get; set; }

      public int Unk0C { get; set; }

      public Scdo() {
        this.Name = "";
        this.Scdks = new List<DRB.Scdk>();
      }

      internal Scdo(
          BinaryReaderEx br,
          Dictionary<int, string> strings,
          Dictionary<int, DRB.Scdk> scdks) {
        int index1 = br.ReadInt32();
        int capacity = br.ReadInt32();
        int num = br.ReadInt32();
        this.Unk0C = br.ReadInt32();
        this.Name = strings[index1];
        this.Scdks = new List<DRB.Scdk>(capacity);
        for (int index2 = 0; index2 < capacity; ++index2) {
          int key = num + 32 * index2;
          this.Scdks.Add(scdks[key]);
          scdks.Remove(key);
        }
      }

      internal void Write(
          BinaryWriterEx bw,
          Dictionary<string, int> stringOffsets,
          Queue<int> scdkOffsets) {
        bw.WriteInt32(stringOffsets[this.Name]);
        bw.WriteInt32(this.Scdks.Count);
        bw.WriteInt32(scdkOffsets.Dequeue());
        bw.WriteInt32(this.Unk0C);
      }

      public override string ToString() {
        return string.Format("{0}[{1}]",
                             (object) this.Name,
                             (object) this.Scdks.Count);
      }
    }

    public class Scdl {
      public string Name { get; set; }

      public List<DRB.Scdo> Scdos { get; set; }

      public int Unk0C { get; set; }

      public Scdl() {
        this.Name = "";
        this.Scdos = new List<DRB.Scdo>();
      }

      internal Scdl(
          BinaryReaderEx br,
          Dictionary<int, string> strings,
          Dictionary<int, DRB.Scdo> scdos) {
        int index1 = br.ReadInt32();
        int capacity = br.ReadInt32();
        int num = br.ReadInt32();
        this.Unk0C = br.ReadInt32();
        this.Name = strings[index1];
        this.Scdos = new List<DRB.Scdo>(capacity);
        for (int index2 = 0; index2 < capacity; ++index2) {
          int key = num + 16 * index2;
          this.Scdos.Add(scdos[key]);
          scdos.Remove(key);
        }
      }

      internal void Write(
          BinaryWriterEx bw,
          Dictionary<string, int> stringOffsets,
          Queue<int> scdoOffsets) {
        bw.WriteInt32(stringOffsets[this.Name]);
        bw.WriteInt32(this.Scdos.Count);
        bw.WriteInt32(scdoOffsets.Dequeue());
        bw.WriteInt32(this.Unk0C);
      }

      public override string ToString() {
        return string.Format("{0}[{1}]",
                             (object) this.Name,
                             (object) this.Scdos.Count);
      }
    }

    public class Dlgo {
      public string Name { get; set; }

      [Browsable(false)] public DRB.Shape Shape { get; set; }

      public DRB.Control Control { get; set; }

      public int Unk0C { get; set; }

      public int Unk10 { get; set; }

      public int Unk14 { get; set; }

      public int Unk18 { get; set; }

      public int Unk1C { get; set; }

      public Dlgo() {
        this.Name = "";
        this.Shape = (DRB.Shape) new DRB.Shape.Null();
        this.Control = (DRB.Control) new DRB.Control.Static();
      }

      public Dlgo(string name, DRB.Shape shape, DRB.Control control) {
        this.Name = name;
        this.Shape = shape;
        this.Control = control;
      }

      internal Dlgo(
          BinaryReaderEx br,
          Dictionary<int, string> strings,
          Dictionary<int, DRB.Shape> shapes,
          Dictionary<int, DRB.Control> controls) {
        int index = br.ReadInt32();
        int key1 = br.ReadInt32();
        int key2 = br.ReadInt32();
        this.Unk0C = br.ReadInt32();
        this.Unk10 = br.ReadInt32();
        this.Unk14 = br.ReadInt32();
        this.Unk18 = br.ReadInt32();
        this.Unk1C = br.ReadInt32();
        this.Name = strings[index];
        this.Shape = shapes[key1];
        shapes.Remove(key1);
        this.Control = controls[key2];
        controls.Remove(key2);
      }

      internal void Write(
          BinaryWriterEx bw,
          Dictionary<string, int> stringOffsets,
          Queue<int> shapOffsets,
          Queue<int> ctrlOffsets) {
        bw.WriteInt32(stringOffsets[this.Name]);
        bw.WriteInt32(shapOffsets.Dequeue());
        bw.WriteInt32(ctrlOffsets.Dequeue());
        bw.WriteInt32(this.Unk0C);
        bw.WriteInt32(this.Unk10);
        bw.WriteInt32(this.Unk14);
        bw.WriteInt32(this.Unk18);
        bw.WriteInt32(this.Unk1C);
      }

      public override string ToString() {
        return string.Format("{0} ({1} {2})",
                             (object) this.Name,
                             (object) this.Control.Type,
                             (object) this.Shape.Type);
      }
    }

    public class Dlg : DRB.Dlgo {
      [Browsable(false)] public List<DRB.Dlgo> Dlgos { get; set; }

      public short LeftEdge { get; set; }

      public short TopEdge { get; set; }

      public short RightEdge { get; set; }

      public short BottomEdge { get; set; }

      public short[] Unk30 { get; private set; }

      public short Unk3A { get; set; }

      public int Unk3C { get; set; }

      public Dlg() {
        this.Dlgos = new List<DRB.Dlgo>();
        this.Unk30 = new short[5] {
            (short) -1,
            (short) -1,
            (short) -1,
            (short) -1,
            (short) -1
        };
      }

      public Dlg(string name, DRB.Shape shape, DRB.Control control)
          : base(name, shape, control) {
        this.Dlgos = new List<DRB.Dlgo>();
        this.Unk30 = new short[5] {
            (short) -1,
            (short) -1,
            (short) -1,
            (short) -1,
            (short) -1
        };
      }

      internal Dlg(
          BinaryReaderEx br,
          Dictionary<int, string> strings,
          Dictionary<int, DRB.Shape> shapes,
          Dictionary<int, DRB.Control> controls,
          Dictionary<int, DRB.Dlgo> dlgos)
          : base(br, strings, shapes, controls) {
        int capacity = br.ReadInt32();
        int num = br.ReadInt32();
        this.LeftEdge = br.ReadInt16();
        this.TopEdge = br.ReadInt16();
        this.RightEdge = br.ReadInt16();
        this.BottomEdge = br.ReadInt16();
        this.Unk30 = br.ReadInt16s(5);
        this.Unk3A = br.ReadInt16();
        this.Unk3C = br.ReadInt32();
        this.Dlgos = new List<DRB.Dlgo>(capacity);
        for (int index = 0; index < capacity; ++index) {
          int key = num + 32 * index;
          this.Dlgos.Add(dlgos[key]);
          dlgos.Remove(key);
        }
      }

      internal void Write(
          BinaryWriterEx bw,
          Dictionary<string, int> stringOffsets,
          Queue<int> shapOffsets,
          Queue<int> ctrlOffsets,
          Queue<int> dlgoOffsets) {
        this.Write(bw, stringOffsets, shapOffsets, ctrlOffsets);
        bw.WriteInt32(this.Dlgos.Count);
        bw.WriteInt32(dlgoOffsets.Dequeue());
        bw.WriteInt16(this.LeftEdge);
        bw.WriteInt16(this.TopEdge);
        bw.WriteInt16(this.RightEdge);
        bw.WriteInt16(this.BottomEdge);
        bw.WriteInt16s((IList<short>) this.Unk30);
        bw.WriteInt16(this.Unk3A);
        bw.WriteInt32(this.Unk3C);
      }

      public DRB.Dlgo this[string name] {
        get {
          return this.Dlgos.Find(
              (Predicate<DRB.Dlgo>) (dlgo => dlgo.Name == name));
        }
      }

      public override string ToString() {
        return string.Format("{0} ({1} {2} [{3}])",
                             (object) this.Name,
                             (object) this.Control.Type,
                             (object) this.Shape.Type,
                             (object) this.Dlgos.Count);
      }
    }
  }
}