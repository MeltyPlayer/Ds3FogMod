using System.IO;

namespace FogMod.io {
  public class IoFile : IFile {
    private readonly FileInfo impl_;

    public IoFile(FileInfo fileInfo) => this.impl_ = fileInfo;
    public IoFile(string fullName) => this.impl_ = new FileInfo(fullName);

    public string Name => this.impl_.Name;
    public string FullName => this.impl_.FullName;
  }
}
