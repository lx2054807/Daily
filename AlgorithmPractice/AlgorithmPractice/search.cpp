#include <iostream>
#include <algorithm>
using namespace std;

#pragma region sequencesearch

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

int main()
{
	//int arr[] = { 1,1,13,11,6,7,3,34,23,12,5,23,42,63,12,7,26,9,15 };
	int arr[] = { 1,1,2,4,4,5,6,7,11,13 };
	int len = (int)sizeof(arr) / (sizeof(*arr));
	int target = 5;
	int targetIdx = binary_search(arr, len, target);
	cout << targetIdx << endl;
	return 0;
}