// binaryTree.js

// Node for the BST
class TreeNode {
    constructor(course) {
      this.course = course;
      this.left = null;
      this.right = null;
    }
  }
  
  // Binary Search Tree Class
  class BinarySearchTree {
    constructor() {
      this.root = null;
    }
  
    // Insert a course into the tree
    insert(course) {
      const node = new TreeNode(course);
      if (!this.root) {
        this.root = node;
      } else {
        this._insertNode(this.root, node);
      }
    }
  
    _insertNode(current, node) {
      // Compare by course.id (lexicographically)
      if (node.course.id < current.course.id) {
        if (!current.left) {
          current.left = node;
        } else {
          this._insertNode(current.left, node);
        }
      } else {
        if (!current.right) {
          current.right = node;
        } else {
          this._insertNode(current.right, node);
        }
      }
    }
  
    // Search for a course by ID
    search(id) {
      return this._searchNode(this.root, id);
    }
  
    _searchNode(node, id) {
      if (!node) return null;
      if (node.course.id === id) return node.course;
      if (id < node.course.id) {
        return this._searchNode(node.left, id);
      } else {
        return this._searchNode(node.right, id);
      }
    }
  
    // Return all courses in order (sorted by id)
    inOrder() {
      const result = [];
      this._inOrderTraverse(this.root, result);
      return result;
    }
  
    _inOrderTraverse(node, result) {
      if (node) {
        this._inOrderTraverse(node.left, result);
        result.push(node.course);
        this._inOrderTraverse(node.right, result);
      }
    }
  
    // Filter courses by major
    filterByMajor(major) {
      const result = [];
      this._filterByMajor(this.root, major, result);
      return result;
    }
  
    _filterByMajor(node, major, result) {
      if (node) {
        this._filterByMajor(node.left, major, result);
        if (node.course.major === major) {
          result.push(node.course);
        }
        this._filterByMajor(node.right, major, result);
      }
    }
  
    // Filter courses by period
    filterByPeriod(period) {
      const result = [];
      this._filterByPeriod(this.root, period, result);
      return result;
    }
  
    _filterByPeriod(node, period, result) {
      if (node) {
        this._filterByPeriod(node.left, period, result);
        if (node.course.period === period) {
          result.push(node.course);
        }
        this._filterByPeriod(node.right, period, result);
      }
    }
  }
  
  module.exports = BinarySearchTree;
  