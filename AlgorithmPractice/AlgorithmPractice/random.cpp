#include <iostream>
#include <algorithm>
using namespace std;
const int seed = 1024;

#pragma region ����ͬ�෨

int last_rand_01 = -1;
/// <summary>
/// ����ͬ�෨
/// �õ�ǰα�������A��B���Cȡ��õ�����������D��Ϊ��һ��α�����
/// �����������Χ����C,Dȷ��
/// ���߱�����Ԥ����
/// </summary>
/// <param name="seed"></param>
/// <returns></returns>
int rand_01()
{
	int R = last_rand_01;
	int A = 17;
	int B = 824;
	int C = 10;	
	int D = 5;
	if (R == -1) 
	{
		last_rand_01 = (A * R + B) % C + D;
		return last_rand_01;
	}
	else
	{
		last_rand_01 = (A * R + B) % C + D;
		return last_rand_01;
	}
}

#pragma endregion

#pragma region ����ɢ�к�����
/// �������û��ʵ��ֻ��ԭ����
/// ����ɢ�к���Ҳ��Hash����,�ǰ����ⳤ������ı�Ϊ�̶����������������ͨ�����ת���õ�����ĺ���
/// ����������MD5,��������SHA,MAC,CRC�ȵ�
/// ���������ԭ��Ϊͨ�����ӳ�ʼ����������ֵ,ͨ������ɢ�к�������α�������,��������1
/// ���е�����,����Ԥ����
#pragma endregion

#pragma region ���ܷ�
/// ʹ�����뷨��������ǿα�����,����ʹ��AES�ԳƼ��ܻ�RSA��Կ����
/// ͨ�����ӳ�ʼ���ڲ�״̬,����Կ�����ڲ���������ֵ,�����ܵ�ֵ��Ϊ��������ͬʱ�Ѽ�������ֵ��1
/// ����Ļ�����֧����������Ĳ���Ԥ����
#pragma endregion

int main() 
{
	cout << rand_01()<<endl;
	cout << rand_01() << endl;
	cout << rand_01() << endl;
	cout << rand_01() << endl;

	return 0;
}