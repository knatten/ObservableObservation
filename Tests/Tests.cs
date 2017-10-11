using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;
using NUnit.Framework;
using org.knatten.ObservableObservation;

namespace Tests
{
    public class Tests
    {
	    private StringWriter _stringWriter;

	    private string[] Out
	    {
		    get { return Regex.Split(_stringWriter.GetStringBuilder().ToString(), @"\r\n|\r|\n"); }
	    }

	    [SetUp]
	    public void SetUp()
	    {
			_stringWriter = new StringWriter();
			ObservableObservation.Reset();
		    ObservableObservation.Out = _stringWriter;
	    }

	    [Test]
	    public void WritesValuesAndCompletion()
	    {
		    var s = new Subject<int>();
		    s.Write("n");
			s.OnNext(1);
			s.OnNext(2);
			s.OnCompleted();
			Assert.AreEqual("n: '1'", Out[0]);
			Assert.AreEqual("n: '2'", Out[1]);
			Assert.AreEqual("n: Completed", Out[2]);
	    }

	    [Test]
	    public void WritesValuesAndError()
	    {
		    var s = new Subject<int>();
		    s.Write("e");
			s.OnNext(1);
		    var exception = new Exception("Failed!");
		    s.OnError(exception);
			Assert.AreEqual("e: '1'", Out[0]);
			Assert.AreEqual("e: Error '" + exception + "'", Out[1]);
	    }

	    [Test]
	    public void AdjustsLength()
	    {
		    var hi = new Subject<int>();
		    var hello = new Subject<int>();
		    hi.Write("hi");
			hi.OnNext(1);
		    hello.Write("hello");
			hi.OnNext(2);
			hello.OnNext(1);
			Assert.AreEqual("hi: '1'", Out[0]);
			Assert.AreEqual("hi   : '2'", Out[1]);
			Assert.AreEqual("hello: '1'", Out[2]);
	    }

		//TODO test that it disposes it's observable

	    [Test]
	    public void WritesDisposed()
	    {
		    var s = new Subject<int>();
		    using (s.Write("hi"))
		    {
			    
		    }
			Assert.AreEqual("hi: Disposed", Out[0]);
	    }

	    [Test]
	    public void Anonymous()
	    {
		    var s = new Subject<int>();
		    s.Write();
			s.OnCompleted();
			Assert.AreEqual("<anon>: Completed", Out[0]);
	    }

	    [Test]
	    public void DisposesWhatItObserves()
	    {
		    var disposed = false;
		    var s = Observable.Create<int>(obs =>
		    {
			    return () => { disposed = true; };
		    });
		    using (s.Write())
		    {
			    
		    }
			Assert.True(disposed);
	    }
    }
}
