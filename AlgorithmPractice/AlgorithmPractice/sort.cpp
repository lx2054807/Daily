#include <iostream>
#include <algorithm>
using namespace std;
//所有排序解释默认为升序排序

#pragma region heapsort
/// <summary>
/// 堆排序分为大顶堆和小顶堆,对应升序和降序排列
/// 大顶堆中每个节点的值都大于或等于子节点的值,小顶堆每个节点的值小于等于子节点的值
/// 排序过程为构建堆,把堆首堆尾元素互换,堆尺寸-1,重新构建堆,并重复之前过程知道堆尺寸=1
/// 平均复杂度O(n)
/// </summary>
/// <param name="arr"></param>
/// <param name="start"></param>
/// <param name="end"></param>
void max_heapify(int arr[], int start, int end) 
{
	int dad = start;
	int son = dad * 2 + 1;
	while (son <= end) 
	{
		if (son + 1 <= end && arr[son] < arr[son + 1]) 
		{
			son++;
		}
		if (arr[dad] > arr[son]) 
		{
			return;
		}
		else 
		{
			swap(arr[dad], arr[son]);
			dad = son;
			son = dad * 2 + 1;
		}
	}
}

void min_heapify(int arr[], int start, int end)
{
	int dad = start;
	int son = dad * 2 + 1;
	while (son <= end) 
	{
		if (son + 1 <= end && arr[son] > arr[son+1])
		{
			son++;
		}
		if (arr[son]> arr[dad])
		{
			return;
		}
		else
		{
			swap(arr[dad], arr[son]);
			dad = son;
			son = dad * 2 + 1;
		}
	}
}

void heap_sort(int arr[], int len)
{
	for (int i = len / 2 - 1; i >= 0; i--) 
	{
		min_heapify(arr, i, len - 1);
		//max_heapify(arr, i, len - 1);
	}
	for (int i = len - 1; i > 0; i--) 
	{
		swap(arr[i], arr[0]);
		min_heapify(arr, 0, i - 1);
		//max_heapify(arr, 0, i - 1);
	}
}

#pragma endregion

#pragma region quicksort
/// <summary>
/// 快速排序使用了分治法来进行排序
/// 算法过程为选取数组中一个数作为基准数,进行排序,所有比基准数小的都放在它左边,大的都放在右边,称为分区操作
/// 之后再之前的两区中重新选取基准数并进行刚才的分区操作直到最后所有区内只有一个数,排序完毕
/// 平均复杂度O(nlogn) 最坏情况O(n平方)
/// </summary>
/// <param name="arr"></param>
/// <param name="left"></param>
/// <param name="right"></param>
/// <returns></returns>
int partition(int arr[], int left, int right) 
{
	int pivot = arr[left];
	while (left < right)
	{
		while (left < right && pivot <= arr[right]) { right--; }
		arr[left] = arr[right];
		while (left < right && pivot >= arr[left]) { left++; }
		arr[right] = arr[left];
	}
	arr[left] = pivot;
	return left;
}

void quick_sort(int arr[], int left, int right) 
{
	if (left < right) 
	{
		int pivot = partition(arr, left, right);
		quick_sort(arr, left, pivot-1);
		quick_sort(arr, pivot+1, right);
	}
}

#pragma endregion

#pragma region bubblesort
/// <summary>
/// 最基础的排序,算法步骤为遍历整个数组,比较相邻两个元素的大小,大的放右边,遍历完成后最大的数在最右边
/// 接下来重复之前的操作,但是遍历深度-1,即不取数组最后的数,直到最后所有数都经历过了比较
/// 时间复杂度O(n平方)
/// </summary>
/// <param name="arr"></param>
/// <param name="len"></param>
void bubble_sort(int arr[], int len) 
{
	for (int i =0; i <= len; i++)
	{
		for (int j = 0 ; j <= len-1-i; j++) 
		{
			if (arr[j] > arr[j+1])
			{
				swap(arr[j], arr[j+1]);
			}
		}
	}
}

#pragma endregion

#pragma region selectsort
/// <summary>
/// 选择排序算法步骤为遍历数组,选定每次遍历目标数后,遍历数组剩下的数,找到最小的数与遍历目标交换直到数组有序
/// 时间复杂度O(n平方)
/// </summary>
/// <param name="arr"></param>
/// <param name="len"></param>
void select_sort(int arr[], int len)
{
	for (int i =0; i<= len; i++)
	{
		int min = i;
		for (int j = i; j <= len; j++)
		{
			if (arr[min] > arr[j])
			{
				min = j;
			}
		}
		swap(arr[i], arr[min]);
	}
}

#pragma endregion

#pragma region insertsort
/// <summary>
/// 插入排序的思路为构造一个有序数组,然后从原始数组中取数遍历有序数组,插入对应位置
/// 复杂度为O(n平方)
/// </summary>
/// <param name="arr"></param>
/// <param name="len"></param>
void insert_sort(int arr[], int len)
{
	for (int i = 1; i <= len; i++)
	{
		int tmp = arr[i];
		for(int j = i-1; j >=0;j--)
		{
			if (arr[j] > tmp) 
			{
				arr[j + 1] = arr[j];
				arr[j] = tmp;
			}
			else
			{
				break;
			}
		}
	}
}

#pragma endregion

//int main()
//{
//	int arr[] = { 1,1,13,11,6,7,3,34,23,12,5,23,42,63,12,7,26,9,15 };
//	int len = (int)sizeof(arr) / (sizeof(*arr));
//	insert_sort(arr,len-1);
//	for (int i = 0; i < len; i++) {
//		cout << arr[i] << endl;
//	}
//	return 0;
//}