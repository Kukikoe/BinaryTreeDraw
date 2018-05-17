using System;
using System.Collections.Generic;

namespace PaintBinaryTree
{
    public class MyBinaryTreeItem<T>
    {
        public T Value;
        public MyBinaryTreeItem<T> parent;
        public MyBinaryTreeItem<T> left;
        public MyBinaryTreeItem<T> right;
        public MyBinaryTreeItem(T Value, MyBinaryTreeItem<T> parent = null, MyBinaryTreeItem<T> left = null, MyBinaryTreeItem<T> right = null)
        {
            this.Value = Value;
            this.parent = parent;
            this.left = left;
            this.right = right;
        }
    }
    public class MyBinaryTree<T> where T : IComparable
    {

        #region fields
        
        private MyBinaryTreeItem<T> _root = null;
        private bool allowDuplicateKeys = false;
        private int _count = 0;
        public int Count { get => _count; }
        public MyBinaryTreeItem<T> RootItem
        {
            get
            {
                return _root;               
            }
        }
        #endregion

        #region private methods
        /// <summary>
        /// Add new item
        /// </summary>
        /// <param name="pair"></param>
        /// <param name="item"></param>
        private void Add(T value, MyBinaryTreeItem<T> item)
        {
            if (!allowDuplicateKeys && value.CompareTo(item.Value) == 0)
            {
                item.Value = value;
            }
            else if (value.CompareTo(item.Value) < 0)
            {
                //go to left
                if (item.left == null)
                {
                    item.left = new MyBinaryTreeItem<T>(value, item);
                    ++_count;
                }
                else
                {
                    Add(value, item.left);
                }
            }
            else //go to right
            {
                if (item.right == null)
                {
                    item.right = new MyBinaryTreeItem<T>(value, item);
                    ++_count;
                }
                else
                {
                    Add(value, item.right);
                }
            }
        }

        private IEnumerator<MyBinaryTreeItem<T>> GetBinaryTreeItemEnumerator(MyBinaryTreeItem<T> item)
        {
            Stack<MyBinaryTreeItem<T>> itemStack = new Stack<MyBinaryTreeItem<T>>();
            while (item != null || itemStack.Count != 0)
            {
                if (itemStack.Count != 0)
                {
                    item = itemStack.Pop();

                    //this is my action
                    yield return item;

                    if (item.right != null)
                    {
                        item = item.right;
                    }
                    else
                    {
                        item = null;
                    }
                }
                while (item != null)
                {
                    itemStack.Push(item);
                    item = item.left;
                }
            }
        }

        private MyBinaryTreeItem<T> GetItemFromValue(T value, MyBinaryTreeItem<T> item)
        {
            if (item.Value.Equals(value))
                return item;
            else if (item.Value.CompareTo(value) > 0)
            {
                if (item.left != null)
                    return GetItemFromValue(value, item.left);
            }
            else
            {
                if (item.right != null)
                    return GetItemFromValue(value, item.right);
            }
            return null;
        }

        private void RemoveItem(MyBinaryTreeItem<T> item, MyBinaryTreeItem<T> parent)
        {
            if (item.left == null && item.right == null)
            {
                RemoveItemWithoutChildren(item, parent);
            }
            else if (item.left != null && item.right != null)
            {
                RemoveItemWithBothChildren(item, parent);
            }
            else
            {
                RemoveItemWithOneChild(item, parent);
            }
            --_count;
        }

        private void RemoveItemWithoutChildren(MyBinaryTreeItem<T> item, MyBinaryTreeItem<T> parent)
        {
            if (item == _root)
            {
                _root = null;
                return;
            }
            if (parent.left == item)
            {
                parent.left = null;
            }
            else
            {
                parent.right = null;
            }
        }

        private void RemoveItemWithOneChild(MyBinaryTreeItem<T> item, MyBinaryTreeItem<T> parent)
        {
            MyBinaryTreeItem<T> childItem;

            childItem = item.left != null ? item.left : item.right;

            if (parent.left == item)
            {
                parent.left = childItem;
            }
            else
            {
                parent.right = childItem;
            }

            childItem.parent = parent;
        }

        private void RemoveItemWithBothChildren(MyBinaryTreeItem<T> item, MyBinaryTreeItem<T> parent)
        {
            //Find successor Node
            MyBinaryTreeItem<T> success = item.right;
            MyBinaryTreeItem<T> successParent = item;
            bool tmp = true;
            while (success.left != null)
            {
                tmp = false;
                successParent = success;
                success = success.left;
            }
            if (tmp)
            {
                item.Value = item.right.Value;
                item.right = item.right.right;
                return;
            }
            item.Value = success.Value;
            successParent.left = success.right;
            if (success.right != null)
                success.right.parent = successParent;
        }
        #endregion

        #region public methods
        public void Add(T value)
        {
            if (_root == null)
            {
                _root = new MyBinaryTreeItem<T>(value);
                ++_count;
            }
            else
            {
                Add(value, _root);
            }
        }
        /// <summary>
        /// Remove item from tree
        /// </summary>
        /// <param name="value">value of item to deleting</param>
        public void Remove(T value)
        {
            var item = GetItemFromValue(value, RootItem);
            if (item != null)
                RemoveItem(item, item.parent);

        }

        public IEnumerator<T> GetEnumerator()
        {
            using (IEnumerator<MyBinaryTreeItem<T>> e = GetBinaryTreeItemEnumerator(_root))
            {
                while (e.MoveNext())
                {
                    yield return e.Current.Value;
                }
            }
        }
        #endregion
    }
}
