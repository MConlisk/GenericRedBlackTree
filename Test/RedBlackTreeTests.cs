using NUnit.Framework;
using System.Diagnostics;
using RedBlackTree.Trees;
using System.Collections.Generic;
using System;

namespace RedBlackTree.Tests
{
	[TestFixture]
	public class RedBlackTreeTests
	{
		private GenericRedBlackTree<string> _tree;
		private List<int> _keys;

		[SetUp]
		public void Setup()
		{
			_tree = new GenericRedBlackTree<string>();
			_keys = new List<int>();
		}

		[Test]
		public void Contains_ReturnsTrue_WhenKeyExists()
		{
			// Arrange
			int key = 5;
			_tree.Insert(key, "Value");

			// Act
			Console.WriteLine($"{_tree}");

			// Assert
			Assert.That(_tree.Contains(key));
		}

		[Test]
		public void Contains_ReturnsFalse_WhenKeyDoesNotExist()
		{
			// Arrange
			int key = 5;

			// Act
			Console.WriteLine($"{_tree}");

			// Assert
			Assert.That(_tree.Contains(key), Is.False);
		}

		[Test]
		public void Count_ReturnsCorrectNumberOfElements()
		{
			// Arrange
			_tree.Insert(1, "Value1");
			_tree.Insert(2, "Value2");
			_tree.Insert(3, "Value3");

			// Act
			Console.WriteLine($"{_tree}");
			int count = _tree.Count;

			// Assert
			Assert.That(count, Is.EqualTo(3));
		}

		[Test]
		public void Insert_AddsKeyValuePairToTree()
		{
			// Arrange
			int key = 5;
			string value = "Value";

			// Act
			_tree.Insert(key, value);
			Console.WriteLine($"{_tree}");

			// Assert
			Assert.That(_tree.Contains(key));
		}

		[Test]
		public void Index_ReturnsReadOnlyCollectionOfKeys()
		{
			// Arrange
			_tree.Insert(1, "Value1");
			_tree.Insert(2, "Value2");
			_tree.Insert(3, "Value3");

			// Act
			var index = _tree.Index;
			Console.WriteLine($"{_tree}");

			// Assert
			Assert.That(index, Has.Count.EqualTo(3));
			Assert.That(index, Does.Contain(1));
			Assert.That(index, Does.Contain(2));
			Assert.That(index, Does.Contain(3));
		}

		[Test]
		public void MaxSize_SetToZero_UnlimitedSize()
		{
			// Arrange
			_tree.MaxSize = 0;

			// Act
			int? maxSize = _tree.MaxSize;

			// Assert
			Assert.That(maxSize, Is.EqualTo(0));
		}

		[Test]
		public void MaxSize_SetToValue_SetsMaximumSizeLimit()
		{
			// Arrange
			int maxSize = 100;
			_tree.MaxSize = maxSize;

			// Act
			int? actualMaxSize = _tree.MaxSize;

			// Assert
			Assert.That(actualMaxSize, Is.EqualTo(maxSize));
		}

		[Test]
		public void SpeedTest_MeasuresExecutionTime()
		{
			// Arrange
			Stopwatch stopwatch = new();
			for (int i = 0; i < 10; i++)
			{
				_tree.Insert(i, $"Key:{i}");
				if (i % 2 == 0)
				{
					_keys.Add(i);
					//Console.WriteLine($"Insert in List: i={i}, Value={_tree.GetValue(i)}");
				}
				//Console.WriteLine($"Insert in Tree: i={i}, Value={_tree.GetValue(i)}");
				Console.WriteLine(_tree.ToString(3));
			}
			Console.WriteLine("Prepare Data Finished");


			// Act
			stopwatch.Start();
			GenericRedBlackTree<string> processList = new();


			// pull the objects on the List from the tree
			foreach (var key in _keys)
			{
				if (_tree.Contains(key))
				{
					processList.Insert(key, _tree.GetValue(key));
				}
				else
				{
					Console.WriteLine($"Get Data in List: Key={key}, was in the processList but not in the tree.");
				}
				Console.WriteLine(_tree.ToString(3));
			}

			// process and Update the objects on the list
			foreach (var entry in processList.GetAll())
			{
				if (_tree.Contains(entry.Key))
				{
					string updatedValue = entry.Value + "_Updated";
					_tree.Update(entry.Key, updatedValue);
				}
				else
				{
					Console.WriteLine($"Update Tree: Key={entry.Key}, was in the processList but not in the tree.");
				}
				Console.WriteLine(_tree.ToString(3));
			}
			
			stopwatch.Stop();

			// Assert
			Assert.Pass($"Elapsed Time: {stopwatch.ElapsedMilliseconds} ms");

			// check to see if changes have been made
			foreach (var item in _tree.GetAll())
			{
				if (item.Key % 2 == 0)
					Assert.That(item.Value, Is.EqualTo($"Key:{item.Key}_Updated"));
				else
					Assert.That(item.Value, Is.EqualTo($"Key:{item.Key}"));
			}

		}

		[Test]
		public void LimitTest_PerformsOperationsAtMaximumSize()
		{
			// Arrange
			_tree.MaxSize = 100;

			// Act
			for (int i = 0; i < 100; i++)
			{
				_tree.Insert(i, $"Value{i}");
			}

			// Assert
			foreach (var item in _tree.GetAll())
			{
				Assert.Multiple(() =>
				{
					Assert.That(item.Key, Is.InRange(0, 99)); // Check that keys are within the expected range
					Assert.That(item.Value, Is.EqualTo($"Value{item.Key}")); // Check that values match the expected pattern
				});
			}

			Assert.Pass("Operations performed at maximum size");
		}

		[Test]
		public void LimitTest_ThrowsExceptionBeyondMaximumSize()
		{
			// Arrange
			_tree.MaxSize = 5; // Set a small maximum size for illustration purposes

			// Act
			for (int i = 0; i < 5; i++)
			{
				_tree.Insert(i, $"Value{i}");
				Console.WriteLine(_tree.ToString(3));
			}

			// Assert
			try
			{
				Assert.Throws<InvalidOperationException>(() => _tree.Insert(6, "Value6"));
			}
			catch (InvalidOperationException)
			{
				Assert.Pass();
			}
			
		}


	}
}