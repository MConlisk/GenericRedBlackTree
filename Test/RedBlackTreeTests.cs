using RedBlackTree.Trees;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Test;

public class RedBlackTreeTests
{
	public const int amount = 1000000;
	public const int size = 1000100;

	private readonly bool InfoComments = false;
	private readonly bool TimeComments = true;
	private readonly bool DevComments = true;

	private HashSet<int> Counted;
	private GenericRedBlackTree<string>[] TestTree;

	private readonly Stopwatch SW = new();
	private readonly HashSet<int> Subset = new();
	private readonly List<KeyValuePair<int, string>> BulkCollection = new();

	[SetUp]
	public void Setup()
	{
		TestTree = new[2](size);
		Counted = new();
		if (InfoComments) Console.WriteLine($"Bulk Collection Setup.");
		for (int i = 0; i < amount; i++)
		{
			var item = new KeyValuePair<int, string>(i, $"Number {i}");
			BulkCollection.Add(item);
			if (i%2 == 0)
			{
				Subset.Add(i);
				if (InfoComments) Console.WriteLine($"Added Item:{item.Key},to Subset Collection.");
			}

			if (InfoComments) Console.WriteLine($"Added Item:{item.Key}, Value:{item.Value} to Bulk Collection.");
		}
		if (InfoComments) Console.WriteLine($"Bulk Collection Setup Complete.");

		if (InfoComments) Console.WriteLine($"Test Bulk Insert");
		SW.Start();
		TestTree.BulkInsert(BulkCollection);
		if (TimeComments) Console.WriteLine($"Bulk Insert took {SW.ElapsedMilliseconds} milliseconds.");
		SW.Stop();
		SW.Reset();
	}

	[Test]
	public void GetAllTest()
	{
		try
		{
			if (InfoComments) Console.WriteLine($"Test Get All");
			SW.Start();
			foreach (var item in TestTree.GetAll())
			{
				Counted.Add(item.Key);
				if (InfoComments) Console.WriteLine($"Item:{item.Key}, Value:{item.Value}");
			}
			if (TimeComments) Console.WriteLine($"Get All Counted {Counted.Count} in {SW.ElapsedMilliseconds} milliseconds.");
			SW.Stop();
			SW.Reset();
		}
		catch (Exception e)
		{
			if (DevComments)
			{
				Console.WriteLine($"Tree Count: {TestTree.Count}");
				Console.WriteLine($"Bulk Collection Count: {BulkCollection.Count}");
				Console.WriteLine($"Exception {e.Message} caught.");
				Console.WriteLine($"StackTrace {e.StackTrace}.");
			}
			Assert.Fail();
		}
		TestTree.ResetState();
	}


	[Test]
	public void GetListTest()
	{
		try
		{
			if (InfoComments) Console.WriteLine($"Test GetSet");
			Counted = new();

			SW.Start();
			foreach (var item in TestTree.GetSet(Subset))
			{
				Counted.Add(item.Key);
				if (InfoComments) Console.WriteLine($"Item:{item.Key}, Value:{item.Value}");
			}
			if (TimeComments) Console.WriteLine($"GetSet Counted {Counted.Count} in {SW.ElapsedMilliseconds} milliseconds.");
			SW.Stop();
			SW.Reset();
		}
		catch (Exception e)
		{
			if (DevComments) Console.WriteLine($"Exception {e.Message} caught.");
			if (DevComments) Console.WriteLine($"StackTrace {e.StackTrace}.");
			Assert.Fail();
		}
	}
}