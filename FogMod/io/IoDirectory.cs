using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FogMod.io {
  public class IoDirectory : IDirectory {
    private readonly DirectoryInfo impl_;


    public static IoDirectory GetCwd()
      => new IoDirectory(Directory.GetCurrentDirectory());


    public IoDirectory(DirectoryInfo directoryInfo)
      => this.impl_ = directoryInfo;

    public IoDirectory(string fullName)
      => this.impl_ = new DirectoryInfo(fullName);


    public string Name => this.impl_.Name;
    public string FullName => this.impl_.FullName;

    public IDirectory GetParent() => new IoDirectory(this.impl_.Parent);
    public bool Exists => this.impl_.Exists;


    public IEnumerable<IDirectory> GetSubdirs()
      => this.impl_.EnumerateDirectories()
             .Select(subdir => new IoDirectory(subdir));

    public IDirectory GetSubdir(string relativePath, bool create = false)
      => this.GetSubdirImpl_(relativePath.Split('/', '\\'), create);

    private IoDirectory GetSubdirImpl_(
        IEnumerable<string> subdirs,
        bool create) {
      var current = this.impl_;

      foreach (var subdir in subdirs) {
        if (subdir == "..") {
          current = current.Parent;
          continue;
        }

        var matches = current.GetDirectories(subdir);

        if (!create || matches.Length == 1) {
          current = matches.Single();
        } else {
          current = current.CreateSubdirectory(subdir);
        }
      }

      return new IoDirectory(current);
    }


    public IEnumerable<IFile> GetFiles()
      => this.impl_.EnumerateFiles().Select(file => new IoFile(file));

    public IFile GetFile(string path) {
      // TODO: Handle subdirectories automatically.
      return new IoFile(this.impl_.GetFiles(path).Single());
    }
  }
}