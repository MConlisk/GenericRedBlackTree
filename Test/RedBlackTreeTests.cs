using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

using DataStructures;
using DataStructures.Balancers;
using DataStructures.Models;
using DataStructures.Nodes;
using DataStructures.Traversers;

using System;

namespace RedBlackTree.Tests
{
	[TestFixture]
	public class UniversalTreeTests
	{

		[Test, Order(1)]
		public void GetValue_ExistingElement_ReturnsCorrectValue()
		{
			// Arrange
			var rbtBalancer = new RedBlackBalancer<int, string>();
			var rbtTraverser = new RedBlackInOrderTraverser<int, string>(rbtBalancer);
			var rbtModel = new RedBlackTreeModel<int, string>(rbtTraverser);
			var universalTree = new UniversalTree<int, string, RedBlackNode<int, string>>(rbtModel);
			int testNum = 5;

			// Act
			for (int i = 0; i < testNum * 2; i++)
			{
				universalTree.Insert(i, "TestValue" + i);
				Console.WriteLine($"insert item {i} with value of TestValue{i}");
			}

			// Assert
			Assert.That(universalTree.GetValue(testNum), Is.EqualTo("TestValue" + testNum));
		}

		// Add more tests as needed for your specific use cases.
	}

	[MemoryDiagnoser]
	public class RedBlackTreeBenchmark
	{
		private RedBlackBalancer<int, string> _rbtBalancer;
		private RedBlackInOrderTraverser<int, string> _rbtTraverser;
		private RedBlackTreeModel<int, string> _rbtModel;

		private UniversalTree<int, string, RedBlackNode<int, string>> _redBlackTree;


		[Params(1000, 10000, 100000)]
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
	public class RedBlackTreeTests
	{
		[Test]
		public void BenchmarkRedBlackTreeOperations()
		{
			_ = BenchmarkRunner.Run<RedBlackTreeBenchmark>();
		}
	}

}