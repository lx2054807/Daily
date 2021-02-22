#include <iostream>
#include <algorithm>
using namespace std;
//�����������Ĭ��Ϊ��������

#pragma region heapsort
/// <summary>
/// �������Ϊ�󶥶Ѻ�С����,��Ӧ����ͽ�������
/// �󶥶���ÿ���ڵ��ֵ�����ڻ�����ӽڵ��ֵ,С����ÿ���ڵ��ֵС�ڵ����ӽڵ��ֵ
/// �������Ϊ������,�Ѷ��׶�βԪ�ػ���,�ѳߴ�-1,���¹�����,���ظ�֮ǰ����֪���ѳߴ�=1
/// ƽ�����Ӷ�O(n)
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
/// ��������ʹ���˷��η�����������
/// �㷨����Ϊѡȡ������һ������Ϊ��׼��,��������,���бȻ�׼��С�Ķ����������,��Ķ������ұ�,��Ϊ��������
/// ֮����֮ǰ������������ѡȡ��׼�������иղŵķ�������ֱ�������������ֻ��һ����,�������
/// ƽ�����Ӷ�O(nlogn) ����O(nƽ��)
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
/// �����������,�㷨����Ϊ������������,�Ƚ���������Ԫ�صĴ�С,��ķ��ұ�,������ɺ������������ұ�
/// �������ظ�֮ǰ�Ĳ���,���Ǳ������-1,����ȡ����������,ֱ��������������������˱Ƚ�
/// ʱ�临�Ӷ�O(nƽ��)
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
/// ѡ�������㷨����Ϊ��������,ѡ��ÿ�α���Ŀ������,��������ʣ�µ���,�ҵ���С���������Ŀ�꽻��ֱ����������
/// ʱ�临�Ӷ�O(nƽ��)
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
/// ���������˼·Ϊ����һ����������,Ȼ���ԭʼ������ȡ��������������,�����Ӧλ��
/// ���Ӷ�ΪO(nƽ��)
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