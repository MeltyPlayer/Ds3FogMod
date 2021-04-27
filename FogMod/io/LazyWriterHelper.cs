using SoulsFormats;

namespace FogMod.io {
  public class LazyWriterHelper {
    public BinaryReaderEx Reader { get; private set; }
    public long Offset { get; private set; }
    public int Length { get; private set; }

    public void Update(BinaryReaderEx reader, long offset, int length) {
      this.CanStream = true;
      this.Reader = reader;
      this.Offset = offset;
      this.Length = length;
    }

    public bool CanStream { get; private set; } = false;
    public void Touch() => this.CanStream = false;

    public bool StreamTo(BinaryWriterEx writer) {
      if (!this.CanStream || this.Offset == 0) {
        return false;
      }

      this.Reader.Position = this.Offset;
      writer.WriteBytes(this.Reader.ReadBytes(this.Length));
      return true;
    }
  }
}
