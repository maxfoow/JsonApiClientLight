using System;

namespace Orion.JsonApiClientLight {
	public class RetryPolicy {
		public static readonly RetryPolicy NoRetry = new RetryPolicy { RetryCount = 0 };

		public RetryPolicy() {
			RetryCount = 3;
			Delay = TimeSpan.FromSeconds(2);
		}

		public RetryPolicy(int retryCount, TimeSpan delay) {
			RetryCount = retryCount;
			Delay = delay;
		}

		public int RetryCount { get; set; }
		public TimeSpan Delay { get; set; }

		public virtual TimeSpan Next(int currentRetry) {
			return TimeSpan.FromSeconds(Delay.TotalSeconds * Math.Pow(currentRetry, 2));
		}

		public void Disable() {
			RetryCount = NoRetry.RetryCount;
			Delay = NoRetry.Delay;
		}

		public bool IsDisabled() {
			return Equals(NoRetry);
		}

		#region Equality members
		protected bool Equals(RetryPolicy other) {
			return RetryCount == other.RetryCount && Delay.Equals(other.Delay);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((RetryPolicy) obj);
		}

		public override int GetHashCode() {
			unchecked {
				return (RetryCount * 397) ^ Delay.GetHashCode();
			}
		}
		#endregion

	}
}