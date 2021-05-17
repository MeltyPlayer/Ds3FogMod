using System.Collections.Generic;
using System.IO;

namespace FogMod.io {
  public interface IIoObject {
    string Name { get; }
    string FullName { get; }

    IDirectory GetParent();
    bool Exists { get; }
  }

  public interface IDirectory : IIoObject {
    IEnumerable<IDirectory> GetSubdirs();
    IDirectory GetSubdir(string relativePath, bool create = false);

    IEnumerable<IFile> GetFiles();
    IEnumerable<IFile> GetFiles(string searchPattern);

    IFile GetFile(string relativePath);
  }

  public interface IFile : IIoObject {
    StreamReader ReadAsText();
    byte[] SkimAllBytes();
  }
}