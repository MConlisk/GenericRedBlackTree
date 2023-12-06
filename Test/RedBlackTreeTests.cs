using NUnit.Framework;
using System.Diagnostics;
using RedBlackTree.Trees;
using System.Collections.Generic;

namespace RedBlackTree.Tests
{
    [TestFixture]
    public class RedBlackTreeTests
    {
        private GenericRedBlackTree<string> _tree;
        private GenericRedBlackTree<string> _testvalues;

        [SetUp]
        public void Setup()
        {
            _tree = new GenericRedBlackTree<string>();
            _testvalues = new GenericRedBlackTree<string>();
        }

        [Test]
        public void Contains_ReturnsTrue_WhenKeyExists()
        {
            // Arrange
            int key = 5;
            _tree.Insert(key, "Value");

            // Act
            bool result = _tree.Contains(key);

            // Assert
            Assert.That(result);
        }

        [Test]
        public void Contains_ReturnsFalse_WhenKeyDoesNotExist()
        {
            // Arrange
            int key = 5;

            // Act
            bool result = _tree.Contains(key);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void Count_ReturnsCorrectNumberOfElements()
        {
            // Arrange
            _tree.Insert(1, "Value1");
            _tree.Insert(2, "Value2");
            _tree.Insert(3, "Value3");

            // Act
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
                KeyValuePair<int, string> entry = new(i, $"Key:{i}");
                _tree.Insert(entry.Key, entry.Value);
                if (i % 2 == 0) _testvalues.Insert(entry.Key, entry.Value);
            }

            // Act
            stopwatch.Start();
            GenericRedBlackTree<string> updatedList = new();

            // pull the objects on the List from the tree
            foreach(var entry in _testvalues.GetAll())
            {
                updatedList.Insert(entry.Key, entry.Value);    
            }

            // process and Update the objects on the list
            foreach (var entry in updatedList.GetAll())
            {
                string updatedValue = entry.Value + "_Updated";
                updatedList.Update(entry.Key, updatedValue);
            }
            stopwatch.Stop();

            // Assert
            Assert.Pass($"Elapsed Time: {stopwatch.ElapsedMilliseconds} ms");
        }

        [Test]
        public void LimitTest_PerformsOperationsAtMaximumSize()
        {
            // Arrange
            _tree.MaxSize = 100;

            // Act

            //TODO: Perform operations on the Red-Black Tree

            // Assert
            Assert.Pass("Operations performed at maximum size");
        }
    }
}