﻿using System.Collections.Generic;

namespace FogMod.io {
  public interface IIoObject {
    string Name { get; }
    string FullName { get; }
  }

  public interface IDirectory : IIoObject {
    IEnumerable<IDirectory> GetSubdirs();
    IDirectory GetSubdir(string relativePath, bool create = false);

    IEnumerable<IFile> GetFiles();
    IFile GetFile(string relativePath);
  }

  public interface IFile : IIoObject {}
}