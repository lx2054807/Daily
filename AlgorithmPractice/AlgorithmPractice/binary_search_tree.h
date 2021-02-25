#pragma once
#include <iostream>
#include <algorithm>
#include <queue>

using namespace std;

/// <summary>
/// https://www.cnblogs.com/QG-whz/p/4536875.html
/// 二叉搜索树有几个性质,如果根节点的左子树不为空,则左子树所有节点值都小于该节点值
/// 如果右子树不为空,则右子树所有节点值都大于该节点值, 同时它的左右子树又都是二叉搜索树
/// 根据它的性质和定义,二叉搜索树的节点值不允许有重复
/// </summary>

template<typename T>
class BSTNode
{
public:
	T _key;
	BSTNode* _lchild;
	BSTNode* _rchild;
	BSTNode* _parent;
	BSTNode(T key, BSTNode* lchild, BSTNode* rchild, BSTNode* parent) :_key(key), _lchild(lchild), _rchild(rchild), _parent(parent) {};
};

template<typename T>
class BST
{
private:
	BSTNode<T>* _root;
public:
	BST() :_root(nullptr) {};
	~BST() {};

    void insert(T key);//二叉树的插入

    BSTNode<T>* search(T key);//二叉树的查找

    void preOrder();  //先序输出
    void inOrder();   //中序输出
    void postOrder(); //后序输出
    void levelOrder() //层序输出
    {
        queue<BSTNode<T>> nodes;
        nodes.push(_root);
        while (!nodes.empty()) 
        {
            BSTNode<T> node = nodes.front();
            cout << node._key << endl;
            nodes.pop();
            if (node._lchild != nullptr)
            {
                nodes.push(node._lchild);
            }
            if (node._rchild != nullptr)
            {
                nodes.push(node._rchild);
            }
        }
    }  
    BSTNode<T>* minimumNode();//查找最小的节点
    BSTNode<T>* maximumNode();//查找最大的节点

    T minimumKey();//查找最小的键值
    T maximumKey();//查找最小的键值

    void print();//打印二叉树
    void remove(T key);

    BSTNode<T>* predecessor(BSTNode<T>* x);//查找某个结点的前驱
    BSTNode<T>* sucessor(BSTNode<T>* x); //查找某个结点的后继

    void destory();

    //内部使用函数，供外部接口调用
private:
    void insert(BSTNode<T>*& tree, BSTNode<T>* z);
    BSTNode<T>* search(BSTNode<T>*& tree, T key) const;
    void preOrder(BSTNode<T>*& tree) const;
    void inOrder(BSTNode<T>*& tree) const;
    void postOrder(BSTNode<T>*& tree) const;
    BSTNode<T>* minimumNode(BSTNode<T>*& tree);
    BSTNode<T>* maximumNode(BSTNode<T>*& tree);
    void print(BSTNode<T>*& tree);
    BSTNode<T>* remove(BSTNode<T>*& tree, BSTNode<T>* z);
    void destory(BSTNode<T>*& tree);
};