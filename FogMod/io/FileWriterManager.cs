using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FogMod.io {
  public interface IFileWriterManager {
    IByteFileWriter OpenBytes(string fullName);
    ITextFileWriter OpenText(string fullName);

    IList<IWrittenFile> Writers { get; }
  }

  public interface IFileWriter {
    IWrittenFile File { get; }
    void Close();
  }

  public interface IByteFileWriter : IFileWriter {
  }

  public interface ITextFileWriter : IFileWriter {
    void WriteLine(string line);
  }

  public interface IWrittenFile {
    string FullName { get; }

    bool IsText { get; }
    IList<string> GetLines();
    IList<byte> GetBytes();
  }

  /*public class FileWriterManager : IFileWriterManager {
    public static IFileWriterManager INSTANCE = new FileWriterManager();

    public IByteFileWriter OpenBytes(string fullName) {}

    public ITextFileWriter OpenText(string fullName) {
      var writer = new TextFileWriter(fullName);
    }

    public IList<IWrittenFile> Writers { get; } = new List<IWrittenFile>();

    private class ByteFileWriter : IByteFileWriter {


      public ByteFileWriter(string fullName) {}

      public IWrittenFile File { get; }

      private class ByteFile : IWrittenFile {
        public ByteFile(string fullName) {
          this.FullName = fullName;
        }

        public string FullName { get; }
        public bool IsText => false;

        public IList<byte> GetBytes() => File.ReadAllBytes(this.FullName);
      }
    }

    private class TextFileWriter : ITextFileWriter {}
  }*/
}