using System.IO;
using System.Threading.Tasks;

using FogMod.io;
using FogMod.util.time;

using SoulsIds;

using YamlDotNet.Serialization;

namespace FogMod {
  public static class AnnotationManager {
    private static Task<AnnotationData> cache_;

    public static async Task<AnnotationData> Get(
        GameEditor editor
    ) {
      if (AnnotationManager.cache_ != null) {
        return await AnnotationManager.cache_;
      }

      return await (AnnotationManager.cache_ = Task.Run(() => {
                       var stopwatch = new Stopwatch();
                       stopwatch.Start();

                       var fogdistDirectory = new IoDirectory(editor.Spec.GameDir);
                       var fogTxt = fogdistDirectory.GetFile("fog.txt");

                       IDeserializer deserializer =
                           new DeserializerBuilder().Build();

                       AnnotationData ann;
                       using (StreamReader streamReader = fogTxt.ReadAsText())
                         ann = deserializer.Deserialize<AnnotationData>(
                             (TextReader) streamReader);
                       ann.SetGame(editor.Spec.Game);
                       stopwatch.ResetAndPrint("Set up annotations");

                       return ann;
                     }));
    }
  }
}