// Decompiled with JetBrains decompiler
// Type: SoulsFormats.SFUtil
// Assembly: FogMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D28026FD-A9AB-45EB-9CE9-27B9E67A6072
// Assembly location: M:\Games\Steam\steamapps\common\DARK SOULS III\Game\mod\FogMod.exe

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

using FogMod.io;

using SharpCompress.Compressors;
using SharpCompress.Compressors.Deflate;

namespace SoulsFormats {
  [ComVisible(true)]
  public static class SFUtil {
    private static readonly Regex timestampRx =
        new Regex("(\\d\\d)(\\w)(\\d+)(\\w)(\\d+)");

    private static byte[] ds2SaveKey =
        SFUtil.ParseHexString(
            "B7 FD 46 3E 4A 9C 11 02 DF 17 39 E5 F3 B2 A5 0F");

    private static byte[] scholarSaveKey =
        SFUtil.ParseHexString(
            "59 9F 9B 69 96 40 A5 52 36 EE 2D 70 83 5E C7 44");

    private static byte[] ds3SaveKey =
        SFUtil.ParseHexString(
            "FD 46 4D 69 5E 69 A3 9A 10 E3 19 A7 AC E8 B7 FA");

    public static byte[] ds3RegulationKey =
        SFEncoding.ASCII.GetBytes("ds3#jn/8_7(rsY9pg55GFN7VFL#+3n/)");

    private static CompressionLevel
        compressionLevel_ = CompressionLevel.Level5;

    public static string GuessExtension(byte[] bytes, bool bigEndian = false) {
      bool flag = false;
      if (DCX.Is(bytes)) {
        flag = true;
        bytes = DCX.Decompress(bytes);
      }
      string str1 = "";
      using (MemoryStream memoryStream = new MemoryStream(bytes)) {
        BinaryReaderEx br =
            new BinaryReaderEx(bigEndian, (Stream) memoryStream);
        string str2 = (string) null;
        if (br.Length >= 4L)
          str2 = br.ReadASCII(4);
        if (str2 == "AISD")
          str1 = ".aisd";
        else if (str2 == "BDF3" || str2 == "BDF4")
          str1 = ".bdt";
        else if (str2 == "BHF3" || str2 == "BHF4")
          str1 = ".bhd";
        else if (str2 == "BND3" || str2 == "BND4")
          str1 = ".bnd";
        else if (str2 == "DDS ")
          str1 = ".dds";
        else if (str2 != null && str2.ToUpper() == "DLSE")
          str1 = ".dlse";
        else if (bigEndian && str2 == "\0BRD" || !bigEndian && str2 == "DRB\0")
          str1 = ".drb";
        else if (str2 == "EDF\0")
          str1 = ".edf";
        else if (str2 == "ELD\0")
          str1 = ".eld";
        else if (str2 == "ENFL")
          str1 = ".entryfilelist";
        else if (str2 != null && str2.ToUpper() == "FSSL")
          str1 = ".esd";
        else if (str2 == "EVD\0")
          str1 = ".evd";
        else if (br.Length >= 3L && br.GetASCII(0L, 3) == "FEV" ||
                 br.Length >= 16L && br.GetASCII(8L, 8) == "FEV FMT ")
          str1 = ".fev";
        else if (br.Length >= 6L && br.GetASCII(0L, 6) == "FLVER\0")
          str1 = ".flver";
        else if (br.Length >= 3L && br.GetASCII(0L, 3) == "FSB")
          str1 = ".fsb";
        else if (br.Length >= 3L && br.GetASCII(0L, 3) == "GFX")
          str1 = ".gfx";
        else if (br.Length >= 25L && br.GetASCII(12L, 14) == "ITLIMITER_INFO")
          str1 = ".itl";
        else if (br.Length >= 4L && br.GetASCII(1L, 3) == "Lua")
          str1 = ".lua";
        else if (checkMsb(br))
          str1 = ".msb";
        else if (br.Length >= 48L && br.GetASCII(44L, 4) == "MTD ")
          str1 = ".mtd";
        else if (str2 == "DFPN")
          str1 = ".nfd";
        else if (checkParam(br))
          str1 = ".param";
        else if (br.Length >= 4L && br.GetASCII(1L, 3) == "PNG")
          str1 = ".png";
        else if (br.Length >= 44L && br.GetASCII(40L, 4) == "SIB ")
          str1 = ".sib";
        else if (str2 == "TAE ")
          str1 = ".tae";
        else if (checkTdf(br))
          str1 = ".tdf";
        else if (str2 == "TPF\0")
          str1 = ".tpf";
        else if (str2 == "#BOM")
          str1 = ".txt";
        else if (br.Length >= 5L && br.GetASCII(0L, 5) == "<?xml")
          str1 = ".xml";
        else if (br.Length >= 12L) {
          if (br.GetByte(0L) == (byte) 0) {
            if (br.GetByte(3L) == (byte) 0) {
              if ((long) br.GetInt32(4L) == br.Length) {
                if (br.GetInt16(10L) == (short) 0)
                  str1 = ".fmg";
              }
            }
          }
        }
      }
      return flag ? str1 + ".dcx" : str1;

      bool checkMsb(BinaryReaderEx br) {
        if (br.Length < 8L)
          return false;
        int int32 = br.GetInt32(4L);
        if (int32 >= 0) {
          if ((long) int32 < br.Length - 1L) {
            try {
              return br.GetASCII((long) int32) == "MODEL_PARAM_ST";
            } catch {
              return false;
            }
          }
        }
        return false;
      }

      bool checkParam(BinaryReaderEx br) {
        return br.Length >= 44L &&
               Regex.IsMatch(br.GetASCII(12L, 32), "^[^\0]+\0 *$");
      }

      bool checkTdf(BinaryReaderEx br) {
        if (br.Length < 4L || br.GetASCII(0L, 1) != "\"")
          return false;
        for (int index = 1; (long) index < br.Length; ++index) {
          if (br.GetASCII((long) index, 1) == "\"")
            return (long) index < br.Length - 2L &&
                   br.GetASCII((long) (index + 1), 2) == "\r\n";
        }
        return false;
      }
    }

    public static byte ReverseBits(byte value) {
      return (byte) (((int) value & 1) << 7 |
                     ((int) value & 2) << 5 |
                     ((int) value & 4) << 3 |
                     ((int) value & 8) << 1 |
                     ((int) value & 16) >> 1 |
                     ((int) value & 32) >> 3 |
                     ((int) value & 64) >> 5 |
                     ((int) value & 128) >> 7);
    }

    public static string Backup(string file, bool overwrite = false) {
      string str = file + ".bak";
      if (overwrite || !File.Exists(str))
        File.Copy(file, str, overwrite);
      return str;
    }

    public static string GetRealExtension(string path) {
      string extension = Path.GetExtension(path);
      if (extension == ".dcx")
        extension = Path.GetExtension(Path.GetFileNameWithoutExtension(path));
      return extension;
    }

    public static string GetRealFileName(string path) {
      string withoutExtension = Path.GetFileNameWithoutExtension(path);
      if (Path.GetExtension(path) == ".dcx")
        withoutExtension = Path.GetFileNameWithoutExtension(withoutExtension);
      return withoutExtension;
    }

    public static BinaryReaderEx GetDecompressedBR(
        BinaryReaderEx br,
        out DCX.Type compression) {
      if (DCX.Is(br))
        return new BinaryReaderEx(false, DCX.Decompress(br, out compression));
      compression = DCX.Type.None;
      return br;
    }

    public static uint FromPathHash(string text) {
      string source = text.ToLowerInvariant().Replace('\\', '/');
      if (!source.StartsWith("/"))
        source = "/" + source;
      return source.Aggregate<char, uint>(0U,
                                          (Func<uint, char, uint>)
                                          ((i, c) => i * 37U + (uint) c));
    }

    public static bool IsPrime(uint candidate) {
      if (candidate < 2U)
        return false;
      if (candidate == 2U)
        return true;
      if (candidate % 2U == 0U)
        return false;
      for (int index = 3;
           (long) (index * index) <= (long) candidate;
           index += 2) {
        if ((long) candidate % (long) index == 0L)
          return false;
      }
      return true;
    }

    public static DateTime BinderTimestampToDate(string timestamp) {
      System.Text.RegularExpressions.Match match =
          SFUtil.timestampRx.Match(timestamp);
      if (!match.Success)
        throw new InvalidDataException("Unrecognized timestamp format.");
      return new DateTime(int.Parse(match.Groups[1].Value) + 2000,
                          (int) match.Groups[2].Value[0] - 65,
                          int.Parse(match.Groups[3].Value),
                          (int) match.Groups[4].Value[0] - 65,
                          int.Parse(match.Groups[5].Value),
                          0);
    }

    public static string DateToBinderTimestamp(DateTime dateTime) {
      int num = dateTime.Year - 2000;
      if (num < 0 || num > 99)
        throw new InvalidDataException(
            "BND timestamp year must be between 2000 and 2099 inclusive.");
      char ch1 = (char) (dateTime.Month + 65);
      int day = dateTime.Day;
      char ch2 = (char) (dateTime.Hour + 65);
      int minute = dateTime.Minute;
      return string
             .Format("{0:D2}{1}{2}{3}{4}",
                     (object) num,
                     (object) ch1,
                     (object) day,
                     (object) ch2,
                     (object) minute)
             .PadRight(8, char.MinValue);
    }

    public static int WriteZlib(
        BinaryWriterEx bw,
        byte formatByte,
        byte[] input) {
      long position = bw.Position;
      bw.WriteByte((byte) 120);
      bw.WriteByte(formatByte);

      var deflateStream =
          new DeflateStream(bw.Stream,
                            CompressionMode.Compress,
                            SFUtil.compressionLevel_);
      deflateStream.FlushMode = FlushType.Finish;
      deflateStream.Write(input, 0, input.Length);
      deflateStream.Flush();

      bw.WriteUInt32(SFUtil.Adler32(input));
      return (int) (bw.Position - position);
    }

    public static byte[] ReadZlib(
        BinaryReaderEx br,
        int compressedSize,
        IFile file = null
    ) {
      IoFile fastFile = null;
      if (file != null) {
        var directory = file.GetParent();
        fastFile = new IoFile(directory.FullName + "\\" + file.Name + ".fst");

        if (fastFile.Exists) {
          return File.ReadAllBytes(fastFile.FullName);
        }
      }

      int num1 = (int) br.AssertByte((byte) 120);
      int num2 =
          (int) br.AssertByte((byte) 1, (byte) 94, (byte) 156, (byte) 218);
      byte[] buffer = br.ReadBytes(compressedSize - 2);
      using (MemoryStream memoryStream1 = new MemoryStream(compressedSize)) {
        using (MemoryStream memoryStream2 = new MemoryStream(buffer)) {
          var deflateStream =
              new DeflateStream((Stream) memoryStream2,
                                CompressionMode.Decompress,
                                SFUtil.compressionLevel_);
          deflateStream.FlushMode = FlushType.Finish;
          deflateStream.CopyTo((Stream) memoryStream1);
          deflateStream.Flush();
        }

        var bytes = memoryStream1.ToArray();

        if (fastFile != null) {
          File.WriteAllBytes(fastFile.FullName, bytes);
        }
        return bytes;
      }
    }

    public static uint Adler32(byte[] data) {
      uint num1 = 1;
      uint num2 = 0;
      foreach (byte num3 in data) {
        num1 = (num1 + (uint) num3) % 65521U;
        num2 = (num2 + num1) % 65521U;
      }
      return num2 << 16 | num1;
    }

    public static List<T> ConcatAll<T>(params IEnumerable<T>[] lists) {
      IEnumerable<T> objs = (IEnumerable<T>) new List<T>();
      foreach (IEnumerable<T> list in lists)
        objs = objs.Concat<T>(list);
      return objs.ToList<T>();
    }

    public static Dictionary<int, T> Dictionize<T>(List<T> items) {
      Dictionary<int, T> dictionary = new Dictionary<int, T>(items.Count);
      for (int index = 0; index < items.Count; ++index)
        dictionary[index] = items[index];
      return dictionary;
    }

    public static byte[] ParseHexString(string str) {
      string[] strArray = str.Split(' ');
      byte[] numArray = new byte[strArray.Length];
      for (int index = 0; index < strArray.Length; ++index)
        numArray[index] = Convert.ToByte(strArray[index], 16);
      return numArray;
    }

    public static byte[] GetDS2SaveKey() {
      return (byte[]) SFUtil.ds2SaveKey.Clone();
    }

    public static byte[] GetScholarSaveKey() {
      return (byte[]) SFUtil.scholarSaveKey.Clone();
    }

    public static byte[] GetDS3SaveKey() {
      return (byte[]) SFUtil.ds3SaveKey.Clone();
    }

    public static byte[] DecryptSL2File(byte[] encrypted, byte[] key) {
      byte[] numArray = new byte[16];
      Buffer.BlockCopy((Array) encrypted, 16, (Array) numArray, 0, 16);
      using (Aes aes = Aes.Create()) {
        aes.Mode = CipherMode.CBC;
        aes.BlockSize = 128;
        aes.Padding = PaddingMode.None;
        aes.Key = key;
        aes.IV = numArray;
        ICryptoTransform decryptor = aes.CreateDecryptor();
        using (MemoryStream memoryStream1 =
            new MemoryStream(encrypted, 32, encrypted.Length - 32)) {
          using (CryptoStream cryptoStream =
              new CryptoStream((Stream) memoryStream1,
                               decryptor,
                               CryptoStreamMode.Read)) {
            using (MemoryStream memoryStream2 = new MemoryStream()) {
              cryptoStream.CopyTo((Stream) memoryStream2);
              return memoryStream2.ToArray();
            }
          }
        }
      }
    }

    public static byte[] EncryptSL2File(byte[] decrypted, byte[] key) {
      using (Aes aes = Aes.Create()) {
        aes.Mode = CipherMode.CBC;
        aes.BlockSize = 128;
        aes.Padding = PaddingMode.None;
        aes.Key = key;
        aes.GenerateIV();
        ICryptoTransform encryptor = aes.CreateEncryptor();
        using (MemoryStream memoryStream1 = new MemoryStream(decrypted)) {
          using (CryptoStream cryptoStream =
              new CryptoStream((Stream) memoryStream1,
                               encryptor,
                               CryptoStreamMode.Read)) {
            using (MemoryStream memoryStream2 = new MemoryStream()) {
              using (MD5 md5 = MD5.Create()) {
                memoryStream2.Write(aes.IV, 0, 16);
                cryptoStream.CopyTo((Stream) memoryStream2);
                byte[] buffer = new byte[memoryStream2.Length + 16L];
                memoryStream2.Position = 0L;
                memoryStream2.Read(buffer, 16, (int) memoryStream2.Length);
                Buffer.BlockCopy(
                    (Array) md5.ComputeHash(buffer, 16, buffer.Length - 16),
                    0,
                    (Array) buffer,
                    0,
                    16);
                return buffer;
              }
            }
          }
        }
      }
    }

    public static BND4 DecryptDS3Regulation(string path) {
      byte[] secret = File.ReadAllBytes(path);
      return SoulsFile<BND4>.Read(
          SFUtil.DecryptByteArray(SFUtil.ds3RegulationKey, secret));
    }

    public static void EncryptDS3Regulation(string path, BND4 bnd) {
      byte[] secret = bnd.Write();
      byte[] bytes = SFUtil.EncryptByteArray(SFUtil.ds3RegulationKey, secret);
      Directory.CreateDirectory(Path.GetDirectoryName(path));
      File.WriteAllBytes(path, bytes);
    }

    public static byte[] EncryptByteArray(byte[] key, byte[] secret) {
      using (MemoryStream memoryStream = new MemoryStream()) {
        using (AesManaged aesManaged = new AesManaged()) {
          aesManaged.Mode = CipherMode.CBC;
          aesManaged.Padding = PaddingMode.PKCS7;
          aesManaged.KeySize = 256;
          aesManaged.BlockSize = 128;
          byte[] iv = aesManaged.IV;
          using (CryptoStream cryptoStream = new CryptoStream(
              (Stream) memoryStream,
              aesManaged.CreateEncryptor(key, iv),
              CryptoStreamMode.Write))
            cryptoStream.Write(secret, 0, secret.Length);
          byte[] array = memoryStream.ToArray();
          byte[] numArray = new byte[iv.Length + array.Length];
          Buffer.BlockCopy((Array) iv, 0, (Array) numArray, 0, iv.Length);
          Buffer.BlockCopy((Array) array,
                           0,
                           (Array) numArray,
                           iv.Length,
                           array.Length);
          return numArray;
        }
      }
    }

    public static byte[] DecryptByteArray(byte[] key, byte[] secret) {
      byte[] rgbIV = new byte[16];
      byte[] buffer = new byte[secret.Length - 16];
      Buffer.BlockCopy((Array) secret, 0, (Array) rgbIV, 0, rgbIV.Length);
      Buffer.BlockCopy(secret,
                       rgbIV.Length,
                       buffer,
                       0,
                       buffer.Length);
      using (MemoryStream memoryStream = new MemoryStream(buffer.Length)) {
        using (AesManaged aesManaged = new AesManaged()) {
          aesManaged.Mode = CipherMode.CBC;
          aesManaged.Padding = PaddingMode.None;
          aesManaged.KeySize = 256;
          aesManaged.BlockSize = 128;
          using (CryptoStream cryptoStream = new CryptoStream(
              (Stream) memoryStream,
              aesManaged.CreateDecryptor(key, rgbIV),
              CryptoStreamMode.Write))
            cryptoStream.Write(buffer, 0, buffer.Length);
          return memoryStream.ToArray();
        }
      }
    }
  }
}