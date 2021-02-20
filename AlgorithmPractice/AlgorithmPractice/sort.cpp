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

int main()
{
	int arr[] = { 1,1,13,11,6,7,3 };
	int len = (int)sizeof(arr) / (sizeof(*arr));
	heap_sort(arr, len);
	for (int i = 0; i < len; i++) {
		cout << arr[i] << endl;
	}
	return 0;
}