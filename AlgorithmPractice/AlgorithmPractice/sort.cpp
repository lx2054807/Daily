#include <iostream>
#include <algorithm>
using namespace std;

#pragma region heapsort

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

int main()
{
	int arr[] = { 1,1,13,11,6,7,3,34,23,12,5,23,42,63,12,7,26,9,15 };
	int len = (int)sizeof(arr) / (sizeof(*arr));
	quick_sort(arr, 0,len-1);
	for (int i = 0; i < len; i++) {
		cout << arr[i] << endl;
	}
	return 0;
}