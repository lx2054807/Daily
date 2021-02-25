#pragma once
#include <iostream>
#include <algorithm>
#include <queue>

using namespace std;

/// <summary>
/// https://www.cnblogs.com/QG-whz/p/4536875.html
/// �����������м�������,������ڵ����������Ϊ��,�����������нڵ�ֵ��С�ڸýڵ�ֵ
/// �����������Ϊ��,�����������нڵ�ֵ�����ڸýڵ�ֵ, ͬʱ�������������ֶ��Ƕ���������
/// �����������ʺͶ���,�����������Ľڵ�ֵ���������ظ�
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

    void insert(T key);//�������Ĳ���

    BSTNode<T>* search(T key);//�������Ĳ���

    void preOrder();  //�������
    void inOrder();   //�������
    void postOrder(); //�������
    void levelOrder() //�������
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
    BSTNode<T>* minimumNode();//������С�Ľڵ�
    BSTNode<T>* maximumNode();//�������Ľڵ�

    T minimumKey();//������С�ļ�ֵ
    T maximumKey();//������С�ļ�ֵ

    void print();//��ӡ������
    void remove(T key);

    BSTNode<T>* predecessor(BSTNode<T>* x);//����ĳ������ǰ��
    BSTNode<T>* sucessor(BSTNode<T>* x); //����ĳ�����ĺ��

    void destory();

    //�ڲ�ʹ�ú��������ⲿ�ӿڵ���
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