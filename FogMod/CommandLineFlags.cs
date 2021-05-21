namespace FogMod {
  public static class CommandLineFlags {
    public static bool IsVisualStudio { get; private set; }

    public static void Populate(string[] args) {
      foreach (var arg in args) {
        if (arg == "/visualStudio") {
          CommandLineFlags.IsVisualStudio = true;
        }
      }
    }
  }
}
