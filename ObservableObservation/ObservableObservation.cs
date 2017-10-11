using System;
using System.IO;
using System.Reactive.Disposables;

namespace org.knatten.ObservableObservation
{
	public static class ObservableObservation
	{
		private static int _length;
		public static TextWriter Out;

		static ObservableObservation()
		{
			Reset();	
		}

		public static IDisposable Write<T>(this IObservable<T> obs, string name = "<anon>")
		{
			if (name.Length > _length) _length = name.Length;

			var subscription = obs.Subscribe(
				v => Out.WriteLine(Preamble + "'{1}'", name, v),
				e => Out.WriteLine(Preamble + "Error '{1}'", name, e),
				() => Out.WriteLine(Preamble + "Completed", name)
			);

			return Disposable.Create(() =>
			{
				subscription.Dispose();
				Out.WriteLine(Preamble + "Disposed", name);
			});
		}

		private static string Preamble
		{
			get { return "{0,-" + _length + "}: "; }
		}

		public static void Reset()
		{
			_length = 0;
			Out = Console.Out;
		}
	}
}