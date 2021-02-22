#include <iostream>
#include <algorithm>
using namespace std;

#pragma region sequencesearch
/// <summary>
/// 顺序查找其实就是遍历
/// 复杂度O(n)
/// </summary>
/// <param name="arr"></param>
/// <param name="len"></param>
/// <param name="target"></param>
/// <returns></returns>
int sequence_search(int arr[], int len, int target) 
{
	for(int i=0; i< len; i++)
	{
		if (arr[i] == target) {
			return i;
		}
	}
	return -1;
}

#pragma endregion

#pragma region binarysearch

/// <summary>
/// 二分查找只能用于有序数组
/// 如果是无序数组需要经历排序
/// 适合不频繁插入删除的静态查找表
/// 复杂度O(logn) 最坏情况O(log(n+1))
/// </summary>
/// <param name="arr"></param>
/// <param name="len"></param>
/// <param name="target"></param>
/// <returns></returns>
int binary_search(int arr[], int len, int target)
{
	int left = 0, right = len, mid = (left + right) / 2;
	while(left <= right)
	{
		if (arr[mid] == target) {
			return mid;
		}
		else if (arr[mid] > target) {
			left = mid+1;
			mid = (left + right) / 2;
		}
		else {
			right = mid-1;
			mid = (left + right) / 2;
		}
	}
	return -1;
}

#pragma endregion

#pragma region insertionsearch
/// <summary>
/// 插值查找类似于优化后的二分查找
/// 二分查找每次都是折一半, 插值查找会根据目标值与当前查找部分的关系计算自适应折点
/// 插值查找同样只适用于有序数组,适合分布均匀的顺序数组
/// 复杂度O(log(logn)) 
/// </summary>
/// <param name="arr"></param>
/// <param name="left"></param>
/// <param name="right"></param>
/// <param name="target"></param>
/// <returns></returns>
int insertion_search(int arr[], int left, int right, int target)
{
	int mid = left + (target - arr[left]) * (right - left) / (arr[right] - arr[left]) ;
	if (arr[mid] == target) {
		return mid;
	}
	else if(arr[mid] > target)
	{
		insertion_search(arr, left, mid - 1, target);
	}
	else
	{
		insertion_search(arr, mid + 1, right, target);
	}
}

#pragma endregion

int main()
{
	//int arr[] = { 1,1,13,11,6,7,3,34,23,12,5,23,42,63,12,7,26,9,15 };
	int arr[] = { 1,1,2,4,4,5,6,7,11,13 };
	int len = (int)sizeof(arr) / (sizeof(*arr));
	int target = 5;
	int targetIdx = insertion_search(arr, 0,len-1, target);
	cout << targetIdx << endl;
	return 0;
}