using System.Diagnostics;
using System.Collections.Generic;
using System;
using DataStructures.Balancers;
using DataStructures.Models;
using DataStructures.Traversers;
using DataStructures;
using DataStructures.Interfaces;
using DataStructures.Nodes;

namespace RedBlackTree.Tests
{
	[TestFixture]
	public class UniversalTreeTests
	{
		[Test, Order(1)]
		public void Insert_SingleElement_TreeContainsElement()
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
			Assert.That(universalTree.Contains(testNum));
		}

		[Test, Order(2)]
		public void Remove_ExistingElement_TreeDoesNotContainElement()
		{
			// Arrange
			var rbtBalancer = new RedBlackBalancer<int, string>();
			var rbtTraverser = new RedBlackInOrderTraverser<int, string>(rbtBalancer);
			var rbtModel = new RedBlackTreeModel<int, string>(rbtTraverser);
			var universalTree = new UniversalTree<int, string, RedBlackNode<int, string>>(rbtModel);

			int testNum = 5;
			// Act
			for(int i = 0; i < testNum * 2; i++)
			{
				universalTree.Insert(i, "TestValue" + i);
				Console.WriteLine($"insert item {i} with value of TestValue{i}");
			}

			universalTree.Remove(testNum);

			// Assert
			Assert.That(!universalTree.Contains(testNum));
		}

		[Test, Order(3)]
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
}