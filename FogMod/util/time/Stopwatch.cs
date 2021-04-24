using System;

namespace FogMod.util.time {
  public class Stopwatch {
    private enum StopwatchState {
      OFF,
      STARTED,
      STOPPED,
    }

    private DateTime startTimestamp_;
    private DateTime endTimestamp_;

    private StopwatchState state_ = StopwatchState.OFF;

    public bool EnableLogging { get; set; } = true;

    public void Start() {
      this.state_ = StopwatchState.STARTED;
      this.startTimestamp_ = DateTime.Now;
    }

    public TimeSpan Stop() {
      this.state_ = StopwatchState.STOPPED;
      this.endTimestamp_ = DateTime.Now;
      return this.Elapsed;
    }

    public TimeSpan Reset() {
      var elapsed = this.Elapsed;
      this.Start();
      return elapsed;
    }

    public void ResetAndPrint(string prefix) {
      var elapsed = this.Reset();

      if (this.EnableLogging) {
        Console.WriteLine($"{prefix}: {elapsed.TotalSeconds}");
      }
    }

    public TimeSpan Elapsed => this.state_ switch {
        StopwatchState.STARTED => DateTime.Now - this.startTimestamp_,
        StopwatchState.STOPPED => this.endTimestamp_ - this.startTimestamp_,
        _ => throw new NotImplementedException("Stopwatch was not started."),
    };
  }
}