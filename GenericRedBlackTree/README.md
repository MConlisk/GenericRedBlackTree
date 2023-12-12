![Screw Apple Logo](/Assets/Logo.png)
# Universal Tree Implementation in C# with GenericFactoryPool

## Overview

This project is an implementation of a universal tree data structure in C# using the GenericFactoryPool NuGet package. The universal tree model provides a foundation for various tree structures, with the current focus on a Red-Black Tree implementation.

## Purpose

The primary purpose of this implementation is to create a flexible and extensible tree structure capable of supporting different tree types. The current version includes a Red-Black Tree model, and future updates will introduce additional tree implementations.

## Features

- **Universal Tree Model:** The core implementation of a tree structure with generic operations suitable for various tree types.

- **Red-Black Tree Model:** An example implementation utilizing the universal tree model, providing methods for insertion, deletion, and retrieval of key-value pairs.

- **Balancing Mechanism:** The Red-Black Tree incorporates a balancing mechanism to ensure that the tree remains balanced after insertions and deletions.

- **Traversing Functionality:** Enables traversal of the tree to retrieve all key-value pairs or those meeting specific conditions.

- **Exception Handling:** Custom exceptions like `DuplicateKeyException`, `InsertTraversalException`, `RemoveTraversalException`, and `TreeBalanceException` provide meaningful error messages for better debugging.

## How to Use

### 1. Initialization

```csharp
// Example of initializing the Universal Tree
var rbtBalancer = new RedBlackBlancer();
var rbtTraverser = new RedBlackInOrderTraverser();
var rbtModel = new RedBlackTreeModel(rbtTraverser, rbtBalancer)
var universalTree = new UniversalTree<int, string>();
