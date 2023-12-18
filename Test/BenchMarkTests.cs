using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

using DataStructures;
using DataStructures.Balancers;
using DataStructures.Models;
using DataStructures.Nodes;
using DataStructures.Traversers;
using DataStructures.Interfaces;

using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Diagnostics.Tracing;

namespace RedBlackTree.Tests
{
	[TestFixture]
	public class NUnitTests
	{

		[Test, Order(1)]
		public void GetValue_ExistingElement_ReturnsCorrectValue()
		{
			Stopwatch sw = new();

			// Arrange
			var rbtBalancer = new RedBlackBalancer<int, string>();
			var rbtTraverser = new RedBlackInOrderTraverser<int, string>(rbtBalancer);
			var rbtModel = new RedBlackTreeModel<int, string>(rbtTraverser);
			var universalTree = new UniversalTree<int, string, RedBlackNode<int, string>>(rbtModel);
			int testNum = 10;

			sw.Start();
			// Act
			for (int i = 0; i < testNum * 2; i++)
			{
				universalTree.Insert(i, "TestValue" + i);
			}
			TimeSpan fillTime = new(sw.ElapsedTicks);
			sw.Stop();

			sw.Reset();

			//sw.Start();
			//universalTree.MapTree();
			//TimeSpan mapTime = new(sw.ElapsedTicks);
			//sw.Stop();

			Console.WriteLine($"Durations");
			Console.WriteLine($"Fill took: {fillTime.TotalMilliseconds} milliseconds.");
			//Console.WriteLine($"Map took: {mapTime.TotalMilliseconds} milliseconds.");

			// Assert
			Assert.That(universalTree.GetValue(testNum), Is.EqualTo("TestValue" + testNum));
		}

	}

	[MemoryDiagnoser]
	public class BenchMark
	{
		private RedBlackBalancer<int, string> _rbtBalancer;
		private RedBlackInOrderTraverser<int, string> _rbtTraverser;
		private RedBlackTreeModel<int, string> _rbtModel;

		private UniversalTree<int, string, RedBlackNode<int, string>> _redBlackTree;
		

		[Params(1, 10, 100)]
		public int N; // Number of elements to insert/remove


		[GlobalSetup]
		public void Setup()
		{
			// Initialize the Red-Black Tree before each benchmark iteration
			_rbtBalancer = new RedBlackBalancer<int, string>();
			_rbtTraverser = new RedBlackInOrderTraverser<int, string>(_rbtBalancer);
			_rbtModel = new RedBlackTreeModel<int, string>(_rbtTraverser);

			_redBlackTree = new UniversalTree<int, string, RedBlackNode<int, string>>(_rbtModel);
		}

		[Benchmark]
		public void InsertBenchmark()
		{
			// Perform N insertions
			for (int i = 0; i < N; i++)
			{
				_redBlackTree.Insert(i, i.ToString());
			}
		}

		[Benchmark]
		public void RemoveBenchmark()
		{
			// Perform N removals
			for (int i = 0; i < N; i++)
			{
				_redBlackTree.Remove(i);
			}
		}
	}

	[TestFixture]
	public class BenchMarkTests
	{
		[Test]
		public void BenchmarkRedBlackTreeOperations()
		{
			_ = BenchmarkRunner.Run<BenchMark>();
		}
	}

}